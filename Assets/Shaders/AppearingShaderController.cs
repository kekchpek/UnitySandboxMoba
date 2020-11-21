using UnityEngine;

namespace SandboxMoba.Shaders
{
    public class AppearingShaderController : MonoBehaviour
    {
        private const string ORIGIN_POINT_KEY = "_AppearingOriginPoint";
        private const string ORIGIN_ROTATION_KEY = "_AppearingRotationQuaternion";

        [SerializeField] private bool _syncOriginPoint;
        [SerializeField] private bool _syncOriginRotation;
        [SerializeField] private Renderer[] _renderersToControl;

        private Material[] _materialsToControl;
        private Vector3 _previouPos;
        private Quaternion _previousRotation;


        private void Awake()
        {
            _materialsToControl = new Material[_renderersToControl.Length];
            for(int i = 0; i < _renderersToControl.Length; i++)
            {
                _materialsToControl[i] = new Material(_renderersToControl[i].sharedMaterial);
                _renderersToControl[i].sharedMaterial = _materialsToControl[i];
            }
        }

        private void Update()
        {
            if (!_syncOriginPoint && !_syncOriginRotation)
                return;
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            foreach (Material mat in _materialsToControl)
            {

                if (_syncOriginPoint && _previouPos != position)
                {
                    mat.SetVector(ORIGIN_POINT_KEY, position);
                }
                if (_syncOriginRotation && _previousRotation != rotation)
                {
                    mat.SetVector(ORIGIN_ROTATION_KEY, new Vector4(rotation.x, rotation.y, rotation.z, rotation.w));
                }
            }

            _previousRotation = rotation;
            _previouPos = position;
        }
    }
}
