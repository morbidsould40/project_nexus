// Copyright (C) 2012 Vertex Stream Games
// Only for use, modification and/or re-distribution in a compiled project.
// No re-sale of modified code permitted.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class csDungeonGenerator
{
    #region Fields

	    private int width  = 25;
	    private int height = 25;
	    private int changeDirectionModifier = 30;
	    private int sparsenessModifier = 70;
	    private int deadEndRemovalModifier = 50;
	    private csDungeonRoomGenerator roomGenerator = new csDungeonRoomGenerator(); 

    #endregion

    #region Constructors

    	public void Constructor()
	    {
			roomGenerator.Constructor(10, 1, 1, 1, 1);		
	    }
	
	    public void Constructor
			(int width, int height, int changeDirectionModifier, int sparsenessModifier, int deadEndRemovalModifier, csDungeonRoomGenerator roomGenerator)
	    {
			this.width = width;
	        this.height = height;
	        this.changeDirectionModifier = changeDirectionModifier;
	        this.sparsenessModifier = sparsenessModifier;
	        this.deadEndRemovalModifier = deadEndRemovalModifier;
	        this.roomGenerator = roomGenerator;
	    }

    #endregion

    #region Methods

	    public csDungeon Generate()
	    {
	        csDungeon dungeon = new csDungeon();
			dungeon.Constructor(width, height);
	        dungeon.FlagAllCellsAsUnvisited();
	
	        CreateDenseMaze(dungeon);
	        SparsifyMaze(dungeon);
	        RemoveDeadEnds(dungeon);
	        roomGenerator.PlaceRooms(dungeon);
	        roomGenerator.PlaceDoors(dungeon);
	
	        return dungeon;
	    }

	    public void CreateDenseMaze(csDungeon dungeon)
	    {
	        Vector2 currentLocation = dungeon.PickRandomCellAndFlagItAsVisited();
	        csDungeonCell.DirectionType previousDirection = csDungeonCell.DirectionType.North;
	
	        while (!dungeon.AllCellsAreVisited)
	        {
	            csDirectionPicker directionPicker = new csDirectionPicker();
				directionPicker.Constructor(previousDirection, changeDirectionModifier);
	            csDungeonCell.DirectionType direction = directionPicker.GetNextDirection();
	
	            while (!dungeon.HasAdjacentCellInDirection(currentLocation, direction) || dungeon.AdjacentCellInDirectionIsVisited(currentLocation, direction))
	            {
	                if (directionPicker.HasNextDirection)
	                    direction = directionPicker.GetNextDirection();
	                else
	                {
	                    currentLocation = dungeon.GetRandomVisitedCell(currentLocation); // Get a new previously visited location
	                    directionPicker = new csDirectionPicker();
						directionPicker.Constructor(previousDirection, changeDirectionModifier); // Reset the direction picker
	                    direction = directionPicker.GetNextDirection(); // Get a new direction
	                }
	            }
	
	            currentLocation = dungeon.CreateCorridor(currentLocation, direction);
	            dungeon.FlagCellAsVisited(currentLocation);
	            previousDirection = direction;
	        }
	    }

	    public void SparsifyMaze(csDungeon dungeon)
	    {
	        // Calculate the number of cells to remove as a percentage of the total number of cells in the dungeon
	        int noOfDeadEndCellsToRemove = (int)Math.Ceiling(((double)sparsenessModifier / 100) * (dungeon.Width * dungeon.Height));
	
	        IEnumerator<Vector2> enumerator = dungeon.DeadEndCellLocations.GetEnumerator();
	
	        for (int i = 0; i < noOfDeadEndCellsToRemove; i++)
	        {
	            if (!enumerator.MoveNext()) // Check if there is another item in our enumerator
	            {
	                enumerator = dungeon.DeadEndCellLocations.GetEnumerator(); // Get a new enumerator
	                if (!enumerator.MoveNext()) break; // No new items exist so break out of loop
	            }
	
	            Vector2 point = enumerator.Current;
	            dungeon.CreateWall(point, dungeon[point].CalculateDeadEndCorridorDirection());
	            dungeon[point].IsCorridor = false;
	        }
	    }

    	public void RemoveDeadEnds(csDungeon dungeon)
	    {
	        foreach (Vector2 deadEndLocation in dungeon.DeadEndCellLocations)
	        {
	            if (ShouldRemoveDeadend())
	            {
	                Vector2 currentLocation = deadEndLocation;
	
	                do
	                {
	                    // Initialize the direction picker not to select the dead-end corridor direction
	                    csDirectionPicker directionPicker = new csDirectionPicker();
						directionPicker.Constructor(dungeon[currentLocation].CalculateDeadEndCorridorDirection(), 100);
	                    csDungeonCell.DirectionType direction = directionPicker.GetNextDirection();
	
	                    while (!dungeon.HasAdjacentCellInDirection(currentLocation, direction))
	                    {
	                        if (directionPicker.HasNextDirection)
	                            direction = directionPicker.GetNextDirection();
	                        else
	                            throw new InvalidOperationException("This should not happen");
	                    }
	                    // Create a corridor in the selected direction
	                    currentLocation = dungeon.CreateCorridor(currentLocation, direction);
	
	                } while (dungeon[currentLocation].IsDeadEnd); // Stop when you intersect an existing corridor.
	            }
	        }
	    }
	
	    public bool ShouldRemoveDeadend()
	    {
	        return csRandom.Instance.Next(1, 99) < deadEndRemovalModifier;
	    }    
	
		public static int[, ] ExpandToTiles(csDungeon dungeon)
	    {
	        // Instantiate our tile array
	        int[, ] tiles = new int[dungeon.Width * 2 + 2, dungeon.Height * 2 + 2];
	
	        // Initialize the tile array to void (empty)
	        for (int x = 0; x < dungeon.Width * 2 + 2; x++)
	            for (int y = 0; y < dungeon.Height * 2 + 2; y++)
	                tiles[x, y] = (int)csDungeonCell.TileType.Rock;           
	
	        // Loop for each corridor cell and expand it
	        foreach (Vector2 cellLocation in dungeon.CorridorCellLocations)
	        {
	            Vector2 tileLocation = new Vector2(cellLocation.x*2 + 1, cellLocation.y*2 + 1);
	            tiles[(int)tileLocation.x, (int)tileLocation.y] = (int)csDungeonCell.TileType.Corridor;
	
	            if (dungeon[cellLocation].NorthSide == csDungeonCell.SideType.Empty) tiles[(int)tileLocation.x, (int)tileLocation.y - 1] = (int)csDungeonCell.TileType.Corridor;
	            if (dungeon[cellLocation].NorthSide == csDungeonCell.SideType.Door)  tiles[(int)tileLocation.x, (int)tileLocation.y - 1] = (int)csDungeonCell.TileType.DoorNS;
				if (dungeon[cellLocation].NorthSide == csDungeonCell.SideType.Wall)  tiles[(int)tileLocation.x, (int)tileLocation.y - 1] = (int)csDungeonCell.TileType.Rock;
	
	            if (dungeon[cellLocation].SouthSide == csDungeonCell.SideType.Empty) tiles[(int)tileLocation.x, (int)tileLocation.y + 1] = (int)csDungeonCell.TileType.Corridor;
	            if (dungeon[cellLocation].SouthSide == csDungeonCell.SideType.Door)  tiles[(int)tileLocation.x, (int)tileLocation.y + 1] = (int)csDungeonCell.TileType.DoorNS;
				if (dungeon[cellLocation].SouthSide == csDungeonCell.SideType.Wall)  tiles[(int)tileLocation.x, (int)tileLocation.y + 1] = (int)csDungeonCell.TileType.Rock;
	
	            if (dungeon[cellLocation].WestSide == csDungeonCell.SideType.Empty) tiles[(int)tileLocation.x - 1, (int)tileLocation.y] = (int)csDungeonCell.TileType.Corridor;
	            if (dungeon[cellLocation].WestSide == csDungeonCell.SideType.Door)  tiles[(int)tileLocation.x - 1, (int)tileLocation.y] = (int)csDungeonCell.TileType.DoorEW;
				if (dungeon[cellLocation].WestSide == csDungeonCell.SideType.Wall)  tiles[(int)tileLocation.x - 1, (int)tileLocation.y] = (int)csDungeonCell.TileType.Rock;
			
	            if (dungeon[cellLocation].EastSide == csDungeonCell.SideType.Empty) tiles[(int)tileLocation.x + 1, (int)tileLocation.y] = (int)csDungeonCell.TileType.Corridor;
	            if (dungeon[cellLocation].EastSide == csDungeonCell.SideType.Door)  tiles[(int)tileLocation.x + 1, (int)tileLocation.y] = (int)csDungeonCell.TileType.DoorEW; 
	    		if (dungeon[cellLocation].EastSide == csDungeonCell.SideType.Wall)  tiles[(int)tileLocation.x + 1, (int)tileLocation.y] = (int)csDungeonCell.TileType.Rock; 
			}		
	
	        // Fill tiles with corridor values for each room in dungeon
	        foreach (csDungeonRoom room in dungeon.Rooms)
	        {
	            // Get the room min and max location in tile coordinates
	            Vector2 minPoint = new Vector2(room.Bounds.xMin * 2 + 1, room.Bounds.yMin * 2 + 1);
	            Vector2 maxPoint = new Vector2(room.Bounds.xMax * 2,     room.Bounds.yMax * 2);
	
	            // Fill the room in tile space with an empty value
	            for (int i = (int)minPoint.x; i < (int)maxPoint.x; i++)
	                for (int j = (int)minPoint.y; j < (int)maxPoint.y; j++)
	                    tiles[i, j] = (int)csDungeonCell.TileType.Room;
	        }
		
			// Remove unnecessary rock
			List<Vector2> rocksToRemove = new List<Vector2>();	
		 	
	        for (int x = 1; x < dungeon.Width * 2 + 1; x++)
	            for (int y = 1; y < dungeon.Height * 2 + 1; y++)
			{
				if (tiles[x,   y  ] == (int)csDungeonCell.TileType.Rock &&		    	
					tiles[x-1, y-1] == (int)csDungeonCell.TileType.Rock &&		    	
					tiles[x  , y-1] == (int)csDungeonCell.TileType.Rock &&		    	
					tiles[x+1, y-1] == (int)csDungeonCell.TileType.Rock &&		    	
			    	tiles[x-1, y  ] == (int)csDungeonCell.TileType.Rock &&		    				    	
					tiles[x+1, y  ] == (int)csDungeonCell.TileType.Rock &&		    	
					tiles[x-1, y+1] == (int)csDungeonCell.TileType.Rock &&		    	
					tiles[x  , y+1] == (int)csDungeonCell.TileType.Rock &&		    	
					tiles[x+1, y+1] == (int)csDungeonCell.TileType.Rock )
			    	
				{
					rocksToRemove.Add(new Vector2(x,y));
				}
			}				
		
			foreach (Vector2 v in rocksToRemove)
				tiles[(int)v.x, (int)v.y] = (int)csDungeonCell.TileType.Void;		
		
			for (int x = 0; x < dungeon.Width * 2 + 1; x++)
			{
				if (tiles[x,0] == (int)csDungeonCell.TileType.Rock && tiles[x,1] == (int)csDungeonCell.TileType.Void)	
					tiles[x,0] =  (int)csDungeonCell.TileType.Void;					
			}	
		
			for (int y = 0; y < dungeon.Height * 2 + 1; y++)
			{
				if (tiles[0,y] == (int)csDungeonCell.TileType.Rock && tiles[1,y] == (int)csDungeonCell.TileType.Void)	
					tiles[0,y] =  (int)csDungeonCell.TileType.Void;					
			}	
	
	        return tiles;
	    }

    #endregion

    #region Properties

	    public int Width
	    {
	        get { return width; }
	        set { width = value; }
	    }
	
	    public int Height
	    {
	        get { return height; }
	        set { height = value; }
	    }
	
	    public int ChangeDirectionModifier
	    {
	        get { return changeDirectionModifier; }
	        set { changeDirectionModifier = value; }
	    }
	
	    public int SparsenessModifier
	    {
	        get { return sparsenessModifier; }
	        set { sparsenessModifier = value; }
	    }
	
	    public int DeadEndRemovalModifier
	    {
	        get { return deadEndRemovalModifier; }
	        set { deadEndRemovalModifier = value; }
	    }

    #endregion

}
