using System;
using UnityEngine;
using Zenject;

namespace Suburb.Screens
{
    public class ScreensFactory : IFactory<Type, BaseScreen>
    {
        private readonly DiContainer container;
        private readonly string uiRootPath;
        
        private readonly Transform uiRoot;

        public ScreensFactory(DiContainer container, string uiRootPath)
        {
            this.container = container;
            this.uiRootPath = uiRootPath;

            var uiRootGameObject = new GameObject("UIRoot");
            GameObject.DontDestroyOnLoad(uiRootGameObject);
            uiRoot = uiRootGameObject.transform;
        }

        public BaseScreen Create(Type screenType)
        {
            string resourcePath = $"{uiRootPath}/{screenType.Name}";
            var prefab = Resources.Load(resourcePath);

            return (BaseScreen)container.InstantiatePrefabForComponent(
                screenType, prefab, uiRoot, Array.Empty<object>());
        }
    }
}
