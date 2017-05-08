using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    PASSIVE,
    ACTIVE
};

public enum Quality
{
    COMMON    = 1,
    RARE      = 2,
    EPIC      = 3,
    LEGENDARY = 4
};

public enum Property
{
    MAXHEALTH     = 0,
    MOVEMENTSPEED = 1,
    ATTACKSPEED   = 2,
    CRITCHANCE    = 3,
    CRITDAMAGE    = 4,
    BASEDAMAGE    = 5
};

public enum Postfix
{
    MINOR    = 1,
    LESSER   = 2,
    SUPERIOR = 3,
    MAJOR    = 4
}

public abstract class Item : MonoBehaviour
{
    // Item type
    public ItemType itemType;

    // Standard stats
    public Quality  quality;
    public Property property;
    public int      baseStat;

    // Bonus stats
    public Postfix  bonusQuality;
    public Property bonusProperty;
    public int      bonusBaseStat;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject;
        if (player.tag == "Player")
        {
            player.GetComponent<Inventory>().AddItem(gameObject);
            // TODO: Move item to item slot.
        }
    }
}
