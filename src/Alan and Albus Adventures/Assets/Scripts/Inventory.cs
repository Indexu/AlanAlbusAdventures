using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int passiveItemCount = 4;
    private const int activeItemCount = 2;
    private const int statsCount = 6;

    private PassiveItem[] passives = new PassiveItem[passiveItemCount];
    private ActiveItem[] actives = new ActiveItem[activeItemCount];

    private Property[] bonusStats = new Property[statsCount]
    {
        (Property)100, (Property)100, (Property)100, (Property)100, (Property)100, (Property)100
    };

    public void AddItem(Item item)
    {
        if (item != null)
        {
            if (item.itemType == ItemType.PASSIVE)
            {
                var pos = GetNextFreeSlot(item.itemType);
                if (pos.HasValue)
                {
                    passives[pos.Value] = (PassiveItem)item;
                    bonusStats[(int)item.property] += ((int)item.quality * item.baseStat);
                    if (item.bonusQuality.HasValue && item.bonusProperty.HasValue && item.bonusBaseStat.HasValue)
                    {
                        bonusStats[(int)item.bonusProperty.Value] += ((int)item.bonusQuality.Value * item.bonusBaseStat.Value);
                    }
                }
            }
            else if (item.itemType == ItemType.ACTIVE)
            {
                var pos = GetNextFreeSlot(item.itemType);
                if (pos.HasValue)
                {
                    actives[pos.Value] = (ActiveItem)item;
                }
            }
        }
    }

    public Item RemoveItem(int pos)
    {
        if (0 <= pos && pos < passiveItemCount)
        {
            Item discard = passives[pos];
            bonusStats[(int)discard.property] -= ((int)discard.quality * discard.baseStat);
            if (discard.bonusQuality.HasValue && discard.bonusProperty.HasValue && discard.bonusBaseStat.HasValue)
            {
                bonusStats[(int)discard.bonusProperty.Value] -= ((int)discard.bonusQuality.Value * discard.bonusBaseStat.Value);
            }
            passives[pos] = null;
            return discard;
        }
        return null;
    }

    public void UseActive(bool isFirstItem)
    {
        var item = (isFirstItem ? actives[0] : actives[1]);
        if (item != null)
        {
            bonusStats[(int)item.property] += ((int)item.quality * item.baseStat);
            if (item.bonusQuality.HasValue && item.bonusProperty.HasValue && item.bonusBaseStat.HasValue)
            {
                bonusStats[(int)item.bonusProperty.Value] += ((int)item.bonusQuality.Value * item.bonusBaseStat.Value);
            }
            StartCoroutine(item.useItem());
            bonusStats[(int)item.property] -= ((int)item.quality * item.baseStat);
            if (item.bonusQuality.HasValue && item.bonusProperty.HasValue && item.bonusBaseStat.HasValue)
            {
                bonusStats[(int)item.bonusProperty.Value] -= ((int)item.bonusQuality.Value * item.bonusBaseStat.Value);
            }
        }
    }

    public float GetStatBonus(Property property)
    {
        return ((float)bonusStats[(int)property] / 100f);
    }

    private int? GetNextFreeSlot(ItemType itemType)
    {
        if (itemType == ItemType.PASSIVE)
        {
            for (var i = 0; i < passives.Length; i++)
            {
                if (passives[i] == null)
                {
                    return i;
                }
            }
        }
        else if (itemType == ItemType.ACTIVE)
        {
            for (var i = 0; i < actives.Length; i++)
            {
                if (actives[i] == null)
                {
                    return i;
                }
            }
        }
        return null;
    }
}
