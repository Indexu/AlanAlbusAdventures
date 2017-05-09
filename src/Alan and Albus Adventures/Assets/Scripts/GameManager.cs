using System.Collections;
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
    public Text winText;
    public GameObject pauseScreen;
    public bool isPaused;
    public bool changingRooms;
    public GameObject[] players;
    public Camera mainCamera;
    public GameObject canvas;
    public RectTransform canvasRect;

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

    public void changeRooms(GameObject room, Transform door, Direction dir, bool boss)
    {
        changingRooms = true;

        bossFight = boss;
        currentRoom = room;
        oldRoom = mainCamera.transform.position;
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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        floorManager = GameObject.FindGameObjectWithTag("FloorManager").GetComponent<FloorManager>();
        players = GameObject.FindGameObjectsWithTag("Player");
        bossUI = GameObject.FindGameObjectWithTag("BossUI");
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvasRect = canvas.GetComponent<RectTransform>();
        changingRooms = false;
        doors = new List<DoorController>();
        bossUI.SetActive(false);
        winText.gameObject.SetActive(false);
        pauseScreen.SetActive(false);
        isPaused = false;

        floorManager.Init();
        floorManager.GenerateFloor();
    }

    private void Update()
    {
        if (changingRooms)
        {
            mainCamera.transform.Translate(vector * cameraSpeed * Time.deltaTime);

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

    private void endTransition()
    {
        mainCamera.transform.position = new Vector3(newRoom.x, newRoom.y, vector.z);
        changingRooms = false;

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
                child.gameObject.GetComponent<VitalityController>().healthSlider.gameObject.SetActive(true);
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
