using Assets.Scripts.Actions;
using Assets.Scripts.Components;
using Assets.Scripts.Components.Config;
using Assets.Scripts.Components.GameFlow;
using Assets.Scripts.Configs;
using Assets.Scripts.UI;
using Assets.Scripts.UI.Context;
using Unity.Entities;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    public class Main : MonoBehaviour
    {
        [Inject]
        private GameConfig _gameConfig;

        [Inject]
        private IUIController _uiController;

        [Inject]
        private IActionFactory _actionFactory;

        [Inject]
        private IActionProcessor _actionProcessor;

        [SerializeField]
        private GameObject _gridRoot;

        public void Start()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            CellEntityHelper.Initialize(entityManager);
            UnitEntityHelper.Initialize(entityManager);
            UnitEntityFactory.Initialize(_gameConfig, entityManager);

            CreateConfigEntities(entityManager, _gameConfig);
            
            CreateGridRootEntity(entityManager);

            var gameFlowEntity = entityManager.CreateSingleton<GameSetupComponent>();
            entityManager.AddComponent<GameSetupComponent>(gameFlowEntity);

            var actionsSystemEntity = entityManager.CreateSingleton<ActionsSystemComponent>();
            entityManager.SetComponentData(actionsSystemEntity, new ActionsSystemComponent 
            { 
                ActionFactory = _actionFactory, 
                ActionProcessor = _actionProcessor 
            });

            var uiControllerEntity = entityManager.CreateSingleton<UIControllerComponent>();
            entityManager.SetComponentData(uiControllerEntity, new UIControllerComponent
            {
                UIController = _uiController
            });

            _uiController.Show<NewGameScreen>(new NewGameScreenContext() { IsFirstLoad = true });
        }

        private void CreateConfigEntities(EntityManager entityManager, GameConfig gameConfig)
        {
            var worldConfigEntity = entityManager.CreateSingleton<WorldConfigComponent>();
            entityManager.SetComponentData(worldConfigEntity,
                new WorldConfigComponent
                {
                    WorldConfig = gameConfig.WorldConfig
                });

            var unitsConfigEntity = entityManager.CreateSingleton<UnitsConfigComponent>();
            entityManager.SetComponentData(unitsConfigEntity,
                new UnitsConfigComponent
                {
                    WalkDistance = gameConfig.UnitsConfig.WalkDistance,
                    Health = gameConfig.UnitsConfig.Health,
                    ActionsCount = gameConfig.UnitsConfig.ActionsCount,
                    VisibilityRange = gameConfig.UnitsConfig.VisibilityRange,
                    PlayerUnitPrefab = gameConfig.UnitsConfig.PlayerUnitPrefab,
                    EnemyUnitPrefab = gameConfig.UnitsConfig.EnemyUnitPrefab
                });
        }

        private void CreateGridRootEntity(EntityManager entityManager)
        {
            var gridRootEntity = entityManager.CreateSingleton<GridRootComponent>();
            entityManager.SetComponentData(gridRootEntity, new GridRootComponent { GridRoot = _gridRoot });
        }
    }
}
