using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileBlobController : Enemy
{

    public float projectileForce;
    public float fireRate;
    public float minTime;
    public float maxTime;
    public GameObject projectile;
    public List<AudioClip> blobFireSounds;

    private float nextFire;
    private Vector2 moveVector;
    private float nextVector;

    protected override void Start()
    {
        base.Start();
        nextFire = 0;
    }

    protected override void FixedUpdate()
    {
        if (Attacking)
        {
            base.FixedUpdate();

            Shoot();

        }
    }

    protected override void Move()
    {
        if (nextVector < Time.time)
        {
            moveVector.x = Random.Range(-1f, 1f);
            moveVector.y = Random.Range(-1f, 1f);

            nextVector = Time.time + Random.Range(minTime, maxTime);
        }

        rb2d.AddForce(moveVector * speed, ForceMode2D.Impulse);
    }

    private void Shoot()
    {
        if (nextFire < Time.time)
        {
            nextFire = Time.time + fireRate;
            int index = Random.Range(0, blobFireSounds.Count);
            SoundManager.instance.PlaySounds(blobFireSounds.ElementAt(index));
            var projectileInstance = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
            projectileInstance.GetComponent<Rigidbody2D>().AddForce(targetVector.normalized * projectileForce, ForceMode2D.Impulse);

            var projectileComponent = projectileInstance.GetComponent<Projectile>();
            projectileComponent.damage = damage;
            projectileComponent.isMagical = magicalDamage;
            projectileComponent.playerFired = false;
            projectileComponent.Init();
        }
    }
}
