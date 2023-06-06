using UnityEngine;

public class BulletShoot : MonoBehaviour
{
    [SerializeField] private Bullet _bullet;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private float _shootDelay = 2f;
    [SerializeField] private float _shootForce = 10f;

    private float _shootTimer = 0f;

    private void Update()
    {
        _shootTimer += Time.deltaTime;
    }

    public void ShootBullet()
    {
        if (_shootTimer < _shootDelay) return;

        var bullet = Instantiate(_bullet, _shootPoint.position, _shootPoint.rotation);
        bullet.Rigidbody.AddForce(_shootForce * bullet.transform.forward, ForceMode.VelocityChange);

        _shootTimer = 0f;
    }
}
