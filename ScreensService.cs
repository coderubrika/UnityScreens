using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Suburb.Screens
{
    public class ScreensService
    {
        private readonly ScreensFactory screensFactory;
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
            {
                BaseScreen screen = screensStack.Peek();
                screen.InitHide();

                if (typeof(TScreen) == screen.GetType())
                    return screen as TScreen;
            }

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

            previousScreen.GoBack();
            previousScreen.InitHide();
            currentScreen.InitShow();

            return currentScreen;
        }

        public BaseScreen GoToPrevious<T>()
            where T : BaseScreen
        {
            if (screensStack.Peek().GetType() == typeof(T))
                return screensStack.Peek() as T;

            if (screensStack.Count < 2)
                return null;

            BaseScreen previousScreen = screensStack.Pop();
            previousScreen.GoBack();
            previousScreen.InitHide();

            T currentScreen = null;

            while (screensStack.Count > 0)
            {
                BaseScreen transitionalScreen = screensStack.Pop();
                currentScreen = transitionalScreen as T;
                if (currentScreen != null)
                {
                    screensStack.Push(currentScreen);
                    break;
                }
                else
                {
                    transitionalScreen.GoBack();
                    transitionalScreen.InitHide();
                }
            }

            if (currentScreen == null)
                currentScreen = GetOrCreateScreen<T>();

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

            return screen as TScreen;
        }
    }

}