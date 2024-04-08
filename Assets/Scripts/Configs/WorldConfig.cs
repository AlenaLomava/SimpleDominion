using Assets.Scripts.Field;
using System;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    [Serializable]
    public class WorldConfig
    {
        [SerializeField]
        private FieldPresetConfig[] _fieldPresets;

        [SerializeField]
        private float _spaceBetween;

        [SerializeField]
        private CellView _grassPrefab;

        [SerializeField]
        private CellView _waterPrefab;

        [SerializeField]
        private CellView _mountPrefab;

        [SerializeField]
        private CellView _settlementPrefab;

        public CellView GrassPrefab => _grassPrefab;

        public CellView WaterPrefab => _waterPrefab;

        public CellView MountPrefab => _mountPrefab;

        public CellView SettlementPrefab => _settlementPrefab;

        public float SpaceBetween => _spaceBetween;

        public FieldPresetConfig[] FieldPresets => _fieldPresets;
    }
}
