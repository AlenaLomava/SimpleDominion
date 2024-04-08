using System;
using UnityEngine;

namespace Assets.Scripts
{
    public interface ISelectable
    {
        event Action OnSelect;
        event Action OnDeselect;

        bool IsSelected { get; }

        GameObject GameObject { get; }

        void Deselect();

        void Select();
    }
}
