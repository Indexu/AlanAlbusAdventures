﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBlobController : Enemy {

	public float projectileForce;
	public float fireRate;
	public float minTime;
	public float maxTime;
	public GameObject projectile;

	private float nextFire;
	private Vector2 moveVector;
	private float nextVector;

	// Update is called once per frame
	protected override void FixedUpdate() 
	{		
		base.FixedUpdate();

		Shoot();
	}

	protected override void Move()
	{
		if (nextVector < Time.time)
		{
			moveVector.x = Random.Range(-1f, 1f);
			moveVector.y = Random.Range(-1f, 1f);

			nextVector = Time.time + Random.Range(minTime, maxTime);
		}

		rb2d.AddForce(moveVector * speed);
	}

	private void Shoot()
	{
		if (nextFire < Time.time)
		{
			nextFire = Time.time + fireRate;

			var projectileInstance = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
			projectileInstance.GetComponent<Rigidbody2D>().AddForce(targetVector.normalized * projectileForce, ForceMode2D.Impulse);

			var projectileComponent = projectileInstance.GetComponent<Projectile>();
			projectileComponent.damage = damage;
			projectileComponent.isMagical = magicalDamage;
			projectileComponent.playerFired = false;
			projectileComponent.Init();
		}
	}
}