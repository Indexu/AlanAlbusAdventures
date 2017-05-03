using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour 
{
	public float movementSpeed;
	public float maxHealth;
	public float critHitChance;
	public float critHitDamage;
	public float baseDamage;
	public float attackSpeed;
	public int statPoints;

	public Text movementSpeedText;
	public Text maxHealthText;
	public Text critHitChanceText;
	public Text critHitDamageText;
	public Text baseDamageText;
	public Text attackSpeedText;
	public GameObject statsTable;

	public void UpgradeMovementSpeed()
	{
		statPoints--;
		movementSpeed += 15;
	}

	public void UpgradeMaxHealth()
	{
		statPoints--;
		maxHealth += 2f;
	}

	public void UpgradeCritHitChance()
	{
		statPoints--;
		critHitChance += 5f;
	}

	public void UpgradeCritHitDamage()
	{
		statPoints--;
		critHitDamage += 0.2f;
	}

	public void UpgradeBaseDamage()
	{
		statPoints--;
		baseDamage += 1f;
	}

	public void UpgradeAttackSpeed()
	{
		statPoints--;
		attackSpeed -= 0.15f;
	}

	public void UpdateUI()
	{
		movementSpeedText.text = movementSpeed.ToString();
		maxHealthText.text = maxHealth.ToString();
		critHitChanceText.text = critHitChance + "%";
		critHitDamageText.text = critHitDamage + "x";
		baseDamageText.text = baseDamage.ToString();
		attackSpeedText.text = attackSpeed + "s";
	}

	public void ShowStats()
	{
		UpdateUI();
		statsTable.SetActive(true);
	}

	public void HideStats()
	{
		statsTable.SetActive(false);
	}

	private void Start()
	{
		HideStats();
	}
}
