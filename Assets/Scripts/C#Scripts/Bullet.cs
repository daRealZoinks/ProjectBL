using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _destroyDelay = 5f;

    public Rigidbody Rigidbody { get; private set; }

    private float _destroyTimer = 0f;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _destroyTimer += Time.deltaTime;

        if (_destroyTimer >= _destroyDelay)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
