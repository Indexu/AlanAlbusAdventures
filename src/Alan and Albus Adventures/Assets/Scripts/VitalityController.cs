using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VitalityController : MonoBehaviour 
{
	public float currentHealth;
	public float magicResistance;
	public float physicalResistance;
	public bool isDead = false;
	public bool isInvincible;
	public bool isInvincibilityFrame;
	public float invincibilityFrameTime;
	public Text healthText;
	public Slider healthSlider;
	public bool doUpdateUI;

	private float nextDamage;
	private Stats stats;
	private GameManager gameManager;

	public void Damage(float amount, bool isMagical)
	{
		if(!isInvincible && !isInvincibilityFrame && !isDead)
		{
			isInvincibilityFrame = true;

			amount /= (isMagical ? magicResistance : physicalResistance);

			currentHealth -= amount;

			if (gameObject.tag == "Player")
			{
				doUpdateUI = true;
			}

			CheckHealth();

			StartCoroutine(InvincibiltyFrame());
		}
	}

	public void Heal(float amount)
	{
		currentHealth += amount;
	}

	private void Start()
	{
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		if (gameObject.tag == "Player")
		{
			doUpdateUI = true;
			stats = GetComponent<Stats> ();
		}
		else
		{
			doUpdateUI = false;
		}
	}

	private void OnGUI()
	{
		if (doUpdateUI)
		{
			UpdateUI();
		}
	}

	private void CheckHealth()
	{
		if (currentHealth <= 0)
		{
			if (gameObject.tag == "Player")
			{
				isDead = true;
			}
			else
			{
				gameManager.EnemyKilled();
				GameObject.Destroy(gameObject);
			}
		}	
	}

	private void UpdateUI()
	{
		healthText.text = currentHealth + "/" + stats.maxHealth;
		healthSlider.value = currentHealth / stats.maxHealth;
	}

	private IEnumerator InvincibiltyFrame()
	{
		yield return new WaitForSeconds(invincibilityFrameTime);
		isInvincibilityFrame = false;
	}
}
