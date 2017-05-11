using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    up, down, left, right
}

public class DoorController : MonoBehaviour
{
    public bool locked;
    public bool leadsToBoss;
    public GameObject connectedRoom;
    public Transform connectedDoor;
    public Direction direction;
    public int requiredPlayers;

    private int playersAdjacent;
    private GameManager gameManager;
    private Behaviour halo;

    public void GoThrough()
    {
        if (playersAdjacent == requiredPlayers)
        {
            playersAdjacent = 0;
            GameManager.instance.changeRooms(connectedRoom, connectedDoor, direction, leadsToBoss);
            halo.enabled = false;
        }
    }

    public void Lock()
    {
        locked = true;
        gameObject.SetActive(false);
    }

    public void Unlock()
    {
        locked = false;
        gameObject.SetActive(true);
    }

    public void EnterRange()
    {
        playersAdjacent++;
        halo.enabled = true;
    }

    public void ExitRange()
    {
        playersAdjacent--;
        if (playersAdjacent == 0)
        {
            halo.enabled = false;
        }
    }

    private void Start()
    {
        playersAdjacent = 0;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        halo = (Behaviour)GetComponent("Halo");
        halo.enabled = false;
    }
}
