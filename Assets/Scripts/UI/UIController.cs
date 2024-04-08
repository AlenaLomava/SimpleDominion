using Assets.Scripts.UI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Assets.Scripts.UI
{
    public class UIController : IUIController
    {
        private readonly UIConfig _uiConfig;
        private readonly UIRoot _uiRoot;
        private Dictionary<Type, UIElement> _elements = new Dictionary<Type, UIElement>();
        private UIElementsPool _elementsPool = new UIElementsPool();
        private DiContainer _container;

        public UIController(UIConfig uiConfig, UIRoot uiRoot, DiContainer container)
        {
            _uiConfig = uiConfig;
            _uiRoot = uiRoot;
            _container = container;
        }

        public TElement Show<TElement>() where TElement : UIElement
        {
            var prefab = _uiConfig.UIElements.OfType<TElement>().FirstOrDefault();
            if (prefab != null)
            {
                var instance = _elementsPool.Get(prefab, _uiRoot.transform, _container);
                var type = typeof(TElement);
                if (!_elements.ContainsKey(type))
                {
                    _elements[type] = instance;
                }
                return instance;
            }

            throw new InvalidOperationException($"No UI element of type {typeof(TElement)} found in installer.");
        }

        public TElement Show<TElement>(IUIElementContext context) where TElement : UIElement
        {
            var elementPrefab = _uiConfig.UIElements.OfType<TElement>().FirstOrDefault();
            if (elementPrefab != null)
            {
                var instance = _elementsPool.Get(elementPrefab, _uiRoot.transform, _container);
                instance.SetContext(context);

                var type = typeof(TElement);
                if (!_elements.ContainsKey(type))
                {
                    _elements[type] = instance;
                }
                return instance;
            }

            throw new InvalidOperationException($"No element of type {typeof(TElement)} found in installer");
        }

        public void Hide<TElement>() where TElement : UIElement
        {
            var type = typeof(TElement);
            if (_elements.ContainsKey(type))
            {
                var element = _elements[type];
                _elements.Remove(type);
                _elementsPool.ReturnToPool(element);
            }
        }

        public void HideAll()
        {
            foreach (var pair in _elements)
            {
                _elementsPool.ReturnToPool(pair.Value);
            }
            _elements.Clear();
        }
    }
}
