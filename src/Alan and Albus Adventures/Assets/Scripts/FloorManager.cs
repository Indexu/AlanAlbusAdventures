using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FloorManager : MonoBehaviour
{
    private class Point : IEquatable<Point>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }

        public override bool Equals(object obj)
        {
            var p = (Point)obj;
            return X == p.X && Y == p.Y;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }
    }

    public int gridLength;
    public int floorLevel;
    public float enemySpawnRadius;
    public int minRooms;
    public int maxRooms;
    public int minBlobsPerRoom;
    public int maxBlobsPerRoom;
    public int maxBlobsThreshold;
    public GameObject room;
    public GameObject doorwayUp;
    public GameObject doorwayDown;
    public GameObject doorwayLeft;
    public GameObject doorwayRight;
    public GameObject enemyHealthBar;
    public GameObject chest;
    public GameObject startFloorTutorial;
    public List<GameObject> enemies;
    public List<GameObject> bosses;

    private GameObject floor;
    private GameObject[,] grid;
    private List<Point> roomCoords;
    private List<Point> availableCoords;
    private Point startCoords;
    private Point chestCoords;
    private Point bossCoords;
    private Bounds bounds;
    private float enemyScalar = 1f;

    public void GenerateFloor()
    {
        Init();
        MinMaxRooms();
        MinMaxBlobs();
        GenerateCoords();
        InstantiateRooms();
        ConnectRooms();
        SpawnEnemies();
        SpawnBoss();
        SpawnChest();
        SpawnStartTutorial();
        PlacePlayersAndCamera();

        floorLevel++;
    }

    private void Init()
    {
        grid = new GameObject[gridLength, gridLength];
        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                grid[i, j] = null;
            }
        }

        roomCoords = new List<Point>();
        availableCoords = new List<Point>();
        bounds = room.GetComponent<Renderer>().bounds;

        if (floor != null)
        {
            GameObject.Destroy(floor);
        }
    }

    private void ResetGrid()
    {
        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridLength; i++)
            {
                grid[i, j] = null;
            }
        }

        roomCoords.Clear();
    }

    private void GenerateCoords()
    {
        var numberOfRooms = floorLevel + Random.Range(minRooms, maxRooms);

        if (gridLength * gridLength < numberOfRooms)
        {
            numberOfRooms = gridLength * gridLength;
        }

        var startX = Random.Range(0, gridLength);
        var startY = Random.Range(0, gridLength);

        startCoords = new Point(startX, startY);

        roomCoords.Add(startCoords);
        AddAdjacentCoords(startCoords);

        for (int i = 0; (i < numberOfRooms && availableCoords.Count != 0); i++)
        {
            var selectedRoomIndex = Random.Range(0, availableCoords.Count);
            var selectedRoomCoords = availableCoords[selectedRoomIndex];
            availableCoords.RemoveAt(selectedRoomIndex);
            roomCoords.Add(selectedRoomCoords);
            AddAdjacentCoords(selectedRoomCoords);
        }

        chestCoords = roomCoords[roomCoords.Count - 2];
        bossCoords = roomCoords[roomCoords.Count - 1];
    }

    private void AddAdjacentCoords(Point point)
    {
        var x = 0;
        var y = 0;

        // Up
        if (point.Y != 0)
        {
            x = point.X;
            y = point.Y - 1;
            var newPoint = new Point(x, y);

            if (!roomCoords.Contains(newPoint) && !availableCoords.Contains(newPoint))
            {
                availableCoords.Add(newPoint);
            }
        }
        // Down
        if (point.Y != gridLength - 1)
        {
            x = point.X;
            y = point.Y + 1;
            var newPoint = new Point(x, y);

            if (!roomCoords.Contains(newPoint) && !availableCoords.Contains(newPoint))
            {
                availableCoords.Add(newPoint);
            }
        }
        // Left
        if (point.X != 0)
        {
            x = point.X - 1;
            y = point.Y;
            var newPoint = new Point(x, y);

            if (!roomCoords.Contains(newPoint) && !availableCoords.Contains(newPoint))
            {
                availableCoords.Add(newPoint);
            }
        }
        // Right
        if (point.X != gridLength - 1)
        {
            x = point.X + 1;
            y = point.Y;
            var newPoint = new Point(x, y);

            if (!roomCoords.Contains(newPoint) && !availableCoords.Contains(newPoint))
            {
                availableCoords.Add(newPoint);
            }
        }
    }

    private void InstantiateRooms()
    {
        var sizeVector = bounds.size;

        floor = new GameObject()
        {
            name = "Floor"
        };

        Vector3 position = new Vector3();

        foreach (var roomCoord in roomCoords)
        {
            position.x = sizeVector.x * roomCoord.X;
            position.y = sizeVector.y * roomCoord.Y;

            grid[roomCoord.X, roomCoord.Y] = Instantiate(room, position, Quaternion.identity, floor.transform);
        }
    }

    private void ConnectRooms()
    {
        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                if (grid[i, j] != null)
                {
                    ConnectAdjacentRooms(grid[i, j], new Point(i, j));
                }
            }
        }
    }

    private void ConnectAdjacentRooms(GameObject centerRoom, Point point)
    {
        // Up?
        if (point.Y != 0)
        {
            if (grid[point.X, point.Y - 1] != null)
            {
                ConnectRooms(centerRoom, grid[point.X, point.Y - 1], Direction.down, (point == bossCoords));
            }
        }
        // Down?
        if (point.Y != gridLength - 1)
        {
            if (grid[point.X, point.Y + 1] != null)
            {
                ConnectRooms(centerRoom, grid[point.X, point.Y + 1], Direction.up, (point == bossCoords));
            }
        }
        // Left
        if (point.X != 0)
        {
            if (grid[point.X - 1, point.Y] != null)
            {
                ConnectRooms(centerRoom, grid[point.X - 1, point.Y], Direction.left, (point == bossCoords));
            }
        }
        // Right
        if (point.X != gridLength - 1)
        {
            if (grid[point.X + 1, point.Y] != null)
            {
                ConnectRooms(centerRoom, grid[point.X + 1, point.Y], Direction.right, (point == bossCoords));
            }
        }
    }

    private void ConnectRooms(GameObject room1, GameObject room2, Direction direction, bool boss)
    {
        Transform door1;
        Transform door2;
        DoorController dc1;
        DoorController dc2;

        if (direction == Direction.up)
        {
            door1 = GetDoorwayByName(room1, doorwayUp.name);
            door2 = GetDoorwayByName(room2, doorwayDown.name);
        }
        else if (direction == Direction.down)
        {
            door1 = GetDoorwayByName(room1, doorwayDown.name);
            door2 = GetDoorwayByName(room2, doorwayUp.name);
        }
        else if (direction == Direction.left)
        {
            door1 = GetDoorwayByName(room1, doorwayLeft.name);
            door2 = GetDoorwayByName(room2, doorwayRight.name);
        }
        else
        {
            door1 = GetDoorwayByName(room1, doorwayRight.name);
            door2 = GetDoorwayByName(room2, doorwayLeft.name);
        }

        if (door1 != null && door2 != null)
        {
            door1.gameObject.SetActive(true);
            door2.gameObject.SetActive(true);

            dc1 = door1.gameObject.GetComponent<DoorController>();
            dc2 = door2.gameObject.GetComponent<DoorController>();

            dc1.connectedRoom = room2;
            dc1.connectedDoor = door2;
            dc2.connectedRoom = room1;
            dc2.connectedDoor = door1;

            dc2.leadsToBoss = boss;
        }
    }

    private Transform GetDoorwayByName(GameObject room, string name)
    {
        var children = room.GetComponentsInChildren<Transform>(true);

        foreach (var child in children)
        {
            if (child.name == name)
            {
                return child;
            }
        }

        return null;
    }

    private void SpawnEnemies()
    {
        var spawnVector = new Vector3();
        int numberOfEnemies;
        GameObject enemy;

        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                if (grid[i, j] != null && (startCoords.X != i || startCoords.Y != j) && (bossCoords.X != i || bossCoords.Y != j) && (chestCoords.X != i || chestCoords.Y != j))
                {
                    var maxRangeVector = grid[i, j].transform.position + (bounds.extents * enemySpawnRadius);
                    var minRangeVector = grid[i, j].transform.position - (bounds.extents * enemySpawnRadius);
                    numberOfEnemies = Random.Range(minBlobsPerRoom, maxBlobsPerRoom + 1);

                    for (int k = 0; k < numberOfEnemies; k++)
                    {
                        enemy = enemies[Random.Range(0, enemies.Count)];
                        spawnVector.x = Random.Range(minRangeVector.x, maxRangeVector.x);
                        spawnVector.y = Random.Range(minRangeVector.y, maxRangeVector.y);

                        var enemyInstance = Instantiate(enemy, spawnVector, Quaternion.identity, grid[i, j].transform);
                        var healthBar = Instantiate(enemyHealthBar, Vector3.zero, Quaternion.identity, GameManager.instance.canvas.transform);

                        var enemyController = enemyInstance.GetComponent<Enemy>();
                        var vc = enemyInstance.GetComponent<VitalityController>();

                        enemyController.damage *= enemyScalar;
                        vc.currentHealth *= enemyScalar;

                        vc.healthSlider = healthBar.GetComponent<Slider>();
                        healthBar.SetActive(false);
                    }
                }
            }
        }

        enemyScalar += 0.25f;
    }

    private void SpawnBoss()
    {
        GameObject boss;

        if (GameManager.instance.storyMode)
        {
            boss = bosses[floorLevel - 1];
        }
        else
        {
            boss = bosses[Random.Range(0, bosses.Count)];
        }

        boss = Instantiate(boss, grid[bossCoords.X, bossCoords.Y].transform.position, Quaternion.identity, grid[bossCoords.X, bossCoords.Y].transform);

        var bc = boss.GetComponent<Boss>();
        var vc = boss.GetComponent<VitalityController>();

        bc.damage *= enemyScalar;
        vc.currentHealth *= enemyScalar;

        Text bossText = null;
        Text bossHealthText = null;
        Slider bossHealthSlider = null;

        var children = GameManager.instance.bossUI.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child.name == "BossText")
            {
                bossText = child.GetComponent<Text>();
            }
            else if (child.name == "BossHealthSlider")
            {
                bossHealthSlider = child.GetComponent<Slider>();
            }
        }

        children = bossHealthSlider.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child.name == "HealthText")
            {
                bossHealthText = child.GetComponent<Text>();
            }
        }

        bc.nameText = bossText;
        vc.healthText = bossHealthText;
        vc.healthSlider = bossHealthSlider;
    }

    private void SpawnChest()
    {
        Instantiate(chest, grid[chestCoords.X, chestCoords.Y].transform.position, Quaternion.identity, grid[chestCoords.X, chestCoords.Y].transform);
    }

    private void SpawnStartTutorial()
    {
        if (floorLevel == 1)
        {
            Instantiate(startFloorTutorial, grid[startCoords.X, startCoords.Y].transform.position, Quaternion.identity, grid[startCoords.X, startCoords.Y].transform);
        }
    }

    private void PlacePlayersAndCamera()
    {
        var spawnRoom = grid[startCoords.X, startCoords.Y];

        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            GameManager.instance.players[i].transform.position = spawnRoom.transform.position;
        }

        GameManager.instance.mainCamera.transform.position = spawnRoom.transform.position + new Vector3(0f, 0f, -10f);
    }

    private void MinMaxRooms()
    {
        if (floorLevel != 1 && floorLevel % 2 == 1)
        {
            minRooms += 2;
            maxRooms += 2;
        }
    }

    private void MinMaxBlobs()
    {
        if (floorLevel != 1 && maxBlobsPerRoom < maxBlobsThreshold)
        {
            minBlobsPerRoom++;
            maxBlobsPerRoom++;
        }
    }
}
