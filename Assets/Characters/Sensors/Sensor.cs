using SandboxMoba.Auxiliary;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SandboxMoba.Characters.Sensors
{
    public class Sensor : MonoBehaviour
    {

        [SerializeField] private int[] _layersToDetect;

        protected readonly ISet<GameObject> _detectedObjects = new HashSet<GameObject>();

        public bool IsObjectDetected => _detectedObjects.Count > 0;

        private void OnTriggerEnter(Collider other)
        {
            GameObject obj = other.gameObject;
            if (_layersToDetect.Any(layer => layer == obj.layer))
            {
                Debug.Assert(!_detectedObjects.Contains(obj));
                _detectedObjects.Add(obj);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            GameObject obj = other.gameObject;
            if (_layersToDetect.Any(layer => layer == obj.layer))
            {
                Debug.Assert(_detectedObjects.Contains(obj));
                _detectedObjects.Remove(obj);
            }
        }

    }
}
