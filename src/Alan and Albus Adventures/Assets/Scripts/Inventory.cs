using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int passiveItemCount = 4;
    private const int activeItemCount = 2;
    private const int statsCount = 6;

    public GameObject[] passives = new GameObject[passiveItemCount];
    public GameObject[]  actives = new GameObject[activeItemCount];

    public int[] bonusStats = new int[statsCount]
    {
        100, 100, 100, 100, 100, 100
    };

    public void AddItem(GameObject gameObject)
    {
        var item = gameObject.GetComponent<Item>();
        if (item != null)
        {
            Debug.Log(item);
            if (item.itemType == ItemType.PASSIVE)
            {
                var pos = GetNextFreeSlot(item.itemType);
                if (pos.HasValue)
                {
                    passives[pos.Value] = gameObject;
                    bonusStats[(int)item.property] += ((int)item.quality * item.baseStat);
                    if (item.bonusQuality != 0 && item.bonusBaseStat != 0)
                    {
                        bonusStats[(int)item.bonusProperty] += ((int)item.bonusQuality * item.bonusBaseStat);
                    }
                }
            }
            else if (item.itemType == ItemType.ACTIVE)
            {
                var pos = GetNextFreeSlot(item.itemType);
                if (pos.HasValue)
                {
                    actives[pos.Value] = gameObject;
                }
            }
        }
    }

    public void RemoveItem(int pos)
    {
        if (0 <= pos && pos < passiveItemCount)
        {
            GameObject discardObject = passives[pos];
            Item discardItem = discardObject.GetComponent<Item>();
            bonusStats[(int)discardItem.property] -= ((int)discardItem.quality * discardItem.baseStat);
            if (discardItem.bonusQuality != 0 && discardItem.bonusBaseStat != 0)
            {
                bonusStats[(int)discardItem.bonusProperty] -= ((int)discardItem.bonusQuality * discardItem.bonusBaseStat);
            }
            passives[pos] = null;
        }
    }

    public void UseActive(bool isFirstItem)
    {
        var item = (isFirstItem ? actives[0] : actives[1]).GetComponent<ActiveItem>();
        if (item != null)
        {
            bonusStats[(int)item.property] += ((int)item.quality * item.baseStat);
            if (item.bonusQuality != 0 && item.bonusBaseStat != 0)
            {
                bonusStats[(int)item.bonusProperty] += ((int)item.bonusQuality * item.bonusBaseStat);
            }
            StartCoroutine(item.useItem());
            bonusStats[(int)item.property] -= ((int)item.quality * item.baseStat);
            if (item.bonusQuality != 0 && item.bonusBaseStat != 0)
            {
                bonusStats[(int)item.bonusProperty] -= ((int)item.bonusQuality * item.bonusBaseStat);
            }
            if (item.charges <= 0)
            {
                if (isFirstItem)
                {
                    GameObject.Destroy(actives[0]);
                    actives[0] = null;
                }
                else
                {
                    GameObject.Destroy(actives[1]);
                    actives[1] = null;
                }
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
