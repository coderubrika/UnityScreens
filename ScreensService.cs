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

            router.Use(new ActItem<FromTo>((points, next) =>
            {
                BaseScreen fromScreen = points.From as BaseScreen;
                BaseScreen toScreen = points.To as BaseScreen;

                if (fromScreen != null)
                {
                    fromScreen.GoBack();
                    fromScreen.InitHide();
                }

                toScreen.InitShow();
                next.Invoke(points);
            }));
        }

        public TScreen GoTo<TScreen>()
            where TScreen : BaseScreen
        {
            Type screenType = typeof(TScreen);
            BaseScreen currentScreen = GetOrCreateScreen<TScreen>();
            return router.GoTo(screenType.Name) ? (TScreen)currentScreen : null;
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

        public IDisposable UseTransition<TFrom, TTo>(ActItem<FromTo> transition, MiddlewareOrder order)
            where TFrom : BaseScreen
            where TTo : BaseScreen
        {
            string from = typeof(TFrom).Name;
            string to = typeof(TTo).Name;
            from = from == BASE_SCREEN ? null : from;
            to = to == BASE_SCREEN ? null : to;
            
            return router.Use(transition, from, to, order);
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