using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GameManager : MonoBehaviour 
{
	public float cameraSpeed;

	private Transform mainCamera;
	private bool changingRooms;
	private Vector3 newRoom;
	private Vector3 oldRoom;
	private Vector3 vector;
	private Vector3 door;
	private Direction direction;
	private GameObject[] players;
	private GameObject currentRoom;
	private int enemies;
	private List<DoorController> doors;

	public void changeRooms(GameObject room, Vector3 door, Direction dir)
	{
		changingRooms = true;

		currentRoom = room;
		oldRoom = mainCamera.position;
		newRoom = room.transform.position;
		this.door = door;
		direction = dir;

		vector = newRoom - oldRoom;
		vector.z = -10f;
	}

	public void EnemyKilled()
	{
		enemies--;

		if (enemies == 0)
		{
			UnlockDoors();
		}
	}

	private void Start()
	{
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
		players = GameObject.FindGameObjectsWithTag("Player");
		changingRooms = false;
		doors = new List<DoorController>();
	}

	private void Update()
	{
		if (changingRooms)
		{
			mainCamera.Translate(vector * cameraSpeed * Time.deltaTime);

			if (direction == Direction.up && newRoom.y < mainCamera.position.y)
			{
				endTransition();
			}
			else if (direction == Direction.down && mainCamera.position.y < newRoom.y)
			{
				endTransition();
			}
			else if (direction == Direction.right && newRoom.x < mainCamera.position.x)
			{
				endTransition();
			}

			else if (direction == Direction.left && mainCamera.position.x < newRoom.x)
			{
				endTransition();
			}
		}
	}

	private void endTransition()
	{
		mainCamera.position = new Vector3(newRoom.x, newRoom.y , vector.z);
		changingRooms = false;

		foreach (GameObject player in players)
		{
			player.transform.position = door;
		}

		ActivateEnemies();
	}

	private void ActivateEnemies()
	{
		doors.Clear();

		var children = currentRoom.GetComponentsInChildren<Transform>();
		foreach (var child in children)
		{
			if (child.gameObject.tag == "Enemy")
			{
				child.gameObject.SetActive(true);
				enemies++;
			}
			else if (child.gameObject.tag == "Door")
			{
				var door = child.gameObject.GetComponent<DoorController>();
				doors.Add(door);
			}
		}

		if (enemies != 0)
		{
			LockDoors();
		}
	}

	private void LockDoors()
	{
		foreach (var door in doors)
		{
			door.Lock();
		}
	}

	private void UnlockDoors()
	{
		foreach (var door in doors)
		{
			door.Unlock();
		}
	}
}
