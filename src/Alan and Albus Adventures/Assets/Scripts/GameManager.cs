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

	public void changeRooms(Vector3 room, Vector3 door, Direction dir)
	{
		changingRooms = true;

		oldRoom = mainCamera.position;
		newRoom = room;
		this.door = door;
		direction = dir;

		vector = newRoom - oldRoom;
		vector.z = -10f;
	}

	private void Start()
	{
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
		players = GameObject.FindGameObjectsWithTag("Player");
		changingRooms = false;
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
	}
}
