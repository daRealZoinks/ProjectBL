using UnityEngine;

namespace StolenFromBrackeys
{
    [RequireComponent(typeof(ConfigurableJoint))]
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float lookSensitivity = 0.1f;
        [SerializeField] private float thrusterForce = 1000f;

        [Header("Joint settings")] 
        [SerializeField] private float jointSpring = 20f;
        [SerializeField] private float jointDamper = 4f;
        [SerializeField] private float jointMaxForce = 40f;
        private JointDrive _jointDrive;
        
        private PlayerMotor _motor;
        private ConfigurableJoint _joint;

        public Vector2 Movement { get; set; }
        public Vector2 Rotation { get; set; }
        public bool Jumping { get; set; }

        private void Start()
        {
            _motor = GetComponent<PlayerMotor>();
            _joint = GetComponent<ConfigurableJoint>();
        }

        private void Update()
        {
            _joint.targetPosition =
                Physics.Raycast(transform.position, Vector3.down, out var hit, 100f)
                    ? new Vector3(0f, -hit.point.y, 0f)
                    : new Vector3(0f, 0f, 0f);
            
            // Calculate movement velocity as a 3D vector
            var xMov = Movement.x;
            var zMov = Movement.y;
            var playerTransform = transform;
            var movHorizontal = playerTransform.right * xMov;
            var movVertical = playerTransform.forward * zMov;
            var velocity = (movHorizontal + movVertical).normalized * speed;

            // Apply movement
            _motor.Velocity = velocity;

            // Calculate rotation as a 3D vector (turning around)
            var yRot = Rotation.x;
            var rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;

            // Apply rotation
            _motor.Rotation = rotation;

            // Calculate camera rotation as a 3D vector (turning around)
            var xRot = Rotation.y;
            var cameraRotation = new Vector3(xRot, 0f, 0f) * lookSensitivity;

            // Apply camera rotation
            _motor.CameraRotation = cameraRotation;
            
            // Calculate the thrusterforce based on player input
            var currentThrusterForce = Vector3.zero;
            if (Jumping)
            {
                currentThrusterForce = Vector3.up * thrusterForce;
                SetJointSettings(jointMaxForce,0f,0f);
            }
            else
            {
                SetJointSettings(jointMaxForce,jointDamper, jointSpring);
            }
            
            // Apply the thruster force
            _motor.ThrusterForce = currentThrusterForce;
        }
        
        private void SetJointSettings(float maxForce,float damperForce,float jointSpringForce)
        {
            _joint.yDrive = new JointDrive
            {
                maximumForce = maxForce,
                positionDamper = damperForce,
                positionSpring = jointSpringForce
            };
        }
    }
}