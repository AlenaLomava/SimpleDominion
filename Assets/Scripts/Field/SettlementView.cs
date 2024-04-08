using UnityEngine;

namespace Assets.Scripts.Field
{
    public class SettlementView : MonoBehaviour
    {
        [SerializeField]
        private SkinnedMeshRenderer _flag;

        [SerializeField]
        private SkinnedMeshRenderer[] _banners;

        [SerializeField]
        private Material[] _teamPlayerMaterials;

        [SerializeField]
        private Material[] _teamEnemyMaterials;

        private void Start()
        {
            ChangeOwner(Team.Neutral);
        }

        public void ChangeOwner(Team team)
        {
            switch (team)
            {
                case Team.Neutral:
                    _flag.enabled = false;
                    foreach(var banner in _banners)
                    {
                        banner.enabled = false;
                    }
                    break;
                case Team.Player: 
                    _flag.enabled = true;
                    _flag.material = _teamPlayerMaterials[0];
                    foreach (var banner in _banners)
                    {
                        banner.enabled = true;
                        banner.material = _teamPlayerMaterials[0];
                    }
                    break;
                case Team.Enemy:
                    _flag.enabled = true;
                    _flag.material = _teamEnemyMaterials[0];
                    foreach (var banner in _banners)
                    {
                        banner.enabled = true;
                        banner.material = _teamEnemyMaterials[0];
                    }
                    break;
                default:
                    _flag.enabled = false;
                    foreach (var banner in _banners)
                    {
                        banner.enabled = false;
                    }
                    break;

            }
        }
    }
}
