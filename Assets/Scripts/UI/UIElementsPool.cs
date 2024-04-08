using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.UI
{
    public class UIElementsPool
    {
        private Dictionary<string, Stack<UIElement>> _pool = new Dictionary<string, Stack<UIElement>>();

        public TElement Get<TElement>(TElement prefab, Transform parent, DiContainer container) where TElement : UIElement
        {
            var key = prefab.GetType().Name;
            if (!_pool.ContainsKey(key))
            {
                _pool[key] = new Stack<UIElement>();
            }

            if (_pool[key].Count > 0)
            {
                var element = _pool[key].Pop();
                element.transform.SetParent(parent, false);
                element.gameObject.SetActive(true);
                return (TElement)element;
            }
            else
            {
                var instance = container.InstantiatePrefabForComponent<TElement>(prefab, parent);
                return instance;
            }
        }

        public void ReturnToPool<TElement>(TElement element) where TElement : UIElement
        {
            element.gameObject.SetActive(false);
            var key = element.GetType().Name;
            if (!_pool.ContainsKey(key))
            {
                _pool[key] = new Stack<UIElement>();
            }
            _pool[key].Push(element);
        }
    }
}
