using UnityEngine;

public class BulletShoot : MonoBehaviour
{
    [SerializeField] private Bullet bullet;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootDelay = 1f;
    [SerializeField] private float shootForce = 100f;

    private float _shootTimer;

    private void Update()
    {
        _shootTimer += Time.deltaTime;
    }

    public void ShootBullet()
    {
        if (_shootTimer < shootDelay) return;

        var bulletInstance = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        bulletInstance.Rigidbody.AddForce(shootForce * bulletInstance.transform.forward, ForceMode.VelocityChange);

        _shootTimer = 0f;
    }
}