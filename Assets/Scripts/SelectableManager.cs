using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class SelectableManager : MonoBehaviour, ISelectableManager
    {
        private ISelectable _previousSelectable;
        private ISelectable _currentSelectable;
        private InputActions _inputActions;

        public ISelectable PreviousSelectable => _previousSelectable;

        public ISelectable CurrentSelectable => _currentSelectable;

        public event Action SelectionChanged;

        private bool pointerOverUI = false;

        private void Awake()
        {
            _inputActions = new InputActions();
        }

        void Update()
        {
            pointerOverUI = EventSystem.current.IsPointerOverGameObject();
        }

        private void OnEnable()
        {
            Subscribe();
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            Unsubscribe();
            _inputActions.Player.Disable();
        }

        private void Subscribe()
        {
            _inputActions.Player.Select.performed += UpdateSelect;
        }

        private void Unsubscribe()
        {
            if (_inputActions != null)
            {
                _inputActions.Player.Select.performed -= UpdateSelect;
            }
        }

        private void UpdateSelect(InputAction.CallbackContext context)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (pointerOverUI)
            {
                return;
            }

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.TryGetComponent(out ISelectable selectable))
                {
                    _previousSelectable = _currentSelectable;
                    _currentSelectable = selectable;

                    if (_previousSelectable != null)
                    {
                        _previousSelectable.Deselect();
                    }

                    _currentSelectable.Select();

                    SelectionChanged?.Invoke();
                }
            }
        }
    }
}
