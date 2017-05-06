using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour 
{
	public int playerID; // ReWired player ID
	public float knockbackForce;
	public ProjectileDirection projectileDirection;

	private Rigidbody2D rb2d;
	private VitalityController vc;
	private DoorController door;
	private GameManager gameManager;
	private Stats stats;
	private Player player;
	private Vector2 moveVector;
	private Vector2 rotationVector;
	private Vector2 knockbackVector;
	private bool knockback;
	private bool doAttack;
	private bool inStatsScreen;

	public void Knockback(Vector2 direction)
	{
		knockback = true;
		knockbackVector = direction.normalized;
	}

	private void Start() 
	{
		player = ReInput.players.GetPlayer(playerID);
		rb2d = GetComponent<Rigidbody2D>();
		vc = GetComponent<VitalityController>();
		stats = GetComponent<Stats>();
		rotationVector = Vector2.up;
		inStatsScreen = false;
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}

	private void Update()
	{
		if (!vc.isDead)
		{
			CheckInput();
		}
		else
		{
			if (player.GetButtonUp("Confirm"))
			{
				gameManager.DeadReset();
			}
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

			if (doAttack)
			{
				if (playerID == 0)
				{
					projectileDirection.Slash(stats, rotationVector);
				}
				else
				{
					projectileDirection.Shoot(stats, rotationVector);
				}

				doAttack = false;
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
		if (gameManager.isPaused)
		{
			if (player.GetButtonUp("Pause"))
			{
				gameManager.Unpause();
			}
			if (player.GetButtonUp("Confirm"))
			{
				gameManager.Reset();
			}
		}
		else if (inStatsScreen)
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
			if (player.GetButton("Attack"))
			{
				doAttack = true;
			}
			if (player.GetButtonUp("Stats Screen"))
			{
				inStatsScreen = true;
				stats.ShowStats();
			}
			if (player.GetButtonUp("Confirm") && door != null)
			{
				if (!gameManager.changingRooms)
				{
					door.GoThrough();
				}
			}
			if (player.GetButtonUp("Pause"))
			{
				gameManager.Pause();
			}
		}
	}

	private void Rotation()
	{
		float x = player.GetAxis("Look Horizontal");
		float y = player.GetAxis("Look Vertical");

		if (x != 0 || y != 0)
		{
			rotationVector.x = x;
			rotationVector.y = y;
			projectileDirection.SetCrosshair(rotationVector);
		}
	}

	private void Movement()
	{
		moveVector.x = player.GetAxis("Move Horizontal");
		moveVector.y = player.GetAxis("Move Vertical");

		rb2d.AddForce(moveVector * stats.movementSpeed, ForceMode2D.Impulse);
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
