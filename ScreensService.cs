using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Suburb.ExpressRouter;

namespace Suburb.Screens
{
    public class ScreensService
    {
        private readonly ScreensFactory screensFactory;
        private readonly Dictionary<Type, BaseScreen> screensCache = new();
        private readonly Router router = new();

        private const string BASE_SCREEN = "BaseScreen";
        public static string UI_CAMERA { get; } = "uiCamera";
        
        public ScreensService(ScreensFactory screensFactory)
        {
            this.screensFactory = screensFactory;

            router.Use((from, to) =>
            {
                BaseScreen fromScreen = from as BaseScreen;
                BaseScreen toScreen = to as BaseScreen;

                if (fromScreen != null)
                {
                    fromScreen.GoBack();
                    fromScreen.InitHide();
                }

                toScreen.InitShow();
            });
        }

        public TScreen GoTo<TScreen>()
            where TScreen : BaseScreen
        {
            Type screenType = typeof(TScreen);
            BaseScreen currentScreen = GetOrCreateScreen<TScreen>();
            return router.GoTo(screenType.Name) ? currentScreen as TScreen : null;
        }

        public BaseScreen GoToPrevious()
        {
            router.GoToPrevious();
            return router.GetLast() as BaseScreen;
        }

        public BaseScreen GoToPrevious<TScreen>()
            where TScreen : BaseScreen
        {
            Type screenType = typeof(TScreen);

            foreach(var endpoint in router.GetPathToPrevious(screenType.Name, true))
            {
                BaseScreen screen = endpoint as BaseScreen;
                screen.GoBack();
            }

            router.GoToPrevious(screenType.Name);
            return router.GetLast() as BaseScreen;
        }

        public IDisposable UseTransition<TFrom, TTo>(Action<TFrom, TTo> onTransition)
            where TFrom : BaseScreen
            where TTo : BaseScreen
        {
            string from = typeof(TFrom).Name;
            string to = typeof(TTo).Name;
            return router.Use(
                (pFrom, pTo) => onTransition.Invoke(pFrom as TFrom, pTo as TTo), 
                from == BASE_SCREEN ? string.Empty : from, 
                to == BASE_SCREEN ? string.Empty : to);
        }
        
        private TScreen GetOrCreateScreen<TScreen>()
            where TScreen : BaseScreen
        {
            Type screenType = typeof(TScreen);

            if (!screensCache.TryGetValue(screenType, out BaseScreen screen))
            {
                screen = screensFactory.Create(screenType) as TScreen;
                router.AddEndpoint(screen);
            }

            screensCache[screenType] = screen;
            return screen as TScreen;
        }
    }

}