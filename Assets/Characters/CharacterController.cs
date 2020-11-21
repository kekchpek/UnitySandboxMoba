﻿using SandboxMoba.Characters.Sensors;
using System;
using UnityEngine;

namespace SandboxMoba.Characters
{
    public class CharacterController : AbstractCharacterController
    {
        [SerializeField] private AbstractCharacterAnimator _animator;
        [SerializeField] private AbstractCameraController _cameraController;
        [SerializeField] private Sensor _groundSensor;
        [SerializeField] private Rigidbody _rigidbody;

        [SerializeField] private float _jumpForce = 5f;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _acceleration = 1f;

        private Vector2 _currentSpeedDir;
        private Vector2 _targetSpeedDir;
        private bool _jumpSignalReceived;
        private bool _isOnGround;

        public void Construct(AbstractCharacterAnimator abstractCharacterAnimator) 
        {
            this._animator = abstractCharacterAnimator ?? throw new ArgumentNullException(nameof(abstractCharacterAnimator));
        }

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
            Vector2 groundSpeed = calculateGroundSpeed();
            handleJumpSignal(groundSpeed);
            handleIsGrounded(groundSpeed);
            handleSpeed(groundSpeed);
        }

        private void handleSpeed(Vector2 groundSpeed)
        {
            if (_isOnGround)
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
            if (_jumpSignalReceived)
            {
                Vector3 jumpStartSpeed = Vector3.up * _jumpForce;
                _rigidbody.velocity = jumpStartSpeed;
            }
        }

        private void handleIsGrounded(Vector2 groundSpeed)
        {
            if (_groundSensor.IsObjectDetected != _isOnGround)
            {
                // character got off from the ground
                if (_isOnGround)
                {
                    _rigidbody.velocity = new Vector3(groundSpeed.x, _rigidbody.velocity.y, groundSpeed.y);
                    // if char has jumped
                    _jumpSignalReceived = false;
                    _animator.SetIsJumping(false);
                }
                else // character grounded
                {

                }
                _isOnGround = _groundSensor.IsObjectDetected;
                _animator.SetInAir(!_isOnGround);
            }
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
                _jumpSignalReceived = true;
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
    }
}