using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float borderSize = 10f;

        private Camera _mainCamera;
        private Vector2 _screenCenter;

        void Start()
        {
            _mainCamera = Camera.main;
            _screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        }

        void Update()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if (mousePosition.x < borderSize || mousePosition.x > Screen.width - borderSize ||
                mousePosition.y < borderSize || mousePosition.y > Screen.height - borderSize)
            {
                MoveCamera(mousePosition);
            }
        }

        private void MoveCamera(Vector2 mousePosition)
        {
            Vector2 direction = (mousePosition - _screenCenter).normalized;

            Vector3 moveDirection = new Vector3(direction.x, direction.y, 0f);

            float cameraSize = _mainCamera.orthographicSize;
            float cameraRatio = _mainCamera.aspect;

            moveDirection.x *= cameraSize * cameraRatio;
            moveDirection.y *= cameraSize;
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}
