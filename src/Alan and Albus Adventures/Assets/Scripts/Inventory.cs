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
    private Text healthPotionCounterText;

    public void AddItem(Item item, int pos)
    {
        if (item != null)
        {
            if (0 <= pos && pos < passiveItemCount)
            {
                passives[pos] = new InventoryItem
                {
                    quality = item.quality,
                    property = item.property,
                    baseStat = item.baseStat,
                    bonusQuality = item.bonusQuality,
                    bonusProperty = item.bonusProperty,
                    bonusBaseStat = item.bonusBaseStat
                };
                bonusStats[(int)item.property] += ((int)item.quality * item.baseStat);
                if (item.bonusQuality != 0 && item.bonusBaseStat != 0)
                {
                    bonusStats[(int)item.bonusProperty] += ((int)item.bonusQuality * item.bonusBaseStat);
                }
            }
        }
    }

    public void RemoveItem(int pos)
    {
        if (0 <= pos && pos < passiveItemCount)
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

    private void Start()
    {
        vc = GetComponent<VitalityController>();

        var playerID = GetComponent<PlayerController>().playerID;
        var searchString = (playerID == 0 ? "AlanItemSlots" : "AlbusItemSlots");
        healthPotionCounterText = UIManager.instance.canvas.transform.Find(searchString).Find("UseItemSlot").Find("UsesFrame").Find("Text").GetComponent<Text>();

        SetCharges();
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
