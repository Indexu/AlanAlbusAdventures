using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Quality
{
    COMMON = 1,
    RARE = 2,
    EPIC = 3,
    LEGENDARY = 4
};

public enum Property
{
    MAXHEALTH = 0,
    MOVEMENTSPEED = 1,
    ATTACKSPEED = 2,
    CRITCHANCE = 3,
    CRITDAMAGE = 4,
    BASEDAMAGE = 5
};

public enum Postfix
{
    MINOR = 1,
    LESSER = 2,
    SUPERIOR = 3,
    MAJOR = 4
}

public class Item : MonoBehaviour
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
    public bool hasPostfix;
    public string statsText;

    private bool player1Enter;
    private bool player2Enter;
    private GameObject tooltip;
    private const float yOffset = 150f;

    public static string PropertyToString(Property prop)
    {
        switch (prop)
        {
            case Property.ATTACKSPEED:
                {
                    return "Attack Speed";
                }
            case Property.BASEDAMAGE:
                {
                    return "Damage";
                }
            case Property.CRITCHANCE:
                {
                    return "Critical Chance";
                }
            case Property.CRITDAMAGE:
                {
                    return "Critical Damage";
                }
            case Property.MAXHEALTH:
                {
                    return "Health";
                }
            case Property.MOVEMENTSPEED:
                {
                    return "Movement Speed";
                }
            default:
                return string.Empty;
        }
    }

    public void PickedUp()
    {
        GameObject.Destroy(gameObject);
    }

    private void OnGUI()
    {
        if (tooltip != null)
        {
            UIManager.instance.MoveUIElement(tooltip, transform.position, yOffset);
        }
    }

    private void OnDestroy()
    {
        if (tooltip != null)
        {
            GameObject.Destroy(tooltip);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

            if (tooltip != null)
            {
                GameObject.Destroy(tooltip);
            }

            if (string.IsNullOrEmpty(statsText))
            {
                CreateStatText();
            }

            tooltip = UIManager.instance.CreateAndShowTooltip(transform.position, yOffset, quality, bonusQuality, bonusProperty, hasPostfix, itemName, statsText);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
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
                GameObject.Destroy(tooltip);
            }
        }
    }

    private void CreateStatText()
    {
        hasPostfix = (bonusBaseStat != 0);

        statsText = "+" + (int)quality + "% to " + Item.PropertyToString(property);

        if (hasPostfix)
        {
            statsText += "\n";
            statsText += "+" + (int)bonusQuality + "% to " + Item.PropertyToString(bonusProperty);
        }
    }
}
