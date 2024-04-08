using System;
using UnityEngine;

namespace Assets.Scripts
{
    internal class SelectableObject : MonoBehaviour, ISelectable
    {
        private bool isSelected = false;

        public event Action OnSelect;
        public event Action OnDeselect;

        public bool IsSelected => isSelected;

        public GameObject GameObject => gameObject;

        public void Select()
        {
            isSelected = true;
            OnSelect?.Invoke();
        }

        public void Deselect()
        {
            isSelected = false;
            OnDeselect?.Invoke();
        }
    }
}
