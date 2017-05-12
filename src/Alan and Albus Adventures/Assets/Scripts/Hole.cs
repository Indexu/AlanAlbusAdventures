using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private Behaviour halo;
    private int playersAdjacent;

    private void Start()
    {
        playersAdjacent = 0;
        halo = (Behaviour)GetComponent("Halo");
        halo.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            playersAdjacent++;

            if (halo == null)
            {
                halo = (Behaviour)GetComponent("Halo");
            }

            halo.enabled = true;

            var ps4 = collider.GetComponent<PlayerController>().playstationController;
            UIManager.instance.ShowHoleButton(transform.position, ps4);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            playersAdjacent--;
            UIManager.instance.HideHoleButton();
            if (playersAdjacent == 0)
            {
                halo.enabled = false;
            }
        }
    }
}
