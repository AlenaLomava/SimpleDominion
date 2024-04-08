using UnityEngine;

namespace Assets.Scripts
{
    public class AutoPlayParticleSystem : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem _particleSystem;

        private void OnEnable()
        {
            _particleSystem.Play();
        }
    }
}
