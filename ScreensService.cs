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
        
        /*
         *в общем в чем соль в том что мне нужно сначала испольнить взаимодествие
         * точки от до для "от" и только потом от до для "до"
         * причем это может и не касаться других обработок
         * таким образом мне нужно поддерживать цепочки переходов
         * если не оверинженерить то тут идеально подойдет цепочка обвервабле
         * как это обыграть
         *
         * роутер не должен принимать экшон а даже если должен
         *
         *
         * по факту мне надо добавить анимацию из from добавить анимацию из to
         * мне переход нужно сделать по середине
         *
         * я могу прописать переходы от до сколько угодно штук
         * но мне блин нужно как то их пометить мать твою
         * поэтому я могу ввести 3 типа перехода
         * от до (от); от до (середина); от до (до)
         */
        
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