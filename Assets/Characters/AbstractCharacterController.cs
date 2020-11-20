using UnityEngine;

namespace SandboxMoba.Characters
{
    public abstract class AbstractCharacterController : MonoBehaviour
    {
        public abstract void SetSpeedDirection(Vector2 speed);

        /// <summary>
        /// Signal character to rotate
        /// </summary>
        /// <param name="angles">(yaw, pitch)</param>
        public abstract void Rotate(Vector2 angles);

        public abstract void Jump();

    }
}
