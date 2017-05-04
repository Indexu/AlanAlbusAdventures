using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

public class GameManager : MonoBehaviour 
{
	public float cameraSpeed;
	public GameObject bossUI;
	public Text winText;
	public GameObject pauseScreen;
	public bool isPaused;

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
	private bool bossFight;
	private int deadPlayers;

	public void changeRooms(GameObject room, Vector3 door, Direction dir, bool boss)
	{
		changingRooms = true;

		bossFight = boss;
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

			if (bossFight)
			{
				EndBossFight();
			}
		}
	}

	public void PlayerKilled()
	{
		deadPlayers++;

		if (deadPlayers == 2)
		{
			Pause();
		}
	}

	public void Pause()
	{
		isPaused = true;
		Time.timeScale = 0f;
		pauseScreen.SetActive(true);
	}

	public void Unpause()
	{
		isPaused = false;
		Time.timeScale = 1f;
		pauseScreen.SetActive(false);
	}

	public void Reset()
	{
		Unpause();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void DeadReset()
	{
		if (deadPlayers == 2)
		{
			Reset();
		}
	}

	private void Start()
	{
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
		players = GameObject.FindGameObjectsWithTag("Player");
		changingRooms = false;
		doors = new List<DoorController>();
		bossUI.SetActive(false);
		winText.gameObject.SetActive(false);
		pauseScreen.SetActive(false);
		isPaused = false;
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
				child.gameObject.GetComponent<Enemy>().Attacking = true;
				enemies++;

				if (bossFight)
				{
					var vc = child.gameObject.GetComponent<VitalityController>();
					if (vc.boss)
					{
						vc.doUpdateUI = true;
					}
				}
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

			if (bossFight)
			{
				StartBossFight();
			}
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

	private void StartBossFight()
	{
		bossUI.SetActive(true);
	}

	private void EndBossFight()
	{
		bossFight = false;
		bossUI.SetActive(false);
		winText.gameObject.SetActive(true);
	}
}
