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
			gameManager.changeRooms(connectedRoom, connectedDoor.position, direction, leadsToBoss);
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

	private void Start() 
	{
		playersAdjacent = 0;
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		halo = (Behaviour)GetComponent("Halo");
		halo.enabled = false;
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (!locked && collider.gameObject.tag == "Player")
		{
			playersAdjacent++;
			halo.enabled = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		if (!locked && collider.gameObject.tag == "Player")
		{
			playersAdjacent--;

			if (playersAdjacent == 0)
			{
				halo.enabled = false;
			}
		}
	}
}
