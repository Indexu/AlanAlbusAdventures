using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem
{
    // Standard stats
    public Quality quality;
    public Property property;
    public int baseStat;

    // Bonus stats
    public Postfix bonusQuality;
    public Property bonusProperty;
    public int bonusBaseStat;

    public string itemName;
    public string statsText;
}

public class Inventory : MonoBehaviour
{
    public float healAmount;

    public int healthPotions = 0;

    public int[] bonusStats = new int[statsCount]
    {
        100, 100, 100, 100, 100, 100
    };

    private const int passiveItemCount = 4;
    private const int statsCount = 6;

    private InventoryItem[] passives = new InventoryItem[passiveItemCount];

    private VitalityController vc;
    private PlayerController pc;
    private Text healthPotionCounterText;
    private bool updateUI = false;
    private int updatePos;
    private bool destroy;
    private float currentDestroyTime;

    public bool AddItem(Item item, int pos)
    {
        if (item != null)
        {
            if (0 <= pos && pos < passiveItemCount && passives[pos] == null)
            {
                passives[pos] = new InventoryItem
                {
                    quality = item.quality,
                    property = item.property,
                    baseStat = item.baseStat,
                    bonusQuality = item.bonusQuality,
                    bonusProperty = item.bonusProperty,
                    bonusBaseStat = item.bonusBaseStat,
                    statsText = item.statsText,
                    itemName = item.itemName
                };
                bonusStats[(int)item.property] += ((int)item.quality * item.baseStat);
                if (item.bonusQuality != 0 && item.bonusBaseStat != 0)
                {
                    bonusStats[(int)item.bonusProperty] += ((int)item.bonusQuality * item.bonusBaseStat);
                }

                if (item.property == Property.MAXHEALTH || (item.bonusProperty == Property.MAXHEALTH && item.bonusBaseStat != 0))
                {
                    vc.doUpdateUI = true;
                }

                return true;
            }
        }

        return false;
    }

    public void RemoveItem(int pos)
    {
        if (0 <= pos && pos < passiveItemCount && passives[pos] != null)
        {
            InventoryItem discardItem = passives[pos];
            bonusStats[(int)discardItem.property] -= ((int)discardItem.quality * discardItem.baseStat);
            if (discardItem.bonusQuality != 0 && discardItem.bonusBaseStat != 0)
            {
                bonusStats[(int)discardItem.bonusProperty] -= ((int)discardItem.bonusQuality * discardItem.bonusBaseStat);
            }
            passives[pos] = null;
        }
    }

    public void AddHealthPotion(int charges)
    {
        healthPotions += charges;
        SetCharges();
    }

    public void UseHealthPotion()
    {
        if (healthPotions > 0)
        {
            healthPotions--;
            vc.Heal(healAmount);
            SetCharges();
        }
    }

    public float GetStatBonus(Property property)
    {
        return ((float)bonusStats[(int)property] / 100f);
    }

    public void ViewItem(int pos)
    {
        currentDestroyTime = 0f;
        updateUI = true;
        destroy = false;
        updatePos = pos;
        UIManager.instance.SetInventoryTooltipDestroySlider(pc.playerID, 0f);
    }

    public void DestroyItem(int pos, float destroyTime)
    {
        if (pos == updatePos)
        {
            destroy = true;
            currentDestroyTime = destroyTime;
        }
        else
        {
            currentDestroyTime = 0f;
            destroy = false;
        }
    }

    private void Start()
    {
        vc = GetComponent<VitalityController>();
        pc = GetComponent<PlayerController>();

        var playerID = GetComponent<PlayerController>().playerID;
        var searchString = (playerID == 0 ? "AlanItemSlots" : "AlbusItemSlots");
        healthPotionCounterText = UIManager.instance.canvas.transform.Find(searchString).Find("UseItemSlot").Find("UsesFrame").Find("Text").GetComponent<Text>();

        SetCharges();
    }

    private void OnGUI()
    {
        if (updateUI)
        {
            updateUI = false;
            if (passives[updatePos] != null)
            {
                var quality = passives[updatePos].quality;
                var postfix = passives[updatePos].bonusQuality;
                var postfixProperty = passives[updatePos].bonusProperty;
                var hasPostfix = passives[updatePos].bonusBaseStat != 0;
                var itemName = passives[updatePos].itemName;
                var statsText = passives[updatePos].statsText;

                var dir = Direction.up;

                switch (updatePos)
                {
                    case 0:
                        {
                            dir = Direction.left;
                            break;
                        }
                    case 1:
                        {
                            dir = Direction.down;
                            break;
                        }
                    case 2:
                        {
                            dir = Direction.right;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                UIManager.instance.ShowInventoryTooltip(pc.playerID, pc.playstationController, dir, quality, postfix, postfixProperty, hasPostfix, itemName, statsText);
            }
        }

        if (destroy)
        {
            UIManager.instance.SetInventoryTooltipDestroySlider(pc.playerID, currentDestroyTime);

            if (1f <= currentDestroyTime)
            {
                RemoveItem(updatePos);
                currentDestroyTime = 0f;
                destroy = false;
                UIManager.instance.HideInventoryTooltip(pc.playerID);

                switch (updatePos)
                {
                    case 0:
                        {
                            UIManager.instance.DestroyPassiveItemLeft(pc.playerID);
                            break;
                        }
                    case 1:
                        {
                            UIManager.instance.DestroyPassiveItemDown(pc.playerID);
                            break;
                        }
                    case 2:
                        {
                            UIManager.instance.DestroyPassiveItemRight(pc.playerID);
                            break;
                        }
                    case 3:
                        {
                            UIManager.instance.DestroyPassiveItemUp(pc.playerID);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }
    }

    private void SetCharges()
    {
        healthPotionCounterText.text = healthPotions.ToString();
    }

    private int? GetNextFreeSlot()
    {
        for (var i = 0; i < passives.Length; i++)
        {
            if (passives[i] == null)
            {
                return i;
            }
        }
        return null;
    }
}
