using UnityEngine;

namespace SandboxMoba.Characters
{
    public abstract class AbstractWeaponAnimator : MonoBehaviour
    {

        public abstract void Appear();

        public abstract void Disappear();

        /// <summary>
        /// Appears if it is disappeared and disappear if appeared
        /// </summary>
        public abstract void SwitchState();

    }
}