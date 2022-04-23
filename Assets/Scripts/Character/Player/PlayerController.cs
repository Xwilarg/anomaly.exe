using MiniJameGam9.SO;
using UnityEngine;
using UnityEngine.InputSystem;
using MiniJameGam9.UI;
using MiniJameGam9.Weapon;

namespace MiniJameGam9.Character.Player
{
    public class PlayerController : ACharacterController
    {
        [SerializeField]
        private PlayerInfo _info;

        [SerializeField]
        private GameObject _chain;

        private CharacterController _cc;
        private Vector3 _mov;
        private bool _isSprinting;
        private float _verticalSpeed;
        private Camera _cam;
        private Vector2 _mousePos;

        private void Start()
        {
            Init();
            _cc = GetComponent<CharacterController>();
            _cam = Camera.main;
            _cam.transform.position = transform.position + transform.up * 20f + -transform.forward * 10f;
            _cam.GetComponent<CameraManager>().ToFollow = transform;
            UpdateUI();
        }

        private void FixedUpdate()
        {
            Vector3 desiredMove = new(_mov.x, 0f, _mov.y);

            // Get a normal for the surface that is being touched to move along it
            Physics.SphereCast(transform.position, _cc.radius, Vector3.down, out RaycastHit hitInfo,
                               _cc.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            Vector3 moveDir = Vector3.zero;
            moveDir.x = desiredMove.x * _info.ForceMultiplier * (_isSprinting ? _info.SpeedRunningMultiplicator : 1f);
            moveDir.z = desiredMove.z * _info.ForceMultiplier * (_isSprinting ? _info.SpeedRunningMultiplicator : 1f);

            if (_cc.isGrounded && _verticalSpeed < 0f) // We are on the ground and not jumping
            {
                moveDir.y = -.1f; // Stick to the ground
                _verticalSpeed = -_info.GravityMultiplicator;
            }
            else
            {
                // We are currently jumping, reduce our jump velocity by gravity and apply it
                _verticalSpeed += Physics.gravity.y * _info.GravityMultiplicator;
                moveDir.y += _verticalSpeed;
            }

            _cc.Move(moveDir);

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(_mousePos.x, _mousePos.y, _cam.transform.position.y));
            Vector3 forward = mouseWorld - transform.position;
            var rot = Quaternion.LookRotation(forward, Vector3.up);
            transform.rotation = Quaternion.Euler(0f, rot.eulerAngles.y, 0f);
        }

        private void UpdateUI()
        {
            UIManager.Instance.AmmoDisplay.text = $"{_projectilesInMagazine}";
        }

        public override bool Shoot()
        {
            var result = base.Shoot();
            if (result)
            {
                UpdateUI();
                _cam.GetComponent<CameraManager>().Launch(.1f, CurrentWeapon.ShakeIntensity);
            }
            return result;
        }

        public void ThrowChain()
        {
            var go = Instantiate(_chain, transform.position + transform.forward, transform.rotation);
            go.GetComponent<Chain>().Caster = transform;
        }

        protected override void OnReloadEnd()
        {
            base.OnReloadEnd();
            UpdateUI();
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            _mov = value.ReadValue<Vector2>().normalized;
        }

        public void OnSprint(InputAction.CallbackContext value)
        {
            _isSprinting = value.ReadValueAsButton();
        }

        public void OnLook(InputAction.CallbackContext value)
        {
            _mousePos = value.ReadValue<Vector2>();
        }

        public void OnShoot(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                Shoot();
            }
        }

        public void OnChain(InputAction.CallbackContext value)
        {
            if (value.performed)
                ThrowChain();
        }
    }

}