using LocalPlayer;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(RigidbodyPredictor))]
    public class Projectile : MonoBehaviour
    {
        [Tooltip("The time the projectile will be destroyed after being launched")]
        [SerializeField]
        private float lifeSpan = 5f;

        private void Start()
        {
            Destroy(gameObject, lifeSpan);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }
    }
}