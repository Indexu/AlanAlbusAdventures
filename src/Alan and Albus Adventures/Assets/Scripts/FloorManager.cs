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
    public GameObject room;
    public int floorLevel;

    private GameObject[,] grid;
    private List<Point> roomCoords;
    private List<Point> availableCoords;

    public void GenerateFloor()
    {
        GenerateCoords();

        for (int i = 0; i < roomCoords.Count; i++)
        {
            Debug.Log("X: " + roomCoords[i].X + " Y: " + roomCoords[i].Y);
        }
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
}
