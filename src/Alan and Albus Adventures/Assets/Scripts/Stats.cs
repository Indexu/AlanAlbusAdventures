using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour 
{
	public float movementSpeed;
	public int maxHealth;
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
	public Text statPointsText;
	public RectTransform selectedBorder;
	public GameObject statsTable;

	private VitalityController vc;

	private const float maxHealthYPos = 248f;
	private const float movementSpeedYPos = 123.7f;
	private const float attackSpeedYPos = 0f;
	private const float critHitChanceYPos = -123.6f;
	private const float critHitDamageYPos = -247.3f;
	private const float baseDamageYPos = -371f;

	private const int maxHealthIndex = 1;
	private const int movementSpeedIndex = 2;
	private const int attackSpeedIndex = 3;
	private const int critHitChanceIndex = 4;
	private const int critHitDamageIndex = 5;
	private const int baseDamageIndex = 6;

	private int selectedStat = maxHealthIndex;
	private const int minSelectedStat = maxHealthIndex;
	private const int maxSelectedStat = baseDamageIndex;
	private bool doUpdateUI;
	private bool doSetSelectedBorder;

	public void UpgradeMovementSpeed()
	{
		movementSpeed += 15;
	}

	public void UpgradeMaxHealth()
	{
		maxHealth += 2;
		vc.currentHealth += 2;
	}

	public void UpgradeCritHitChance()
	{
		critHitChance += 5f;
	}

	public void UpgradeCritHitDamage()
	{
		critHitDamage += 0.2f;
	}

	public void UpgradeBaseDamage()
	{
		baseDamage += 1f;
	}

	public void UpgradeAttackSpeed()
	{
		attackSpeed -= 0.1f;
	}

	public void UpgradeStat()
	{
		if (0 < statPoints)
		{
			statPoints--;

			switch (selectedStat)
			{
			case maxHealthIndex:
				{
					UpgradeMaxHealth();
					break;
				}

			case movementSpeedIndex:
				{
					UpgradeMovementSpeed();
					break;
				}

			case attackSpeedIndex:
				{
					UpgradeAttackSpeed();
					break;
				}

			case critHitChanceIndex:
				{
					UpgradeCritHitChance();
					break;
				}

			case critHitDamageIndex:
				{
					UpgradeCritHitDamage();
					break;
				}

			case baseDamageIndex:
				{
					UpgradeBaseDamage();
					break;
				}

			default:
				{
					break;
				}
			}

			doUpdateUI = true;
		}
	}

	public void Down()
	{
		selectedStat++;

		if (maxSelectedStat < selectedStat)
		{
			selectedStat = maxHealthIndex;
		}

		doSetSelectedBorder = true;
	}

	public void Up()
	{
		selectedStat--;

		if (selectedStat < minSelectedStat)
		{
			selectedStat = baseDamageIndex;
		}

		doSetSelectedBorder = true;
	}

	public void UpdateUI()
	{
		movementSpeedText.text = movementSpeed.ToString();
		maxHealthText.text = maxHealth.ToString();
		critHitChanceText.text = critHitChance + "%";
		critHitDamageText.text = critHitDamage + "x";
		baseDamageText.text = baseDamage.ToString();
		attackSpeedText.text = attackSpeed.ToString("0.00") + "s";
		statPointsText.text = statPoints.ToString();
		vc.doUpdateUI = true;
	}

	public void ShowStats()
	{
		doUpdateUI = true;
		statsTable.SetActive(true);
	}

	public void HideStats()
	{
		statsTable.SetActive(false);
	}

	private void Start()
	{
		HideStats();
		vc = GetComponent<VitalityController>();
		doUpdateUI = true;
		doSetSelectedBorder = true;
	}

	private void OnGUI()
	{
		if (doUpdateUI)
		{
			doUpdateUI = false;
			UpdateUI();
		}
		if (doSetSelectedBorder)
		{
			doSetSelectedBorder = false;
			SetSelectedBorder();
		}
	}

	private void SetSelectedBorder()
	{
		switch (selectedStat)
		{
			case maxHealthIndex:
			{
				selectedBorder.localPosition = new Vector3(selectedBorder.localPosition.x, maxHealthYPos, selectedBorder.localPosition.z);
				break;
			}

			case movementSpeedIndex:
			{
				selectedBorder.localPosition = new Vector3(selectedBorder.localPosition.x, movementSpeedYPos, selectedBorder.localPosition.z);
				break;
			}
				
			case attackSpeedIndex:
			{
				selectedBorder.localPosition = new Vector3(selectedBorder.localPosition.x, attackSpeedYPos, selectedBorder.localPosition.z);
				break;
			}

			case critHitChanceIndex:
			{
				selectedBorder.localPosition = new Vector3(selectedBorder.localPosition.x, critHitChanceYPos, selectedBorder.localPosition.z);
				break;
			}
				
			case critHitDamageIndex:
			{
				selectedBorder.localPosition = new Vector3(selectedBorder.localPosition.x, critHitDamageYPos, selectedBorder.localPosition.z);
				break;
			}
				
			case baseDamageIndex:
			{
				selectedBorder.localPosition = new Vector3(selectedBorder.localPosition.x, baseDamageYPos, selectedBorder.localPosition.z);
				break;
			}
				
			default:
			{
				break;
			}
		}
	}
}
