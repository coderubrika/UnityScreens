using System;
using UnityEngine;
using Zenject;

namespace Suburb.Screens
{
    public class LayoutsFactory
    {
        private readonly DiContainer container;
        private readonly string layoutsRootPath;

        public LayoutsFactory(DiContainer container, string layoutsRootPath)
        {
            this.container = container;
            this.layoutsRootPath = layoutsRootPath;
        }

        public TLayout Create<TLayout>(Transform mount)
        {
            Type layoutType = typeof(TLayout);
            string resourcePath = $"{layoutsRootPath}/{layoutType.Name}";
            var prefab = Resources.Load(resourcePath);

            return (TLayout)container.InstantiatePrefabForComponent(
                layoutType, prefab, mount, Array.Empty<object>());
        }
    }
}
