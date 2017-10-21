// Copyright (C) 2012 Vertex Stream Games
// Only for use, modification and/or re-distribution in a compiled project.
// No re-sale of modified code permitted.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csDungeonRoomGenerator
{
    #region Fields

        private int maxRoomHeight    = 6;
        private int maxRoomWidth     = 6;
        private int minRoomHeight    = 1;
        private int minRoomWidth     = 1;
        private int noOfRoomsToPlace = 10;

    #endregion

    #region Constructors	       

		public void Constructor()
		{
		}

        public void Constructor
			(int noOfRoomsToPlace, 
	 		 int minRoomWidth, 
	 		 int maxRoomWidth, 
	 		 int minRoomHeight, 
	 		 int maxRoomHeight)
        {
            this.noOfRoomsToPlace = noOfRoomsToPlace;
            this.minRoomWidth     = minRoomWidth;
            this.maxRoomWidth     = maxRoomWidth;
            this.minRoomHeight    = minRoomHeight;
            this.maxRoomHeight    = maxRoomHeight;
        }

    #endregion

    #region Properties

        public int NoOfRoomsToPlace
        {
            get { return noOfRoomsToPlace; }
            set { noOfRoomsToPlace = value; }
        }

        public int MinRoomWidth
        {
            get { return minRoomWidth; }
            set { minRoomWidth = value; }
        }

        public int MaxRoomWidth
        {
            get { return maxRoomWidth; }
            set { maxRoomWidth = value; }
        }

        public int MinRoomHeight
        {
            get { return minRoomHeight; }
            set { minRoomHeight = value; }
        }

        public int MaxRoomHeight
        {
            get { return maxRoomHeight; }
            set { maxRoomHeight = value; }
        }

    #endregion

    #region Methods

        public csDungeonRoom CreateRoom(bool forceSmall)
        {
            csDungeonRoom room = new csDungeonRoom();
			
			room.Constructor(
				csRandom.Instance.Next(forceSmall == true ? 1 : minRoomWidth,  forceSmall == true ? 1 : maxRoomWidth), 
				csRandom.Instance.Next(forceSmall == true ? 1 : minRoomHeight, forceSmall == true ? 1 : maxRoomHeight)
							);
            
			room.InitializeRoomCells();
            return room;
        }

        public void PlaceRooms(csDungeon dungeon)
        { 
			int roomsPlaced = 0;
		
            for (int roomCounter = 0; roomCounter < noOfRoomsToPlace; roomCounter++)
            {
				csDungeonRoom room = null;
			
				// Ensure that at least half the rooms placed are small rooms to avoid "empty" dungeons
//              if (maxRoomWidth > 1 || maxRoomHeight > 1)
//				{
//					if (roomCounter < noOfRoomsToPlace / 2)
//					{
//						room = CreateRoom(true);						
//					}
//					else
					{
						room = CreateRoom(false);
					}
//				}
//				else
//				{
//					room = CreateRoom(false);
//				}			
                
				int bestRoomPlacementScore = 0; // int.MaxValue;
                Vector2? bestRoomPlacementLocation = null;

                foreach (Vector2 currentRoomPlacementLocation in dungeon.CorridorCellLocations)
                {
                    int currentRoomPlacementScore = CalculateRoomPlacementScore(currentRoomPlacementLocation, room, dungeon);

                    if (currentRoomPlacementScore > bestRoomPlacementScore)
                    {
                        bestRoomPlacementScore    = currentRoomPlacementScore;
                        bestRoomPlacementLocation = currentRoomPlacementLocation;
                    }
                }

                // Create room at best room placement cell
                if (bestRoomPlacementLocation != null)
				{
                    PlaceRoom(bestRoomPlacementLocation.Value, room, dungeon);
					roomsPlaced ++;
				}
            }
        }
	

    public int CalculateRoomPlacementScore(Vector2 location, csDungeonRoom room, csDungeon dungeon)
    {
        // Check if the room at the given location will fit inside the bounds of the map
		
		//if (dungeon.Bounds.Contains(new Rect(location.x, location.y, (float)room.Width + 1, (float)room.Height + 1)))
		if (dungeon.Bounds.Contains(location) &&
		    dungeon.Bounds.Contains(new Vector2(location.x + room.Width + 1, location.y + room.Height + 1)))        
        {
            int roomPlacementScore = 0;

            // Loop for each cell in the room
            foreach (Vector2 roomLocation in room.CellLocations)
            {
                // Translate the room cell location to its location in the dungeon
                Vector2 dungeonLocation = new Vector2(location.x + roomLocation.x, location.y + roomLocation.y);

                // Add 1 point for each adjacent corridor to the cell
                if (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, csDungeonCell.DirectionType.North)) roomPlacementScore++;
                if (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, csDungeonCell.DirectionType.South)) roomPlacementScore++;
                if (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, csDungeonCell.DirectionType.West))  roomPlacementScore++;
                if (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, csDungeonCell.DirectionType.East))  roomPlacementScore++;

                // Add 3 points if the cell overlaps an existing corridor
                if (dungeon[dungeonLocation].IsCorridor) roomPlacementScore += 3;

                // Do not allow rooms to overlap!
                foreach (csDungeonRoom dungeonRoom in dungeon.Rooms)
                    if (dungeonRoom.Bounds.Contains(dungeonLocation))
                        return 0;
            }

            return roomPlacementScore;
        }
        else
        {
            return 0;
        }
    }

    public void PlaceRoom(Vector2 location, csDungeonRoom room, csDungeon dungeon)
    {
        // Offset the room origin to the new location
        room.SetLocation(location);

        // Loop for each cell in the room
        foreach (Vector2 roomLocation in room.CellLocations)
        {
            // Translate the room cell location to its location in the dungeon
            Vector2 dungeonLocation = new Vector2(location.x + roomLocation.x, location.y + roomLocation.y);
            dungeon[dungeonLocation].NorthSide = room[roomLocation].NorthSide;
            dungeon[dungeonLocation].SouthSide = room[roomLocation].SouthSide;
            dungeon[dungeonLocation].WestSide  = room[roomLocation].WestSide;
            dungeon[dungeonLocation].EastSide  = room[roomLocation].EastSide;

            // Create room walls on map (either side of the wall)
            if ((roomLocation.x == 0			  ) && (dungeon.HasAdjacentCellInDirection(dungeonLocation, csDungeonCell.DirectionType.West)))  dungeon.CreateWall(dungeonLocation, csDungeonCell.DirectionType.West);
            if ((roomLocation.x == room.Width - 1 ) && (dungeon.HasAdjacentCellInDirection(dungeonLocation, csDungeonCell.DirectionType.East)))  dungeon.CreateWall(dungeonLocation, csDungeonCell.DirectionType.East);
            if ((roomLocation.y == 0			  ) && (dungeon.HasAdjacentCellInDirection(dungeonLocation, csDungeonCell.DirectionType.North))) dungeon.CreateWall(dungeonLocation, csDungeonCell.DirectionType.North);
            if ((roomLocation.y == room.Height - 1) && (dungeon.HasAdjacentCellInDirection(dungeonLocation, csDungeonCell.DirectionType.South))) dungeon.CreateWall(dungeonLocation, csDungeonCell.DirectionType.South);
        }

        dungeon.AddRoom(room);
    }

    public void PlaceDoors(csDungeon dungeon)
    {
        foreach (csDungeonRoom room in dungeon.Rooms)
        {
            bool hasNorthDoor = false;
            bool hasSouthDoor = false;
            bool hasWestDoor  = false;
            bool hasEastDoor  = false;

            foreach (Vector2 cellLocation in room.CellLocations)
            {
                // Translate the room cell location to its location in the dungeon
                Vector2 dungeonLocation = new Vector2(room.Bounds.x + cellLocation.x, room.Bounds.y + cellLocation.y);

                // Check if we are on the west boundary of our room
                // and if there is a corridor to the west
                if ((cellLocation.x == 0) &&
                    (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, csDungeonCell.DirectionType.West)) &&
                    (!hasWestDoor))
                {
                    dungeon.CreateDoor(dungeonLocation, csDungeonCell.DirectionType.West);
                    hasWestDoor = true;
                }

                // Check if we are on the east boundary of our room
                // and if there is a corridor to the east
                if ((cellLocation.x == room.Width - 1) &&
                    (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, csDungeonCell.DirectionType.East)) &&
                    (!hasEastDoor))
                {
                    dungeon.CreateDoor(dungeonLocation, csDungeonCell.DirectionType.East);
                    hasEastDoor = true;
                }

                // Check if we are on the north boundary of our room 
                // and if there is a corridor to the north
                if ((cellLocation.y == 0) &&
                    (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, csDungeonCell.DirectionType.North)) &&
                    (!hasNorthDoor))
                {
                    dungeon.CreateDoor(dungeonLocation, csDungeonCell.DirectionType.North);
                    hasNorthDoor = true;
                }


                // Check if we are on the south boundary of our room 
                // and if there is a corridor to the south
                if ((cellLocation.y == room.Height - 1) &&
                    (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, csDungeonCell.DirectionType.South)) &&
                    (!hasSouthDoor))
                {
                    dungeon.CreateDoor(dungeonLocation, csDungeonCell.DirectionType.South);
                    hasSouthDoor = true;
                }
            }
        }
    }

    #endregion
}	