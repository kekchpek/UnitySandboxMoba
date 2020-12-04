using SandboxMoba.Characters;
using UnityEngine;

namespace SandboxMoba.Input
{

    [SerializeField] 

    public class CharacterInputController : AbstractInputController
    {
        private const string MOUSE_X = "Mouse X";
        private const string MOUSE_Y = "Mouse Y";

        [SerializeField] private AbstractCharacterController _characterController;
        [SerializeField] private float _characterRotationYawModifier = 5f;
        [SerializeField] private float _characterRotationPitchModifier = 0f;
        [SerializeField] private bool _inversePitchRotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            applySpeed();
            applyJump();
            applyCameraMoving();
            applySwitchMode();
        }

        private void applySpeed()
        {
            Vector2 speedDirection = Vector2.zero;
            if (UnityEngine.Input.GetKey("w"))
            {
                speedDirection += Vector2.up;
            }
            if (UnityEngine.Input.GetKey("s"))
            {
                speedDirection += Vector2.down;
            }
            if (UnityEngine.Input.GetKey("a"))
            {
                speedDirection += Vector2.left;
            }
            if (UnityEngine.Input.GetKey("d"))
            {
                speedDirection += Vector2.right;
            }
            _characterController.SetSpeedDirection(speedDirection.normalized);
        }

        private void applyJump()
        {
            if (UnityEngine.Input.GetKeyDown("space"))
            {
                _characterController.Jump();
            }
        }

        private void applyCameraMoving()
        {
            Vector2 delta = new Vector2(UnityEngine.Input.GetAxis(MOUSE_X), UnityEngine.Input.GetAxis(MOUSE_Y));
            delta.x *= _characterRotationYawModifier;
            delta.y *= _inversePitchRotation ? _characterRotationPitchModifier : -_characterRotationPitchModifier;
            _characterController.Rotate(delta);
        }

        private void applySwitchMode()
        {
            if (UnityEngine.Input.GetKeyDown("e"))
            {
                _characterController.SwitchMode();
            }
        }

    }
}
