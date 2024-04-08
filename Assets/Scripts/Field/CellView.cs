using Assets.Scripts.Actions;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Field
{
    public class CellView : MonoBehaviour
    {
        [SerializeField]
        private SelectableObject _selectable;

        [SerializeField]
        private Material _defaultMaterial;

        [SerializeField]
        private Material _highlightMaterial;

        [SerializeField]
        private MeshRenderer _renderer;

        [SerializeField]
        private GameObject _hiddenView;

        [SerializeField]
        private GameObject _revealedView;

        private Entity _entity;
        private IActionFactory _actionFactory;
        private IActionProcessor _actionProcessor;

        public Entity Entity => _entity;

        private void OnDestroy()
        {
            Unsubscribe();
        }

        public void Start()
        {
            Subscribe();
        }

        public void Initialize(Entity entity, IActionFactory actionFactory, IActionProcessor actionProcessor)
        {
            _entity = entity;
            _actionFactory = actionFactory;
            _actionProcessor = actionProcessor;
        }

        private void Subscribe()
        {
            _selectable.OnSelect += Selected;
        }

        private void Unsubscribe()
        {
            _selectable.OnSelect -= Selected;
        }

        public void Selected()
        {
            _actionProcessor.Enqueue(new Queue<IAction>(new[] 
                { 
                    _actionFactory.GetChangePositionAction(),
                    _actionFactory.GetSettlementClickedAction()
                }));
        }

        public void ChangeToHighlightMaterial()
        {
            _renderer.material = _highlightMaterial;
        }

        public void ChangeToDefaultMaterial()
        {
            _renderer.material = _defaultMaterial;
        }

        public void UpdateCellVisibility(bool isVisible)
        {
            _hiddenView.SetActive(!isVisible);
            _revealedView.SetActive(isVisible);
        }
    }
}
