using UnityEngine;

namespace Assets.Scripts
{
    public class UnitAnimationController : MonoBehaviour, IUnitAnimationController
    {
        [SerializeField]
        private Animator _animator;

        private const string ATTACK_PARAM_NAME = "Attack";
        private const string RESET_PARAM_NAME = "Reset";

        private static readonly int AttackHash = Animator.StringToHash(ATTACK_PARAM_NAME);
        private static readonly int ResetHash = Animator.StringToHash(RESET_PARAM_NAME);

        public void RunAttackAnim()
        {
            _animator.SetTrigger(AttackHash);
        }

        public void Reset()
        {
            _animator.SetTrigger(ResetHash);
        }
    }
}
