using Assets.Scripts.Actions;
using Assets.Scripts.UI.Context;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI
{
    public class SettlementCaptureButton : UIElement
    {
        [SerializeField]
        private Button _button;

        [Inject]
        private IActionFactory _actionFactory;

        [Inject]
        private IActionProcessor _actionProcessor;

        public override void SetContext(IUIElementContext context)
        {
        }

        private void OnEnable()
        {
            Subscribe();
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
            _actionProcessor.Enqueue(new Queue<IAction>(new[] { _actionFactory.GetSettlementCaptureAction() }));
        }
    }
}
