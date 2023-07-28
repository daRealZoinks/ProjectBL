using UnityEngine;

namespace LocalPlayer.ProceduralAnimation
{
    public class FloatAndRotate : MonoBehaviour
    {
        [SerializeField]
        private float height = 0.5f;

        [SerializeField]
        private float floatSpeed = 0.5f;

        [SerializeField]
        private float rotateSpeed = 50f;

        private Vector3 _startPosition;

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            // Float the object up and down
            transform.position = _startPosition + new Vector3(0f, height * Mathf.Sin(Time.time * floatSpeed), 0f);

            // Rotate the object around the Y axis
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        }
    }
}