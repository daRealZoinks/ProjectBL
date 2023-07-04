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
        [SerializeField] private float wallCheckDistance = .56f;

        private PlayerCharacterController _playerCharacterController;
        private Rigidbody _rigidbody;

        private bool _isWallRight;
        private bool _isWallLeft;

        private RaycastHit _righthitinfo;
        private RaycastHit _lefthitinfo;

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
            OnWallJump += WallJump;
        }

        private void Update()
        {
            if (_playerCharacterController.Jumping && (_isWallRight || _isWallLeft)) OnWallJump?.Invoke();

            _playerCharacterController.MovementEnabled = !(_isWallRight || _isWallLeft);
        }

        private void FixedUpdate()
        {
            if (_playerCharacterController.IsGrounded)
            {
                _isWallRight = false;
                _isWallLeft = false;
                return;
            }

            Ray rightRay = new(transform.position, transform.right);
            Ray leftRay = new(transform.position, -transform.right);

            // check if you have a wall in ur right or left
            _isWallRight = Physics.Raycast(rightRay, out _righthitinfo, wallCheckDistance, _playerCharacterController.GroundLayerMask);
            _isWallLeft = Physics.Raycast(leftRay, out _lefthitinfo, wallCheckDistance, _playerCharacterController.GroundLayerMask);

            // if you have a wall on your right
            if (_isWallRight)
            {
                // apply force to the right to stick the player to the wall
                _rigidbody.AddForce(-_righthitinfo.normal, ForceMode.Acceleration);
            }

            // if you have a wall on your left
            if (_isWallLeft)
            {
                // apply force to the left to stick the player to the wall
                _rigidbody.AddForce(-_lefthitinfo.normal, ForceMode.Acceleration);
            }
        }

        private void WallJump()
        {
            var sideForce = Vector3.zero;
            if (_isWallRight) sideForce = _righthitinfo.normal * wallJumpSideForce;
            if (_isWallLeft) sideForce = _lefthitinfo.normal * wallJumpSideForce;

            var jumpForce = Vector3.up * Mathf.Sqrt(wallJumpHeight * -2 * Physics.gravity.y);

            var forwardForce = transform.forward * wallJumpForwardForce;

            var finalForce = jumpForce + sideForce + forwardForce;

            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
            _rigidbody.AddForce(finalForce, ForceMode.VelocityChange);

            _playerCharacterController.Jumping = false;
        }
    }
}