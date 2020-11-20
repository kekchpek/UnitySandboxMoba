using SandboxMoba.Auxiliary;
using UnityEngine;

namespace SandboxMoba.Characters
{
    public class CameraController : AbstractCameraController
    {
        [SerializeField] private float _distance;
        private float _bearingRad;
        [SerializeField] private float _maxBearing;
        [SerializeField] private float _minBearing;
        [SerializeField] private Camera _camera;

        public override void AddBearing(float angle)
        {
            _bearingRad += angle * Mathf.PI / 180f;
            _bearingRad = Mathf.Clamp(_bearingRad, Mathf.Deg2Rad * _minBearing, Mathf.Deg2Rad * _maxBearing);
        }

        private void Update()
        {
            Vector3 goodCameraPosition = -Vector3.forward * _distance;

            goodCameraPosition = Quaternion.Euler(_bearingRad * Mathf.Rad2Deg, 0, 0) * goodCameraPosition;
            goodCameraPosition = Quaternion.Euler(0, transform.eulerAngles.y, 0) * goodCameraPosition;
            Ray ray = new Ray(transform.position, goodCameraPosition);
            Debug.DrawRay(transform.position, goodCameraPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, goodCameraPosition.magnitude, ~Layers.CHARACTER))
            {
                _camera.transform.position = hit.point;
            }
            else
            {
                _camera.transform.position = transform.position + goodCameraPosition;
            }
            _camera.transform.localEulerAngles = new Vector3(_bearingRad * 180f / Mathf.PI, 0, 0);
        }
    }
}
