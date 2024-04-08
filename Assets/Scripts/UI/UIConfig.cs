using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [CreateAssetMenu(fileName = nameof(UIConfig), menuName = "SimpleDominion/Configs/" + nameof(UIConfig))]
    public class UIConfig : ScriptableObject
    {
        [SerializeField]
        private UIElement[] _uiElements;

        public IList<UIElement> UIElements => _uiElements;
    }
}
