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

        private BaseScreen previousScreen;
        private BaseScreen currentScreen;

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
            previousScreen = currentScreen;
            previousScreen?.InitHide();

            currentScreen = GetOrCreateScreen<TScreen>();
            currentScreen.InitShow();

            return currentScreen as TScreen;
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