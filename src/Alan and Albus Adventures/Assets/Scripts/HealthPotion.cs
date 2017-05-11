using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour {

    public int charges;

    private bool player1Enter;
    private bool player2Enter;

    public void PickedUp()
    {
        GameObject.Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            int playerID = collision.gameObject.GetComponent<PlayerController>().playerID;
            if (playerID == 0)
            {
                player1Enter = true;
            }
            else
            {
                player2Enter = true;
            }
            // Display x button
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
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
                // Hide x button
            }
        }
    }
}
