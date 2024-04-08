using Assets.Scripts.UI.Context;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public abstract class UIElement : MonoBehaviour
    {
        public abstract void SetContext(IUIElementContext context);
    }
}
