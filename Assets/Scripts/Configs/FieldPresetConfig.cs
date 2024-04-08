using System;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    [Serializable]
    public class FieldPresetConfig
    {
        [SerializeField]
        private int _size;

        [SerializeField]
        private int _mountCellsNumber;

        [SerializeField]
        private int _waterCellsNumber;

        [SerializeField]
        private int _settlementsNumber;

        public int Size => _size;

        public int MountCellsNumber => _mountCellsNumber;

        public int WaterCellsNumber => _waterCellsNumber;

        public int SettlementsNumber => _settlementsNumber;
    }
}
