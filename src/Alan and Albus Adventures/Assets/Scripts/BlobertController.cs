using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobertController : Boss 
{
	public float projectileForce;
	public float fireRate;
	public GameObject projectile;

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
		targetVector.x = target.transform.position.x - transform.position.x;
		targetVector.y = target.transform.position.y - transform.position.y;

		rb2d.AddForce(targetVector.normalized * speed);
	}

	private void Shoot()
	{
		if (nextFire < Time.time)
		{
			nextFire = Time.time + fireRate;

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
