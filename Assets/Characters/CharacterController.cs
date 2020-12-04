using SandboxMoba.Characters.Sensors;
using System;
using System.Collections;
using UnityEngine;

namespace SandboxMoba.Characters
{
    public class CharacterController : AbstractCharacterController
    {
        [SerializeField] private AbstractCharacterAnimator _animator;
        [SerializeField] private AbstractCameraController _cameraController;
        [SerializeField] private Sensor _groundSensor;
        [SerializeField] private Sensor _ceilingSensor;
        [SerializeField] private Rigidbody _rigidbody;

        [SerializeField] private float _jumpForce = 5f;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _acceleration = 1f;
        [SerializeField] private float _changeModeTimeSec = 1f;

        private Vector2 _currentSpeedDir;
        private Vector2 _targetSpeedDir;
        private bool _isJumping;
        private bool _isOnGround;
        private bool _isCeiled;
        private CharacterMode _mode = CharacterMode.SIMPLE;

        public override void Rotate(Vector2 angles)
        {
            transform.Rotate(new Vector3(0, angles.x, 0));
            _cameraController.AddBearing(angles.y);
        }

        public override void SetSpeedDirection(Vector2 speedDir)
        {
            _targetSpeedDir = speedDir;
        }

        private void FixedUpdate()
        {
            handleCeiled();
            Vector2 groundSpeed = calculateGroundSpeed();
            handleJumpSignal(groundSpeed);
            handleIsGrounded(groundSpeed);
            handleSpeed(groundSpeed);
        }

        private void handleCeiled()
        {
            if (_ceilingSensor.IsObjectDetected != _isCeiled)
            {
                _isCeiled = _ceilingSensor.IsObjectDetected;
            }
        }

        private void handleSpeed(Vector2 groundSpeed)
        {
            if (_isOnGround && _mode != CharacterMode.CHANGING_MODE)
            {
                Vector2 accelerationVector = _targetSpeedDir - _currentSpeedDir;
                float momentAcceleration = _acceleration * Time.fixedDeltaTime;
                if (accelerationVector.magnitude <= momentAcceleration)
                {
                    _currentSpeedDir = _targetSpeedDir;
                }
                else
                {
                    _currentSpeedDir += accelerationVector.normalized * momentAcceleration;
                }
                _rigidbody.velocity = new Vector3(groundSpeed.x, _rigidbody.velocity.y, groundSpeed.y);
            }
        }

        private void handleJumpSignal(Vector2 groundSpeed)
        {
            if (_isJumping)
            {
                if (!_isCeiled)
                {
                    Vector3 jumpStartSpeed = Vector3.up * _jumpForce;
                    _rigidbody.velocity = jumpStartSpeed;
                }
                else
                {
                    stopJumping();
                }
            }
        }

        private void handleIsGrounded(Vector2 groundSpeed)
        {
            if (_groundSensor.IsObjectDetected != _isOnGround)
            {
                // character got off from the ground
                if (_isOnGround)
                {
                    stopJumping();
                    _rigidbody.velocity = new Vector3(groundSpeed.x, _rigidbody.velocity.y, groundSpeed.y);
                }
                else // character grounded
                {

                }
                _isOnGround = _groundSensor.IsObjectDetected;
                _animator.SetInAir(!_isOnGround);
            }
        }

        private void stopJumping()
        {
            _isJumping = false;
            _animator.SetIsJumping(false);
        }

        private void Update()
        {
            if (_isOnGround)
                _animator.SetSpeed(_currentSpeedDir);
            else
            {
                _animator.SetSpeed(Vector3.zero);
            }
        }

        public override void Jump()
        {
            if (_isOnGround)
            {
                _isJumping = true;
                _animator.SetIsJumping(true);
            }
        }

        private Vector2 calculateGroundSpeed()
        {
            Vector2 resultSpeed = _speed * _currentSpeedDir;
            Vector2 characterDir = new Vector2(transform.forward.x, transform.forward.z);
            float angle = Vector2.Angle(Vector2.up, characterDir);
            angle = Mathf.PI * angle / 180f * -Mathf.Sign(characterDir.x);
            float tmpX = resultSpeed.x;
            resultSpeed.x = resultSpeed.x * Mathf.Cos(angle) - resultSpeed.y * Mathf.Sin(angle);
            resultSpeed.y = tmpX * Mathf.Sin(angle) + resultSpeed.y * Mathf.Cos(angle);
            return resultSpeed;
        }

        public override void SwitchMode()
        {
            if (_mode == CharacterMode.CHANGING_MODE)
                return;
            this.StartCoroutine(switchModeCoroutine());
        }

        private IEnumerator switchModeCoroutine()
        {
            CharacterMode modeToSet = _mode == CharacterMode.SIMPLE ? CharacterMode.BATTLE : CharacterMode.SIMPLE;
            _mode = CharacterMode.CHANGING_MODE;
            _animator.ChangeMode(CharacterMode.CHANGING_MODE);
            yield return new WaitForSeconds(_changeModeTimeSec);
            _mode = modeToSet;
            _animator.ChangeMode(modeToSet);
        }
    }
}
