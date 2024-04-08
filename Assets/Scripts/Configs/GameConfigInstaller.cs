using UnityEngine;
using Zenject;

namespace Assets.Scripts.Configs
{
    [CreateAssetMenu(fileName = nameof(GameConfigInstaller), menuName = "SimpleDominion/Installers/" + nameof(GameConfigInstaller))]
    public class GameConfigInstaller : ScriptableObjectInstaller<GameConfigInstaller>
    {
        [SerializeField]
        private GameConfig gameConfig;

        public override void InstallBindings()
        {
            Container.BindInstance(gameConfig);
        }
    }
}
