using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalityController : MonoBehaviour 
{
	public float health;
	public float magicResistance;
	public float physicalResistance;
	public bool isDead = false;
	public bool isInvincible;
	public bool isInvincibilityFrame;
	public float invincibilityFrameTime;

	private float nextDamage;

	public void Damage(float amount, bool isMagical)
	{
		if(!isInvincible && !isInvincibilityFrame)
		{
			isInvincibilityFrame = true;

			amount /= (isMagical ? magicResistance : physicalResistance);

			health -= amount;
			CheckHealth();

			StartCoroutine(InvincibiltyFrame());
		}
	}

	public void Heal(float amount)
	{
		health += amount;
	}

	private void CheckHealth()
	{
		if (health <= 0)
		{
			if (gameObject.tag == "Player")
			{
				isDead = true;
			}
			else
			{
				GameObject.Destroy(gameObject);
			}
		}	
	}

	private IEnumerator InvincibiltyFrame()
	{
		yield return new WaitForSeconds(invincibilityFrameTime);
		isInvincibilityFrame = false;
	}
}
