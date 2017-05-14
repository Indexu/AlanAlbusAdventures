using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;
using UnityEngine.EventSystems;
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public float cameraSpeed;
    public float floorTransitionTime;
    public GameObject bossUI;
    public GameObject pauseScreen;
    public GameObject gameOverScreen;
    public GameObject firstSelectedPauseScreen;
    public GameObject firstSelectedGameOverScreen;
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
    public List<GameObject> AlanItemList;
    public List<GameObject> AlbusItemList;
    public float dropChance;
    public float healthPotionChance;
    // Please make these \/ add up to 1
    public float commonChance;
    public float rareChance;
    public float epicChance;
    public float legendaryChance;
    // End ----------------------------
    public GameObject healthPotion;
    public GameObject[] maxHealthItems;
    public GameObject[] movementSpeedItems;
    public GameObject[] attackSpeedItems;
    public GameObject[] criticalChanceItems;
    public GameObject[] criticalDamageItems;
    public GameObject[] baseDamageItems;
    public EventSystem eventSystem;

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
    private float currentFloorTransitionTime;
    private bool changingFloors;
    private const float statPointUpOffset = 150f;

    public void NextFloor()
    {
        GameManager.instance.enemies = 0;
        GameManager.instance.changingRooms = true;
        GameManager.instance.changingFloors = true;
        UIManager.instance.HideAllTooltips();
        UIManager.instance.ClearDoorButtons();
        UIManager.instance.SetTransitionText("Floor " + floorManager.floorLevel);

        foreach (var player in players)
        {
            player.GetComponent<Stats>().HideStats();
            player.GetComponent<PlayerController>().inStatsScreen = false;
        }

        StartCoroutine(FadeTransitionIn());
    }

    public void ChangeRooms(GameObject room, Transform door, Direction dir, bool boss)
    {
        GameManager.instance.changingRooms = true;
        GameManager.instance.enemies = 0;

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
            player.GetComponent<Stats>().HideStats();
            player.GetComponent<PlayerController>().inStatsScreen = false;
        }
    }

    public void EnemyKilled(float xp, Vector3 pos)
    {
        GameManager.instance.enemies--;

        DropHealthPotion(pos);

        GameManager.instance.AddExperience(xp);

        if (GameManager.instance.enemies == 0)
        {
            inCombat = false;
            UnlockDoors();

            foreach (var player in players)
            {
                player.transform.Find("Aim").GetComponent<ProjectileDirection>().ClearCollidingEnemies();
            }

            DropLoot(Random.Range(1, 4), false);

            if (GameManager.instance.bossFight)
            {
                EndBossFight();
            }
        }
    }

    public void EnemySpawned()
    {
        GameManager.instance.enemies++;
    }

    public void PlayerKilled()
    {
        GameManager.instance.deadPlayers++;

        if (GameManager.instance.deadPlayers == 2)
        {
            GameOverMenu();
        }
    }

    public void Pause()
    {
        GameManager.instance.isPaused = true;
        Time.timeScale = 0f;
        eventSystem.firstSelectedGameObject = firstSelectedPauseScreen;
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        GameManager.instance.pauseScreen.SetActive(true);
    }
    public void ReviveCounter()
    {
        GameManager.instance.deadPlayers--;
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
    public void GameOverMenu()
    {
        Time.timeScale = 0f;
        eventSystem.firstSelectedGameObject = firstSelectedGameOverScreen;
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        GameManager.instance.gameOverScreen.SetActive(true);
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
                    if (DidLootDrop() || itemComponent.quality == Quality.LEGENDARY)
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
                                itemComponent.bonusBaseStat = 12;
                                break;
                            case Property.CRITCHANCE:
                                itemComponent.bonusBaseStat = 8;
                                break;
                            case Property.CRITDAMAGE:
                                itemComponent.bonusBaseStat = 20;
                                break;
                            case Property.BASEDAMAGE:
                                itemComponent.bonusBaseStat = 10;
                                break;
                        };
                        itemComponent.bonusQuality = GetPostfixOfItem();
                        break;
                    }
                }
            }
        }
    }

    public void DropHealthPotion(Vector3 pos)
    {
        bool drop = (Random.Range(0f, 1f) <= healthPotionChance);
        if (drop)
        {
            Instantiate(healthPotion, pos, Quaternion.identity, GameManager.instance.currentRoom.transform);
        }
    }

    public void RemoveFromItemLists(GameObject item)
    {
        GameManager.instance.AlanItemList.Remove(item);
        GameManager.instance.AlbusItemList.Remove(item);
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
        GameManager.instance.gameOverScreen = GameManager.instance.canvas.transform.Find("GameOverScreen").gameObject;
        GameManager.instance.firstSelectedPauseScreen = GameManager.instance.pauseScreen.transform.Find("Unpause").gameObject;
        GameManager.instance.firstSelectedGameOverScreen = GameManager.instance.gameOverScreen.transform.Find("Restart (1)").gameObject;
        GameManager.instance.eventSystem = EventSystem.current.GetComponent<EventSystem>();
        GameManager.instance.changingRooms = false;
        GameManager.instance.doors = new List<DoorController>();
        GameManager.instance.AlanItemList = new List<GameObject>();
        GameManager.instance.AlbusItemList = new List<GameObject>();
        GameManager.instance.bossUI.SetActive(false);
        GameManager.instance.pauseScreen.SetActive(false);
        GameManager.instance.gameOverScreen.SetActive(false);
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

        GameManager.instance.enemies = 0;

        var enemyList = new List<Enemy>();
        var children = currentRoom.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.tag == "Enemy" && child.gameObject.activeSelf)
            {
                enemyList.Add(child.gameObject.GetComponent<Enemy>());
                GameManager.instance.enemies++;

                if (bossFight)
                {
                    var vc = child.GetComponent<VitalityController>();
                    if (vc.boss)
                    {
                        vc.doUpdateUI = true;
                    }
                }
            }
            else if (child.tag == "Door")
            {
                var door = child.gameObject.GetComponent<DoorController>();
                doors.Add(door);
            }
        }

        if (GameManager.instance.enemies != 0)
        {
            GameManager.instance.inCombat = true;
            LockDoors();

            if (GameManager.instance.bossFight)
            {
                StartBossFight();
            }

            yield return new WaitForSeconds(GameManager.instance.timeBeforeAttacking);
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
            UIManager.instance.DisplayStatUpText(player.transform.position, 150f);
        }

        currentRoom.transform.Find("Hole").gameObject.SetActive(true);
    }

    private bool DidLootDrop()
    {
        return Random.value <= dropChance;
    }

    private Quality GetQualityOfItem()
    {
        var quality = Random.value;
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
        var quality = Random.value;
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
