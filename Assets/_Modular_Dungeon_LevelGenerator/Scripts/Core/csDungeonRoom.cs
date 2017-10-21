// Copyright (C) 2012 Vertex Stream Games
// Only for use, modification and/or re-distribution in a compiled project.
// No re-sale of modified code permitted.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csDungeonRoom 
	: csDungeonMap
{
    #region Constructor

	    public new void Constructor(int width, int height) 
		{
			base.Constructor(width, height);
	    }

    #endregion

    #region Methods

	    public void InitializeRoomCells()
	    {
	        foreach (Vector2 location in CellLocations)
	        {
	            csDungeonCell cell = new csDungeonCell();
	
	            cell.WestSide  = (location.x == bounds.x		 ) ? csDungeonCell.SideType.Wall : csDungeonCell.SideType.Empty;
	            cell.EastSide  = (location.x == bounds.width  - 1) ? csDungeonCell.SideType.Wall : csDungeonCell.SideType.Empty;
	            cell.NorthSide = (location.y == bounds.y		 ) ? csDungeonCell.SideType.Wall : csDungeonCell.SideType.Empty;
	            cell.SouthSide = (location.y == bounds.height - 1) ? csDungeonCell.SideType.Wall : csDungeonCell.SideType.Empty;
	
	            this[location] = cell;
	        }
	    }

	    public void SetLocation(Vector2 location)
	    {
	        bounds = new Rect(location.x, location.y, bounds.width, bounds.height);	
	    }

    #endregion    
}
