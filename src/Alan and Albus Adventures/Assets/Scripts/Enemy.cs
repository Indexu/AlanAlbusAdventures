using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour 
{
	public float speed;
	public float damage;
	public bool magicalDamage;

	protected Rigidbody2D rb2d;
	protected GameObject target;
	protected Vector2 targetVector;
	protected const float targetSwitchThreshold = 0.6f;

	protected virtual void Start() 
	{
		rb2d = GetComponent<Rigidbody2D>();
		target = null;
	}
	
	protected virtual void FixedUpdate() 
	{		
		GetTarget();
		Move();
	}

	protected void OnTriggerStay2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			var collisionVitalityController = collider.gameObject.GetComponent<VitalityController>();

			if (!collisionVitalityController.isInvincibilityFrame || collisionVitalityController.isInvincible)
			{
				collisionVitalityController.Damage(damage, magicalDamage);

				var forceVector = collider.gameObject.transform.position - transform.position;

				collider.gameObject.GetComponent<PlayerController>().Knockback(forceVector);
			}
		}
	}

	protected void GetTarget()
	{
		var players = GameObject.FindGameObjectsWithTag("Player");

		if (players[0].GetComponent<VitalityController>().isDead)
		{
			target = players[1];
		}
		else if (players[1].GetComponent<VitalityController>().isDead)
		{
			target = players[0];
		}
		else
		{
			var player1Dist = Vector2.Distance(transform.position, players[0].transform.position);
			var player2Dist = Vector2.Distance(transform.position, players[1].transform.position);

			if (target != null)
			{
				if (target == players[0] && player2Dist < (player1Dist * targetSwitchThreshold))
				{
					target = players[1];
				}
				else if (target == players[1] && player1Dist < (player2Dist * targetSwitchThreshold))
				{
					target = players[0];
				}
			}
			else
			{
				target = (player1Dist < player2Dist ? players[0] : players[1]);
			}
		}

		targetVector = target.transform.position - transform.position;
	}

	protected abstract void Move();
}
