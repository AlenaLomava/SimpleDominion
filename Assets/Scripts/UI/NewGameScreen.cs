using Assets.Scripts.Components;
using Assets.Scripts.Components.GameFlow;
using Assets.Scripts.UI.Context;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class NewGameScreen : UIElement
    {
        [SerializeField]
        private Button[] _worldSizeButtons;

        [SerializeField]
        private GameObject _winText;

        [SerializeField]
        private GameObject _loseText;

        private bool _isPlayerWin;
        private bool _isFirstLoad;

        public override void SetContext(IUIElementContext context)
        {
            if (context is NewGameScreenContext newGameScreenContext)
            {
                _isFirstLoad = newGameScreenContext.IsFirstLoad;
                _isPlayerWin = newGameScreenContext.IsPlayerWin;
            }

            if (!_isFirstLoad)
            {
                ShowEndGameText();
            }
        }

        private void ShowEndGameText()
        {
            _winText.SetActive(_isPlayerWin);
            _loseText.SetActive(!_isPlayerWin);
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
            for (int i = 0; i < _worldSizeButtons.Length; i++)
            {
                int worldI = i;
                _worldSizeButtons[i].onClick.AddListener(() => StartGame(worldI));
            }
        }

        private void Unsubscribe()
        {
            foreach (var button in _worldSizeButtons)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        private void StartGame(int worldIndex)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = entityManager.CreateEntityQuery(typeof(GameSetupComponent));

            using (NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp))
            {
                foreach (var entity in entities)
                {
                    entityManager.RemoveComponent<GameSetupComponent>(entity);
                    entityManager.AddComponentData(entity, new CreatingWorldComponent { WorldIndex = worldIndex });
                }
            }
        }
    }
}
