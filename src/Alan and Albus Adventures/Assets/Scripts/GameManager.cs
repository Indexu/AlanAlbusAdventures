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
    public float floorTransitionTime;
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

    // Loot
    public float dropChance;
    // Please make these \/ add up to 1
    public float commonChance;
    public float rareChance;
    public float epicChance;
    public float legendaryChance;
    // End ----------------------------
    public GameObject[] maxHealthItems;
    public GameObject[] movementSpeedItems;
    public GameObject[] attackSpeedItems;
    public GameObject[] criticalChanceItems;
    public GameObject[] criticalDamageItems;
    public GameObject[] baseDamageItems;

    private FloorManager floorManager;
    private Vector3 newRoom;
    private Vector3 oldRoom;
    private Vector3 vector;
    private Transform door;
    private Direction direction;
    private GameObject currentRoom;
    private int enemies;
    private int killed;
    private List<DoorController> doors;
    private bool bossFight;
    private int deadPlayers;
    private Stats[] playerStats;
    private float currentFloorTransitionTime;
    private bool changingFloors;
    private const float statPointUpOffset = 150f;

    public void NextFloor()
    {
        GameManager.instance.changingRooms = true;
        GameManager.instance.changingFloors = true;
        UIManager.instance.HideAllTooltips();
        UIManager.instance.ClearDoorButtons();
        UIManager.instance.SetTransitionText("Floor " + floorManager.floorLevel);

        StartCoroutine(FadeTransitionIn());
    }

    public void ChangeRooms(GameObject room, Transform door, Direction dir, bool boss)
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
        GameManager.instance.killed++;

        GameManager.instance.AddExperience(xp);

        if (GameManager.instance.enemies == 0)
        {
            inCombat = false;
            UnlockDoors();

            DropLoot(GameManager.instance.killed, false);

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
        //SoundManager.instance.Destroy();
        //1UIManager.instance.Destroy();     
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

                UIManager.instance.DisplayStatUpText(player.transform.position, 150f);
            }

            currentExperience = currentExperience % maxExperience;
        }

        UIManager.instance.SetExperienceBar(currentExperience, maxExperience);
    }

    private void Awake()
    {
        Time.timeScale = 1f;
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
    }

    private void Start()
    {
        UIManager.instance.SetExperienceBar(currentExperience, maxExperience);
    }

    private void Update()
    {
        if (changingRooms && !changingFloors)
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
        currentFloorTransitionTime = 0f;

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
        killed = 0;
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
            UIManager.instance.DisplayStatUpText(player.transform.position, 150f);
        }

        currentRoom.transform.Find("Hole").gameObject.SetActive(true);
    }

    private bool DidLootDrop()
    {
        return Random.Range(0f, 1f) <= dropChance;
    }

    private Quality GetQualityOfItem()
    {
        var quality = Random.Range(0f, 1f);
        if (quality <= legendaryChance)
        {
            return Quality.LEGENDARY;
        }
        else if (quality <= legendaryChance + epicChance)
        {
            return Quality.EPIC;
        }
        else if (quality <= legendaryChance + epicChance + rareChance)
        {
            return Quality.RARE;
        }
        else
        {
            return Quality.COMMON;
        }
    }

    private Postfix GetPostfixOfItem()
    {
        var quality = Random.Range(0f, 1f);
        if (quality <= legendaryChance)
        {
            return Postfix.MAJOR;
        }
        else if (quality <= legendaryChance + epicChance)
        {
            return Postfix.SUPERIOR;
        }
        else if (quality <= legendaryChance + epicChance + rareChance)
        {
            return Postfix.LESSER;
        }
        else
        {
            return Postfix.MINOR;
        }
    }

    private Property GetPropertyOfItem()
    {
        var property = Random.Range(0f, 1.5f);
        if (property <= 0.25f)
        {
            return Property.MAXHEALTH;
        }
        else if (property <= 0.5f)
        {
            return Property.MOVEMENTSPEED;
        }
        else if (property <= 0.75f)
        {
            return Property.ATTACKSPEED;
        }
        else if (property <= 1f)
        {
            return Property.CRITCHANCE;
        }
        else if (property <= 1.25f)
        {
            return Property.CRITDAMAGE;
        }
        else
        {
            return Property.BASEDAMAGE;
        }
    }

    public void DropLoot(int rolls, bool chest)
    {
        for (int i = 0; i < rolls; i++)
        {
            if (DidLootDrop() || chest || true)
            {
                var property = GetPropertyOfItem();
                GameObject item;
                GameObject[] arr = null;
                switch (property)
                {
                    case Property.MAXHEALTH:
                        arr = maxHealthItems;
                        break;
                    case Property.MOVEMENTSPEED:
                        arr = movementSpeedItems;
                        break;
                    case Property.ATTACKSPEED:
                        arr = attackSpeedItems;
                        break;
                    case Property.CRITCHANCE:
                        arr = criticalChanceItems;
                        break;
                    case Property.CRITDAMAGE:
                        arr = criticalDamageItems;
                        break;
                    case Property.BASEDAMAGE:
                        arr = baseDamageItems;
                        break;
                };
                item = Instantiate(arr[(Random.Range(0, arr.Length))], GameManager.instance.currentRoom.transform.position, Quaternion.identity, GameManager.instance.currentRoom.transform);
                var itemComponent = item.GetComponent<Item>();
                itemComponent.quality = GetQualityOfItem();
                for (int j = 0; j < (int)itemComponent.quality; j++)
                {
                    if (DidLootDrop())
                    {
                        itemComponent.bonusProperty = GetPropertyOfItem();
                        switch (itemComponent.bonusProperty)
                        {
                            case Property.MAXHEALTH:
                                itemComponent.bonusBaseStat = 10;
                                break;
                            case Property.MOVEMENTSPEED:
                                itemComponent.bonusBaseStat = 15;
                                break;
                            case Property.ATTACKSPEED:
                                itemComponent.bonusBaseStat = 10;
                                break;
                            case Property.CRITCHANCE:
                                itemComponent.bonusBaseStat = 5;
                                break;
                            case Property.CRITDAMAGE:
                                itemComponent.bonusBaseStat = 10;
                                break;
                            case Property.BASEDAMAGE:
                                itemComponent.bonusBaseStat = 15;
                                break;
                        };
                        itemComponent.bonusQuality = GetPostfixOfItem();
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator FadeTransitionIn()
    {
        currentFloorTransitionTime = 0f;
        var alpha = currentFloorTransitionTime / floorTransitionTime;
        while (alpha < 1f)
        {
            currentFloorTransitionTime += Time.deltaTime;
            alpha = currentFloorTransitionTime / floorTransitionTime;
            UIManager.instance.SetTransitionAlpha(alpha);

            yield return null;
        }

        GameManager.instance.floorManager.GenerateFloor();

        yield return new WaitForSeconds(floorTransitionTime);

        yield return StartCoroutine(FadeTransitionOut());
    }

    private IEnumerator FadeTransitionOut()
    {
        currentFloorTransitionTime = floorTransitionTime;
        var alpha = currentFloorTransitionTime / floorTransitionTime;
        while (0f < alpha)
        {
            currentFloorTransitionTime -= Time.deltaTime;
            alpha = currentFloorTransitionTime / floorTransitionTime;
            UIManager.instance.SetTransitionAlpha(alpha);

            yield return null;
        }

        GameManager.instance.changingRooms = false;
        GameManager.instance.changingFloors = false;
    }
}
