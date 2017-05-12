using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{

    public int charges;

    private bool player1Enter;
    private bool player2Enter;
    private const float yOffset = -125f;
    private GameObject button;

    public void PickedUp()
    {
        GameObject.Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            GameObject.Destroy(button);
        }
    }

    private void OnGUI()
    {
        if (button != null)
        {
            UIManager.instance.MoveUIElement(button, transform.position, yOffset);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var pc = collision.gameObject.GetComponent<PlayerController>();
            int playerID = pc.playerID;
            if (playerID == 0)
            {
                player1Enter = true;
            }
            else
            {
                player2Enter = true;
            }

            if (button != null)
            {
                GameObject.Destroy(button);
            }

            button = UIManager.instance.CreateAndShowButton(transform.position, yOffset, Direction.down, pc.playstationController);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            int playerID = collision.gameObject.GetComponent<PlayerController>().playerID;
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
                GameObject.Destroy(button);
            }
        }
    }
}
