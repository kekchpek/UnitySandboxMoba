using System;
using UnityEngine;

namespace SandboxMoba.Characters
{
    public class CharacterAnimator : AbstractCharacterAnimator
    {

        private const string SPEED_VERTICAL_KEY = "vertical_speed";
        private const string SPEED_HORIZONTAL_KEY = "horizontal_speed";
        private const string IS_IN_AIR_KEY = "is_in_air";
        private const string IS_JUMPING_KEY = "is_jumping";

        [SerializeField] private Animator _animator;

        public void Construct(Animator animator)
        {
            _animator = animator ?? throw new ArgumentNullException(nameof(animator));
        }

        public override void SetInAir(bool isInAir)
        {
            _animator.SetBool(IS_IN_AIR_KEY, isInAir);
        }

        public override void SetIsJumping(bool isJumping)
        {
            _animator.SetBool(IS_JUMPING_KEY, isJumping);
        }

        public override void SetSpeed(Vector2 speed)
        {
            _animator.SetFloat(SPEED_VERTICAL_KEY, speed.y);
            _animator.SetFloat(SPEED_HORIZONTAL_KEY, speed.x);
        }
    }
}
