using System;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    [Serializable]
    public class UnitsConfig
    {
        [SerializeField]
        private UnitView _playerUnitPrefab;

        [SerializeField]
        private UnitView _enemyUnitPrefab;

        [SerializeField]
        private int _walkDistance;

        [SerializeField]
        private int _health;

        [SerializeField]
        private int _actionsCount;

        [SerializeField]
        private int _visibilityRange;

        public UnitView PlayerUnitPrefab => _playerUnitPrefab;

        public UnitView EnemyUnitPrefab => _enemyUnitPrefab;

        public int WalkDistance => _walkDistance;

        public int Health => _health;

        public int ActionsCount => _actionsCount;

        public int VisibilityRange => _visibilityRange;
    }
}
