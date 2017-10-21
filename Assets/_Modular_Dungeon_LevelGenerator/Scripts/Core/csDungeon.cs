// Copyright (C) 2012 Vertex Stream Games
// Only for use, modification and/or re-distribution in a compiled project.
// No re-sale of modified code permitted.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

 public class csDungeon 
	: csDungeonMap
{
    #region Fields

	    private List<Vector2> visitedCells = new List<Vector2>();
	    private List<csDungeonRoom> rooms = new List<csDungeonRoom>();

    #endregion

    #region Constructors

	    public new void Constructor(int width, int height) 
	    {
			base.Constructor(width, height);
	    }

    #endregion

    #region Methods

	    internal void AddRoom(csDungeonRoom room)
	    {
	        rooms.Add(room);
	    }

	    public void FlagAllCellsAsUnvisited()
	    {
	        foreach(Vector2 location in CellLocations)
	            this[location].Visited = false;
	    }

	    public Vector2 PickRandomCellAndFlagItAsVisited()
	    {
	        Vector2 randomLocation = new Vector2(csRandom.Instance.Next(Width - 1), csRandom.Instance.Next(Height - 1));
	        FlagCellAsVisited(randomLocation);
	        return randomLocation;
	    }

    public bool AdjacentCellInDirectionIsVisited(Vector2 location, csDungeonCell.DirectionType direction)
    {
        Vector2? target = GetTargetLocation(location, direction);
        
        if (target == null)
            return false;

        switch (direction)
        {
            case csDungeonCell.DirectionType.North:
                return this[target.Value].Visited;
            case csDungeonCell.DirectionType.West:
                return this[target.Value].Visited;
            case csDungeonCell.DirectionType.South:
                return this[target.Value].Visited;
            case csDungeonCell.DirectionType.East:
                return this[target.Value].Visited;
            default:
                throw new InvalidOperationException();
        }
    }

    public bool AdjacentCellInDirectionIsCorridor(Vector2 location, csDungeonCell.DirectionType direction)
    {
        Vector2? target = GetTargetLocation(location, direction);

        if (target == null)
            return false;

        switch (direction)
        {
            case csDungeonCell.DirectionType.North:
                return this[target.Value].IsCorridor;
            case csDungeonCell.DirectionType.West:
                return this[target.Value].IsCorridor;
            case csDungeonCell.DirectionType.South:
                return this[target.Value].IsCorridor;
            case csDungeonCell.DirectionType.East:
                return this[target.Value].IsCorridor;
            default:
                return false;
        }
    }

    public void FlagCellAsVisited(Vector2 location)
    {
        if (!Bounds.Contains(location)) throw new ArgumentException("Location is outside of Dungeon bounds", "location");
        if (this[location].Visited) throw new ArgumentException("Location is already visited", "location");

        this[location].Visited = true;
        visitedCells.Add(location);
    }

    public Vector2 GetRandomVisitedCell(Vector2 location)
    {
        if (visitedCells.Count == 0) throw new InvalidOperationException("There are no visited cells to return.");

        int index = csRandom.Instance.Next(visitedCells.Count - 1);

        // Loop while the current cell is the visited cell
        while (visitedCells[index] == location)
            index = csRandom.Instance.Next(visitedCells.Count - 1);

        return visitedCells[index];
    }

    public Vector2 CreateCorridor(Vector2 location, csDungeonCell.DirectionType direction)
    {
        Vector2 targetLocation = CreateSide(location, direction, csDungeonCell.SideType.Empty);
        this[location].IsCorridor = true; // Set current location to corridor
        this[targetLocation].IsCorridor = true; // Set target location to corridor
        return targetLocation;
    }

    public Vector2 CreateWall(Vector2 location, csDungeonCell.DirectionType direction)
    {
        return CreateSide(location, direction, csDungeonCell.SideType.Wall);
    }

    public Vector2 CreateDoor(Vector2 location, csDungeonCell.DirectionType direction)
    {
        return CreateSide(location, direction, csDungeonCell.SideType.Door);
    }

    private Vector2 CreateSide(Vector2 location, csDungeonCell.DirectionType direction, csDungeonCell.SideType sideType)
    {
        Vector2? target = GetTargetLocation(location, direction);
        if (target == null) throw new ArgumentException("There is no adjacent cell in the given direction", "location");

        switch (direction)
        {
            case csDungeonCell.DirectionType.North:
                this[location].NorthSide = sideType;
                this[target.Value].SouthSide = sideType;
                break;
            case csDungeonCell.DirectionType.South:
                this[location].SouthSide = sideType;
                this[target.Value].NorthSide = sideType;
                break;
            case csDungeonCell.DirectionType.West:
                this[location].WestSide = sideType;
                this[target.Value].EastSide = sideType;
                break;
            case csDungeonCell.DirectionType.East:
                this[location].EastSide = sideType;
                this[target.Value].WestSide = sideType;
                break;
        }

        return target.Value;
    }

    #endregion

    #region Properties

    public ReadOnlyCollection<csDungeonRoom> Rooms
    {
        get { return rooms.AsReadOnly(); }
    }

    public IEnumerable<Vector2> DeadEndCellLocations
    {
        get
        {
            foreach (Vector2 point in CellLocations)
                if (this[point].IsDeadEnd) yield return point;
        }
    }

    public IEnumerable<Vector2> CorridorCellLocations
    {
        get
        {
            foreach (Vector2 point in CellLocations)
                if (this[point].IsCorridor) yield return point;

        }
    }

    public bool AllCellsAreVisited
    {
        get { return visitedCells.Count == (Width*Height); }
    }

    #endregion
}
