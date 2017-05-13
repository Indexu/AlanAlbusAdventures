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

    private Text movementSpeedText;
    private Text maxHealthText;
    private Text critHitChanceText;
    private Text critHitDamageText;
    private Text baseDamageText;
    private Text attackSpeedText;
    private Text statPointsText;
    private Text movementSpeedBonusText;
    private Text maxHealthBonusText;
    private Text critHitChanceBonusText;
    private Text critHitDamageBonusText;
    private Text baseDamageBonusText;
    private Text attackSpeedBonusText;
    private RectTransform selectedBorder;
    private GameObject statsTable;
    private int viewing;

    private VitalityController vc;
    private Inventory inventory;
    private int playerID;
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
    private bool showUnmodified;

    public void UpgradeMovementSpeed()
    {
        movementSpeed += 1;
    }

    public void UpgradeMaxHealth()
    {
        maxHealth += 5;
        vc.currentHealth += 5;
    }

    public void UpgradeCritHitChance()
    {
        critHitChance += 5f;
    }

    public void UpgradeCritHitDamage()
    {
        critHitDamage += 2f;
    }

    public void UpgradeBaseDamage()
    {
        baseDamage += 1f;
    }

    public void UpgradeAttackSpeed()
    {
        attackSpeed -= 0.05f;
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

    private void UpdateUI()
    {
        statPointsText.text = statPoints.ToString();

        var ms = movementSpeed;
        var mh = (float)maxHealth;
        var cc = critHitChance;
        var cd = critHitDamage;
        var bd = baseDamage;
        var atk = attackSpeed;

        if (!showUnmodified)
        {
            ms *= inventory.GetStatBonus(Property.MOVEMENTSPEED);
            mh *= inventory.GetStatBonus(Property.MAXHEALTH);
            cc *= inventory.GetStatBonus(Property.CRITCHANCE);
            cd *= inventory.GetStatBonus(Property.CRITDAMAGE);
            bd *= inventory.GetStatBonus(Property.BASEDAMAGE);
            atk /= inventory.GetStatBonus(Property.ATTACKSPEED);

            movementSpeedBonusText.color = Color.white;
            maxHealthBonusText.color = Color.white;
            critHitChanceBonusText.color = Color.white;
            critHitDamageBonusText.color = Color.white;
            baseDamageBonusText.color = Color.white;
            attackSpeedBonusText.color = Color.white;
        }
        else
        {
            movementSpeedBonusText.color = Color.gray;
            maxHealthBonusText.color = Color.gray;
            critHitChanceBonusText.color = Color.gray;
            critHitDamageBonusText.color = Color.gray;
            baseDamageBonusText.color = Color.gray;
            attackSpeedBonusText.color = Color.gray;
        }

        movementSpeedText.text = ms.ToString("0");
        maxHealthText.text = mh.ToString("0");
        critHitChanceText.text = cc.ToString("0") + "%";
        critHitDamageText.text = cd.ToString("0") + "x";
        baseDamageText.text = bd.ToString("0");
        attackSpeedText.text = atk.ToString("0.00") + "s";

        movementSpeedBonusText.text = "(x" + inventory.GetStatBonus(Property.MOVEMENTSPEED).ToString("0.00") + ")";
        maxHealthBonusText.text = "(x" + inventory.GetStatBonus(Property.MAXHEALTH).ToString("0.00") + ")";
        critHitChanceBonusText.text = "(x" + inventory.GetStatBonus(Property.CRITCHANCE).ToString("0.00") + ")";
        critHitDamageBonusText.text = "(x" + inventory.GetStatBonus(Property.CRITDAMAGE).ToString("0.00") + ")";
        baseDamageBonusText.text = "(x" + inventory.GetStatBonus(Property.BASEDAMAGE).ToString("0.00") + ")";
        attackSpeedBonusText.text = "(x" + inventory.GetStatBonus(Property.ATTACKSPEED).ToString("0.00") + ")";

        if (inventory.GetStatBonus(Property.MOVEMENTSPEED) != 1f && !showUnmodified)
        {
            movementSpeedText.color = Color.green;
        }
        else
        {
            movementSpeedText.color = Color.white;
        }

        if (inventory.GetStatBonus(Property.MAXHEALTH) != 1f && !showUnmodified)
        {
            maxHealthText.color = Color.green;
        }
        else
        {
            maxHealthText.color = Color.white;
        }

        if (inventory.GetStatBonus(Property.CRITCHANCE) != 1f && !showUnmodified)
        {
            critHitChanceText.color = Color.green;
        }
        else
        {
            critHitChanceText.color = Color.white;
        }

        if (inventory.GetStatBonus(Property.CRITDAMAGE) != 1f && !showUnmodified)
        {
            critHitDamageText.color = Color.green;
        }
        else
        {
            critHitDamageText.color = Color.white;
        }

        if (inventory.GetStatBonus(Property.BASEDAMAGE) != 1f && !showUnmodified)
        {
            baseDamageText.color = Color.green;
        }
        else
        {
            baseDamageText.color = Color.white;
        }

        if (inventory.GetStatBonus(Property.ATTACKSPEED) != 1f && !showUnmodified)
        {
            attackSpeedText.color = Color.green;
        }
        else
        {
            attackSpeedText.color = Color.white;
        }

        vc.doUpdateUI = true;
    }

    public void ShowStats()
    {
        doUpdateUI = true;
        showUnmodified = false;
        statsTable.SetActive(true);
        UIManager.instance.ShowExperienceBar();
        viewing++;
    }

    public void HideStats()
    {
        showUnmodified = false;
        statsTable.SetActive(false);
        viewing--;

        if (viewing <= 0)
        {
            viewing = 0;
            UIManager.instance.HideExperienceBar();
        }
    }

    public void UnmodifiedStats(bool show)
    {
        showUnmodified = show;
        doUpdateUI = true;
    }

    private void Start()
    {
        vc = GetComponent<VitalityController>();
        inventory = GetComponent<Inventory>();

        selectedStat = 1;
        doUpdateUI = true;
        doSetSelectedBorder = true;

        FindTexts();
        HideStats();

        viewing = 0;
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

    private void FindTexts()
    {
        playerID = GetComponent<PlayerController>().playerID;
        var searchString = (playerID == 0 ? "AlanStats" : "AlbusStats");

        statsTable = UIManager.instance.canvas.transform.Find(searchString).gameObject;

        movementSpeedText = statsTable.transform.Find("MovementSpeedValueText").GetComponent<Text>();
        maxHealthText = statsTable.transform.Find("MaxHealthValueText").GetComponent<Text>();
        critHitChanceText = statsTable.transform.Find("CritChanceValueText").GetComponent<Text>();
        critHitDamageText = statsTable.transform.Find("CritDamageValueText").GetComponent<Text>();
        baseDamageText = statsTable.transform.Find("BaseDamageValueText").GetComponent<Text>();
        attackSpeedText = statsTable.transform.Find("AttackSpeedValueText").GetComponent<Text>();

        movementSpeedBonusText = statsTable.transform.Find("MovementSpeedBonusText").GetComponent<Text>();
        maxHealthBonusText = statsTable.transform.Find("MaxHealthBonusText").GetComponent<Text>();
        critHitChanceBonusText = statsTable.transform.Find("CritChanceBonusText").GetComponent<Text>();
        critHitDamageBonusText = statsTable.transform.Find("CritDamageBonusText").GetComponent<Text>();
        baseDamageBonusText = statsTable.transform.Find("BaseDamageBonusText").GetComponent<Text>();
        attackSpeedBonusText = statsTable.transform.Find("AttackSpeedBonusText").GetComponent<Text>();

        statPointsText = statsTable.transform.Find("StatPointsValueText").GetComponent<Text>();
        selectedBorder = statsTable.transform.Find("SelectedBorder").GetComponent<RectTransform>();
    }
}
