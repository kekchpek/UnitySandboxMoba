using UnityEngine;

namespace SandboxMoba.Characters
{
    public abstract class AbstractCharacterAnimator : MonoBehaviour
    {
        public abstract void SetSpeed(Vector2 speed);

        public abstract void SetInAir(bool isInAir);

        public abstract void SetIsJumping(bool isJumping);

        public abstract void ChangeMode(CharacterMode newMode);

    }

}
