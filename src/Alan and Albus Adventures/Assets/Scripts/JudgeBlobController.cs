using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JudgeBlobController : Boss
{

    public float projectileForce;
    public float minTime;
    public float maxTime;
    public GameObject projectile;
    public List<AudioClip> blobFireSounds;

    private float fireRate;
    private float nextFire;
    private Vector2 moveVector;
    private float nextVector;
    private VitalityController vc;
    private float maxHealth;
    private Vector2 rotatingAimVector;
    private const float baseRotationSpeed = 40f;

    protected override void Start()
    {
        base.Start();

        vc = GetComponent<VitalityController>();

        maxHealth = vc.currentHealth;
        fireRate = vc.currentHealth / maxHealth;
        rotatingAimVector = Vector2.up;
    }

    protected override void FixedUpdate()
    {
        if (Attacking)
        {
            base.FixedUpdate();

            fireRate = (vc.currentHealth / maxHealth) / 2;

            rotatingAimVector = Quaternion.AngleAxis(-(baseRotationSpeed / fireRate) * Time.fixedDeltaTime, Vector3.forward) * rotatingAimVector;

            Vector3 v = new Vector3(rotatingAimVector.x, rotatingAimVector.y, 0f);
            Debug.DrawLine(transform.position, transform.position + (v * 100));

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
            projectileInstance.GetComponent<Rigidbody2D>().AddForce(rotatingAimVector.normalized * projectileForce, ForceMode2D.Impulse);

            var projectileComponent = projectileInstance.GetComponent<Projectile>();
            projectileComponent.damage = damage;
            projectileComponent.isMagical = magicalDamage;
            projectileComponent.playerFired = false;
            projectileComponent.Init();
        }
    }
}
