using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour 
{
	public int playerID; // ReWired player ID
	public float projectileForce;
	public float knockbackForce;
	public bool magicalDamage;
	public GameObject projectile;

	private Rigidbody2D rb2d;
	private VitalityController vc;
	public DoorController door;
	private Stats stats;
	private Player player;
	private Vector2 moveVector;
	private Vector2 rotationVector;
	private Vector2 knockbackVector;
	private bool knockback;
	private float nextFire;
	private bool doShoot;
	private bool inStatsScreen;

	public void Knockback(Vector2 direction)
	{
		knockback = true;
		knockbackVector = direction.normalized;
	}

	// Use this for initialization
	private void Start() 
	{
		player = ReInput.players.GetPlayer(playerID);
		rb2d = GetComponent<Rigidbody2D>();
		vc = GetComponent<VitalityController>();
		stats = GetComponent<Stats>();
		rotationVector = Vector2.up;
		inStatsScreen = false;
	}

	private void Update()
	{
		if (!vc.isDead)
		{
			CheckInput();
		}
	}
	
	// Update is called once per frame
	private void FixedUpdate()
	{
		CheckKnockback();

		if (!vc.isDead && !inStatsScreen)
		{
			Rotation();
			Movement();

			if (doShoot)
			{
				Shoot();
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "Door")
		{
			door = collider.gameObject.GetComponent<DoorController>();
		}
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "Door")
		{
			door = null;
		}
	}

	private void CheckInput()
	{
		if (inStatsScreen)
		{
			if (player.GetButtonUp("Stats Screen"))
			{
				inStatsScreen = false;
				stats.HideStats();
			}
			if (player.GetButtonUp("DPad Up"))
			{
				stats.Up();
			}
			if (player.GetButtonUp("DPad Down"))
			{
				stats.Down();
			}
			if (player.GetButtonUp("Confirm"))
			{
				stats.UpgradeStat();
			}
		}
		else
		{
			if (player.GetButton("Shoot"))
			{
				doShoot = true;
			}
			if (player.GetButtonUp("Stats Screen"))
			{
				inStatsScreen = true;
				stats.ShowStats();
			}
			if (player.GetButtonUp("Confirm") && door != null)
			{
				door.GoThrough();
			}
		}
	}

	private void Shoot()
	{	
		if (nextFire < Time.time)
		{
			nextFire = Time.time + stats.attackSpeed;

			var projectileInstance = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
			projectileInstance.GetComponent<Rigidbody2D>().AddForce(rotationVector.normalized * projectileForce, ForceMode2D.Impulse);

			var projectileComponent = projectileInstance.GetComponent<Projectile>();
			projectileComponent.damage = stats.baseDamage;
			projectileComponent.isMagical = magicalDamage;
			projectileComponent.playerFired = true;

			if (Random.value * 100 < stats.critHitChance)
			{
				projectileComponent.damage *= stats.critHitDamage;
			}

			projectileComponent.Init();
		}

		doShoot = false;
	}

	private void Rotation()
	{
		float x = player.GetAxis("Look Horizontal");
		float y = player.GetAxis("Look Vertical");

		if (x != 0 || y != 0)
		{
			rotationVector.x = x;
			rotationVector.y = y;

			float angle = Mathf.Atan2 (-rotationVector.x, rotationVector.y);

			angle *= Mathf.Rad2Deg;

			rb2d.freezeRotation = false;
			rb2d.MoveRotation (angle);
		}
		else
		{
			rb2d.freezeRotation = true;
		}
	}

	private void Movement()
	{
		moveVector.x = player.GetAxis("Move Horizontal");
		moveVector.y = player.GetAxis("Move Vertical");

		rb2d.AddForce(moveVector * stats.movementSpeed);
	}

	private void CheckKnockback()
	{
		if (knockback) 
		{
			knockback = false;
			rb2d.AddForce(knockbackVector * knockbackForce, ForceMode2D.Impulse);
		}
	}
}
