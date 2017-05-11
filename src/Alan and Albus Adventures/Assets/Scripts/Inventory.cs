using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public void UseHealthPotion()
    {
        if (healthPotions > 0)
        {
            healthPotions--;
            vc.Heal(healAmount);
        }
    }

    public float GetStatBonus(Property property)
    {
        return ((float)bonusStats[(int)property] / 100f);
    }

    private void Start()
    {
        vc = GetComponent<VitalityController>();
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
