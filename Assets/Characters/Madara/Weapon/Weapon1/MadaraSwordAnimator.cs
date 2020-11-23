using SandboxMoba.Shaders;
using UnityEngine;


namespace SandboxMoba.Characters
{
    public class MadaraSwordAnimator : AbstractWeaponAnimator
    {

        [SerializeField] private float _appearingSpeed = 1f;
        [SerializeField] private float _appearingParticlesSpace = 1f;
        [SerializeField] private AppearingShaderController _appearingController;
        [SerializeField] private ParticleSystem _appearingParticles;
        [SerializeField] private ParticleSystem _disappearingParticles;

        private float _appearingValue;
        private float _disappearingValue;

        private bool _isAppearing;
        private bool _isDisappearing;

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown("q"))
            {
                Appear();
            }
            if (UnityEngine.Input.GetKeyDown("e"))
            {
                Disappear();
            }

            if (_isAppearing)
            {
                if (_appearingValue > 1f)
                {
                    _appearingValue = 1f;
                    _isAppearing = false;
                    _appearingController.IsAppearing = false;
                }
                if (_appearingValue + _appearingParticlesSpace / _appearingController.BorderRage > 1f)
                    _appearingParticles.Stop();
                _appearingValue += Time.deltaTime * _appearingSpeed;
                _appearingController.MaxBorder = _appearingValue;
                _appearingParticles.transform.localPosition = Vector3.up * (_appearingValue * _appearingController.BorderRage + _appearingParticlesSpace);
            }
            if (_isDisappearing)
            {
                if (_disappearingValue > 1f)
                {
                    _disappearingValue = 1f;
                    _isDisappearing = false;
                    _appearingController.IsAppearing = false;
                    _appearingController.IsDisappeared = true;
                    _disappearingParticles.Stop();
                }
                _disappearingValue += Time.deltaTime * _appearingSpeed;
                _appearingController.MaxBorder = 1f - _disappearingValue;
                _disappearingParticles.transform.localPosition = Vector3.up * _appearingController.MaxBorderCoordinate;
            }
        }

        public override void Appear()
        {
            _isDisappearing = false;
            _isAppearing = true;
            _appearingController.IsDisappeared = false;
            _appearingParticles.Play();
            _disappearingParticles.Stop();
            _appearingController.MaxBorder = 0;
            _appearingController.MinBorder = 0;
            _appearingController.IsAppearing = true;
            _appearingParticles.transform.position = Vector3.zero;
            _appearingValue = -(_appearingParticlesSpace / _appearingController.BorderRage);
        }

        public override void Disappear()
        {
            _isDisappearing = true;
            _isAppearing = false;
            _appearingController.IsDisappeared = false;
            _appearingParticles.Stop();
            _disappearingParticles.Play();
            _appearingController.MaxBorder = 1;
            _appearingController.MinBorder = 0;
            _appearingController.IsAppearing = true;
            _disappearingParticles.transform.position = Vector3.zero;
            _disappearingValue = 0f;
        }
    }
}
