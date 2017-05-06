using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }
    }

    public int gridLength;
    public int floorLevel;
    public GameObject room;
    public GameObject doorwayUp;
    public GameObject doorwayDown;
    public GameObject doorwayLeft;
    public GameObject doorwayRight;

    private GameObject[,] grid;
    private List<Point> roomCoords;
    private List<Point> availableCoords;

    public void GenerateFloor()
    {
        GenerateCoords();
        InstantiateRooms();
        ConnectRooms();
    }

    private void Start ()
    {
        InitializeGrid();
        GenerateFloor();
    }

    private void InitializeGrid()
    {
        grid = new GameObject[gridLength, gridLength];
        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                grid[i,j] = null;
            }
        }

        roomCoords = new List<Point>();
        availableCoords = new List<Point>();
    }

    private void ResetGrid()
    {
        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridLength; i++)
            {
                grid[i,j] = null;
            }
        }

        roomCoords.Clear();
    }

    private void GenerateCoords()
    {
        var numberOfRooms = floorLevel + Random.Range(floorLevel / 2, floorLevel * 2);

        if (gridLength * gridLength < numberOfRooms)
        {
            numberOfRooms = gridLength * gridLength;
        }

        var startX = Random.Range(0, gridLength);
        var startY = Random.Range(0, gridLength);

        var startRoom = new Point(startX, startY);

        roomCoords.Add(startRoom);
        AddAdjacentCoords(startRoom);

        for (int i = 0; i <= numberOfRooms; i++)
        {
            var selectedRoomIndex = Random.Range(0, availableCoords.Count - 1);
            var selectedRoomCoords = availableCoords[selectedRoomIndex];
            availableCoords.RemoveAt(selectedRoomIndex);
            roomCoords.Add(selectedRoomCoords);
            AddAdjacentCoords(selectedRoomCoords);
        }
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
        if (point.Y != gridLength-1)
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
        if (point.X != gridLength-1)
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
        var sizeVector = room.GetComponent<Renderer>().bounds.size;

        var floor = new GameObject()
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
                ConnectRooms(centerRoom, grid[point.X, point.Y - 1], Direction.down);
            }
        }
        // Down?
        if (point.Y != gridLength - 1)
        {
            if (grid[point.X, point.Y + 1] != null)
            {
                ConnectRooms(centerRoom, grid[point.X, point.Y + 1], Direction.up);
            }
        }
        // Left
        if (point.X != 0)
        {
            if (grid[point.X - 1, point.Y] != null)
            {
                ConnectRooms(centerRoom, grid[point.X - 1, point.Y], Direction.left);
            }
        }
        // Right
        if (point.X != gridLength - 1)
        {
            if (grid[point.X + 1, point.Y] != null)
            {
                ConnectRooms(centerRoom, grid[point.X + 1, point.Y], Direction.right);
            }
        }
    }

    private void ConnectRooms(GameObject room1, GameObject room2, Direction direction)
    {
        Transform door1;
        Transform door2;
        DoorController dc1;
        DoorController dc2;

        if (direction == Direction.up)
        {
            door1 = GetDoorwayByName(room1, doorwayUp.transform.name);
            door2 = GetDoorwayByName(room2, doorwayDown.transform.name);
        }
        else if (direction == Direction.down)
        {
            door1 = GetDoorwayByName(room1, doorwayDown.transform.name);
            door2 = GetDoorwayByName(room2, doorwayUp.transform.name);
        }
        else if (direction == Direction.left)
        {
            door1 = GetDoorwayByName(room1, doorwayLeft.transform.name);
            door2 = GetDoorwayByName(room2, doorwayRight.transform.name);
        }
        else
        {
            door1 = GetDoorwayByName(room1, doorwayRight.transform.name);
            door2 = GetDoorwayByName(room2, doorwayLeft.transform.name);
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
}
