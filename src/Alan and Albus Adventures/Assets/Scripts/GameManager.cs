﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public float cameraSpeed;
    public GameObject bossUI;
    public GameObject pauseScreen;
    public bool isPaused;
    public bool changingRooms;
    public GameObject[] players;
    public Camera mainCamera;
    public GameObject canvas;
    public RectTransform canvasRect;
    public bool inCombat;
    public float timeBeforeAttacking;
    public float maxExperience;
    public float currentExperience;

    private FloorManager floorManager;
    private Vector3 newRoom;
    private Vector3 oldRoom;
    private Vector3 vector;
    private Transform door;
    private Direction direction;
    private GameObject currentRoom;
    private int enemies;
    private List<DoorController> doors;
    private bool bossFight;
    private int deadPlayers;
    private Stats[] playerStats;

    public void changeRooms(GameObject room, Transform door, Direction dir, bool boss)
    {
        GameManager.instance.changingRooms = true;

        GameManager.instance.bossFight = boss;
        GameManager.instance.currentRoom = room;
        GameManager.instance.oldRoom = mainCamera.transform.position;
        GameManager.instance.newRoom = room.transform.position;
        GameManager.instance.door = door;
        GameManager.instance.direction = dir;

        GameManager.instance.vector = newRoom - oldRoom;
        GameManager.instance.vector.z = -10f;

        UIManager.instance.ClearDoorButtons();

        var playersLayer = LayerMask.NameToLayer("Players");
        var doorsLayer = LayerMask.NameToLayer("DoorTriggers");
        Physics2D.IgnoreLayerCollision(playersLayer, doorsLayer, true);

        foreach (var player in players)
        {
            player.GetComponent<PlayerController>().door = null;
        }
    }

    public void EnemyKilled(float xp)
    {
        GameManager.instance.enemies--;

        GameManager.instance.AddExperience(xp);

        if (GameManager.instance.enemies == 0)
        {
            inCombat = false;
            UnlockDoors();

            if (GameManager.instance.bossFight)
            {
                EndBossFight();
            }
        }
    }

    public void PlayerKilled()
    {
        GameManager.instance.deadPlayers++;

        if (GameManager.instance.deadPlayers == 2)
        {
            Pause();
        }
    }

    public void Pause()
    {
        GameManager.instance.isPaused = true;
        Time.timeScale = 0f;
        GameManager.instance.pauseScreen.SetActive(true);
    }

    public void Unpause()
    {
        GameManager.instance.isPaused = false;
        Time.timeScale = 1f;
        GameManager.instance.pauseScreen.SetActive(false);
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

    public void AddExperience(float amount)
    {
        currentExperience += amount;

        if (maxExperience <= currentExperience)
        {
            foreach (var player in playerStats)
            {
                player.statPoints++;
            }

            currentExperience = currentExperience % maxExperience;
        }

        UIManager.instance.SetExperienceBar(currentExperience, maxExperience);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GameManager.instance.Init();
        }
        else if (instance != this)
        {
            GameManager.instance.Init();
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UIManager.instance.SetExperienceBar(currentExperience, maxExperience);
    }

    private void Update()
    {
        if (changingRooms)
        {
            GameManager.instance.mainCamera.transform.Translate(vector * cameraSpeed * Time.deltaTime);

            if (direction == Direction.up && newRoom.y < mainCamera.transform.position.y)
            {
                endTransition();
            }
            else if (direction == Direction.down && mainCamera.transform.position.y < newRoom.y)
            {
                endTransition();
            }
            else if (direction == Direction.right && newRoom.x < mainCamera.transform.position.x)
            {
                endTransition();
            }

            else if (direction == Direction.left && mainCamera.transform.position.x < newRoom.x)
            {
                endTransition();
            }
        }
    }

    private void Init()
    {
        GameManager.instance.mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        GameManager.instance.floorManager = GameObject.FindGameObjectWithTag("FloorManager").GetComponent<FloorManager>();
        GameManager.instance.players = GameObject.FindGameObjectsWithTag("Player");
        GameManager.instance.bossUI = GameObject.FindGameObjectWithTag("BossUI");
        GameManager.instance.canvas = GameObject.FindGameObjectWithTag("Canvas");
        GameManager.instance.canvasRect = canvas.GetComponent<RectTransform>();
        GameManager.instance.pauseScreen = GameManager.instance.canvas.transform.Find("PauseScreen").gameObject;
        GameManager.instance.changingRooms = false;
        GameManager.instance.doors = new List<DoorController>();
        GameManager.instance.bossUI.SetActive(false);
        GameManager.instance.pauseScreen.SetActive(false);
        GameManager.instance.isPaused = false;

        playerStats = new Stats[GameManager.instance.players.Length];
        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            playerStats[i] = players[i].GetComponent<Stats>();
        }

        GameManager.instance.floorManager.GenerateFloor();
    }

    private void endTransition()
    {
        GameManager.instance.mainCamera.transform.position = new Vector3(newRoom.x, newRoom.y, vector.z);

        var spawns = new List<Vector3>();
        foreach (Transform child in door)
        {
            if (child.name == "Spawn")
            {
                spawns.Add(child.position);
            }
        }

        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = spawns[i];
        }

        StartCoroutine(ActivateEnemies());
        GameManager.instance.changingRooms = false;
    }

    private IEnumerator ActivateEnemies()
    {
        GameManager.instance.doors.Clear();

        var enemyList = new List<Enemy>();
        var children = currentRoom.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.gameObject.tag == "Enemy")
            {
                enemyList.Add(child.gameObject.GetComponent<Enemy>());
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
            inCombat = true;
            LockDoors();

            if (bossFight)
            {
                StartBossFight();
            }

            yield return new WaitForSeconds(timeBeforeAttacking);
            foreach (var enemy in enemyList)
            {
                enemy.Attacking = true;
                enemy.gameObject.GetComponent<VitalityController>().healthSlider.gameObject.SetActive(true);
            }
        }

        var playersLayer = LayerMask.NameToLayer("Players");
        var doorsLayer = LayerMask.NameToLayer("DoorTriggers");
        Physics2D.IgnoreLayerCollision(playersLayer, doorsLayer, false);
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
        GameManager.instance.bossUI.SetActive(true);
    }

    private void EndBossFight()
    {
        GameManager.instance.bossFight = false;
        GameManager.instance.bossUI.SetActive(false);

        foreach (var player in playerStats)
        {
            player.statPoints++;
        }
    }
}
