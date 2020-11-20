using SandboxMoba.Auxiliary;
using UnityEngine;

namespace SandboxMoba.Characters.Sensors
{
    public class GroundSensor : MonoBehaviour
    {

        int _collidedEnvironmentObjectsCount;

        public bool IsGrounded => _collidedEnvironmentObjectsCount > 0;


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == Layers.ENVIRONMENT)
            {
                _collidedEnvironmentObjectsCount++;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == Layers.ENVIRONMENT)
            {
                _collidedEnvironmentObjectsCount--;
            }
            Debug.Assert(_collidedEnvironmentObjectsCount >= 0);
        }

    }
}
