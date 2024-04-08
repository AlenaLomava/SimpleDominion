using Assets.Scripts.Components;
using Unity.Collections;
using Unity.Entities;
using Zenject;

namespace Assets.Scripts.Actions
{
    public class NextTurnAction : IAction
    {
        private EntityManager _entityManager;

        [Inject]
        public NextTurnAction()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public bool CanProcess()
        {
            return true;
        }

        public void Clear()
        {
        }

        public void Process()
        {
            var query = _entityManager.CreateEntityQuery(typeof(GameInProgressComponent));

            using (NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp))
            {
                foreach (var entity in entities)
                {
                    _entityManager.SetComponentData(entity, new GameInProgressComponent { IsPlayerTurn = false });
                }
            }
        }
    }

    public class NextTurnActionFactory : PlaceholderFactory<NextTurnAction>
    {
    }
}
