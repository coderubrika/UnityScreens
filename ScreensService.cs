using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Suburb.Screens
{
    public class ScreensService
    {
        private readonly ScreensFactory screensFactory;
        private readonly Transform screensRoot;
        private readonly Dictionary<Type, BaseScreen> screensCache = new();
        private readonly Stack<BaseScreen> screensStack = new();

        public ScreensService(ScreensFactory screensFactory)
        {
            this.screensFactory = screensFactory;
        }

        public TScreen GoTo<TScreen>()
            where TScreen : BaseScreen
        {
            if (screensStack.Count > 0)
                screensStack.Peek().InitHide();

            BaseScreen currentScreen = GetOrCreateScreen<TScreen>();
            screensStack.Push(currentScreen);
            currentScreen.InitShow();

            return currentScreen as TScreen;
        }

        public BaseScreen GoToPrevious()
        {
            if (screensStack.Count < 2)
                return null;

            BaseScreen previousScreen = screensStack.Pop();
            BaseScreen currentScreen = screensStack.Peek();

            previousScreen.InitHide();
            currentScreen.InitShow();

            return currentScreen;
        }

        public BaseScreen GoToPrevious<T>()
            where T : BaseScreen
        {
            if (screensStack.Count < 2)
                return null;

            BaseScreen previousScreen = screensStack.Pop();
            T currentScreen = null;

            while (screensStack.Count > 0)
            {
                currentScreen = screensStack.Pop() as T;
                if (currentScreen != null)
                {
                    screensStack.Push(currentScreen);
                    break;
                }
            }

            if (currentScreen == null)
                currentScreen = GetOrCreateScreen<T>();

            previousScreen.InitHide();
            currentScreen.InitShow();

            return currentScreen;
        }

        private TScreen GetOrCreateScreen<TScreen>()
            where TScreen : BaseScreen
        {
            Type screenType = typeof(TScreen);

            if (screensCache.TryGetValue(screenType, out BaseScreen baseScreen))
            {
                return baseScreen as TScreen;
            }

            BaseScreen screen = screensFactory.Create(screenType) as TScreen;
            screensCache[screenType] = screen;
            screen.transform.SetParent(screensRoot);

            return screen as TScreen;
        }
    }

}