using Assets.Scripts.Actions;
using Assets.Scripts.UI.Context;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI
{
    public class HireButton : UIElement
    {
        [SerializeField]
        private GameObject _limitText;

        [SerializeField]
        private TextMeshProUGUI _hireText;

        [SerializeField]
        private Button _button;

        private Color _defaultColor = Color.black;
        private Color _disabledColor = Color.gray;

        private bool _canHire;

        [Inject]
        private IActionFactory _actionFactory;

        [Inject]
        private IActionProcessor _actionProcessor;

        public override void SetContext(IUIElementContext context)
        {
            if (context is HireButtonContext hireButtonContext)
            {
                _canHire = hireButtonContext.CanHire;
            }

            Show();
        }

        private void Show()
        {
            Subscribe();
            _limitText.SetActive(!_canHire);
            _hireText.color = _canHire ? _defaultColor : _disabledColor;
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _button.onClick.AddListener(ButtonClicked);
        }

        private void Unsubscribe()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void ButtonClicked()
        {
            _actionProcessor.Enqueue(new Queue<IAction>(new[] { _actionFactory.GetHireAction() }));
        }
    }
}
