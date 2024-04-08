using Assets.Scripts.Actions;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts
{
    public class UnitView : MonoBehaviour
    {
        [SerializeField]
        private SelectableObject _selectable;

        [SerializeField]
        private UnitAnimationController _unitAnimationController;

        private IActionProcessor _actionProcessor;
        private IActionFactory _actionFactory;
        private Entity _entity;
        private float _spaceBetweenCells;

        public Entity Entity => _entity;

        public IUnitAnimationController UnitAnimationController => _unitAnimationController;

        private void OnDestroy()
        {
            Unsubscribe();
        }

        public void Start()
        {
            Subscribe();
        }

        public void Initialize(Entity entity, IActionFactory actionFactory, IActionProcessor actionProcessor, float spaceBetweenCells)
        {
            _entity = entity;
            _actionFactory = actionFactory;
            _actionProcessor = actionProcessor;
            _spaceBetweenCells = spaceBetweenCells;
        }

        public void SetPosition(int row, int column)
        {
            transform.position = new Vector3(
                    row * _spaceBetweenCells,
                    1,
                    column * _spaceBetweenCells);
        }

        public void UpdateVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void Selected()
        {
            var team = UnitEntityHelper.GetOwnershipValue(Entity);

            if (team == Team.Player)
            {
                var showSettlementCaptureButtonAction = _actionFactory.GetShowSettlementCaptureButtonAction();
                var showMovementGridAction = _actionFactory.GetShowMovementGridAction();

                _actionProcessor.Enqueue(
                    new Queue<IAction>(new[] { showSettlementCaptureButtonAction, showMovementGridAction }));
            }
            else if (team == Team.Enemy)
            {
                var takeDamageAction = _actionFactory.GetHandleDamageAction();

                _actionProcessor.Enqueue(
                    new Queue<IAction>(new[] { takeDamageAction }));
            }
        }

        private void Subscribe()
        {
            _selectable.OnSelect += Selected;
        }

        private void Unsubscribe()
        {
            _selectable.OnSelect -= Selected;
        }
    }
}
