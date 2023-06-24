using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Suburb.Screens
{
    public class ScreensService
    {
        private readonly string resourcesRoot;
        private readonly IFactory<string, BaseScreen> screensFactory;
        private readonly Transform screensRoot;
        private readonly Dictionary<Type, BaseScreen> screensCache = new();
        private readonly Stack<BaseScreen> screensStack = new();

        public ScreensService(IFactory<string, BaseScreen> screensFactory, string resourcesRoot)
        {
            this.resourcesRoot = resourcesRoot;
            this.screensFactory = screensFactory;

            GameObject screenRootObject = new GameObject("UI Root");
            screensRoot = screenRootObject.transform;
            GameObject.DontDestroyOnLoad(screenRootObject);
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

        private TScreen GetOrCreateScreen<TScreen>()
            where TScreen : BaseScreen
        {
            Type screenType = typeof(TScreen);

            if (screensCache.TryGetValue(screenType, out BaseScreen baseScreen))
            {
                return baseScreen as TScreen;
            }

            BaseScreen screen = screensFactory.Create($"{resourcesRoot}/{typeof(TScreen).Name}") as TScreen;
            screensCache[screenType] = screen;
            screen.transform.SetParent(screensRoot);

            return screen as TScreen;
        }
    }

}