using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _destroyDelay = 5f;

    private float _destroyTimer;

    public Rigidbody Rigidbody { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _destroyTimer += Time.deltaTime;

        if (_destroyTimer >= _destroyDelay) Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}