using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace LocalPlayer
{
    [RequireComponent(typeof(PlayerCharacterController))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerWallRunController : MonoBehaviour
    {
        [SerializeField] private float wallJumpHeight = 3;
        [SerializeField] private float wallJumpSideForce = 8;
        [SerializeField] private float wallJumpForwardForce = 8;
        [SerializeField] private float wallRunForwardImpulse = 8;
        [SerializeField] private float wallCheckDistance = 0.75f;

        private PlayerCharacterController _playerCharacterController;
        private Rigidbody _rigidbody;

        private bool _isWallRight;
        private bool _isWallLeft;

        private RaycastHit _righthitinfo;
        private RaycastHit _lefthitinfo;
        private bool _wallRunningEnabled = true;

        public bool IsWallRight => _isWallRight;
        public bool IsWallLeft => _isWallLeft;
        public bool IsWallRunning => _isWallRight || _isWallLeft;

        public event UnityAction OnWallJump;

        private void Awake()
        {
            _playerCharacterController = GetComponent<PlayerCharacterController>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            OnWallJump += WallJumpAsync;
        }

        private void Update()
        {
            if (_playerCharacterController.Jumping && (_isWallRight || _isWallLeft)) OnWallJump?.Invoke();

            _playerCharacterController.MovementEnabled = !(_isWallRight || _isWallLeft);
        }

        private void FixedUpdate()
        {
            if (!_wallRunningEnabled) return;

            if (_playerCharacterController.IsGrounded)
            {
                _isWallRight = false;
                _isWallLeft = false;
                return;
            }

            Ray rightRay = new(transform.position, transform.right);
            Ray leftRay = new(transform.position, -transform.right);

            var wasWallRight = _isWallRight;
            var wasWallLeft = _isWallLeft;

            if (!_isWallRight) _isWallRight = Physics.Raycast(rightRay, out _righthitinfo, wallCheckDistance, _playerCharacterController.GroundLayerMask);
            if (!_isWallLeft) _isWallLeft = Physics.Raycast(leftRay, out _lefthitinfo, wallCheckDistance, _playerCharacterController.GroundLayerMask);

            if (_isWallRight && !wasWallRight)
            {
                var forwardForce = -Vector3.Cross(_righthitinfo.normal, transform.up) * wallRunForwardImpulse;

                _rigidbody.AddForce(forwardForce, ForceMode.VelocityChange);
            }

            if (_isWallLeft && !wasWallLeft)
            {
                var forwardForce = Vector3.Cross(_lefthitinfo.normal, transform.up) * wallRunForwardImpulse;

                _rigidbody.AddForce(forwardForce, ForceMode.VelocityChange);
            }

            if (_isWallRight) _rigidbody.AddForce(-_righthitinfo.normal, ForceMode.Acceleration);
            if (_isWallLeft) _rigidbody.AddForce(-_lefthitinfo.normal, ForceMode.Acceleration);
        }

        private async void WallJumpAsync()
        {
            var sideForce = Vector3.zero;
            if (_isWallRight) sideForce = _righthitinfo.normal * wallJumpSideForce;
            if (_isWallLeft) sideForce = _lefthitinfo.normal * wallJumpSideForce;

            var jumpForce = Vector3.up * Mathf.Sqrt(wallJumpHeight * -2 * Physics.gravity.y);

            var forwardForce = transform.forward * wallJumpForwardForce;

            var finalForce = jumpForce + sideForce + forwardForce;

            _wallRunningEnabled = false;

            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);

            if (_isWallRight)
            {
                var velocity = Vector3.Project(_rigidbody.velocity, _righthitinfo.normal);

                _rigidbody.velocity -= velocity;
            }

            if (_isWallLeft)
            {
                var velocity = Vector3.Project(_rigidbody.velocity, _lefthitinfo.normal);

                _rigidbody.velocity -= velocity;
            }

            _isWallRight = false;
            _isWallLeft = false;

            _rigidbody.AddForce(finalForce, ForceMode.VelocityChange);

            _playerCharacterController.Jumping = false;

            await Task.Delay(TimeSpan.FromSeconds(0.4f));
            _wallRunningEnabled = true;
        }
    }
}