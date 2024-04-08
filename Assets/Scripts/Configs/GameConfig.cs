using UnityEngine;

namespace Assets.Scripts.Configs
{
    [CreateAssetMenu(fileName = nameof(GameConfig), menuName = "SimpleDominion/Configs/" + nameof(GameConfig))]
    public class GameConfig : ScriptableObject
    {
        [SerializeField]
        private WorldConfig _worldConfig;

        [SerializeField]
        private UnitsConfig _unitsConfig;

        public WorldConfig WorldConfig => _worldConfig;

        public UnitsConfig UnitsConfig => _unitsConfig;
    }
}
