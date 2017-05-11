using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class Item : MonoBehaviour
{
    // Standard stats
    public Quality  quality;
    public Property property;
    public int      baseStat;

    // Bonus stats
    public Postfix  bonusQuality;
    public Property bonusProperty;
    public int      bonusBaseStat;

    private bool player1Enter;
    private bool player2Enter;

    public void PickedUp()
    {
        GameObject.Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var tag = collision.gameObject.tag;
        if (tag == "Player")
        {
            var player = collision.gameObject;
            var playerID = player.GetComponent<PlayerController>().playerID;
            if (playerID == 0)
            {
                player1Enter = true;
            }
            else
            {
                player2Enter = true;
            }
            // Display Tooltip
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        var tag = collision.gameObject.tag;
        if (tag == "Player")
        {
            var playerID = collision.gameObject.GetComponent<PlayerController>().playerID;
            if (playerID == 0)
            {
                player1Enter = false;
            }
            else
            {
                player2Enter = false;
            }

            if (!player1Enter && !player2Enter)
            {
                // Hide Tooltip
            }
        }
    }
}
