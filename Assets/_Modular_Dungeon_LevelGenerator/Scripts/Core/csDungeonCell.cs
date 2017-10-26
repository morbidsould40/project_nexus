// Copyright (C) 2012 Vertex Stream Games
// Only for use, modification and/or re-distribution in a compiled project.
// No re-sale of modified code permitted.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csDungeonCell 	
{
	#region Enums			
		
		public enum SideType
	    {
	        Empty,
	        Wall,
	        Door
	    }
	
		public enum DirectionType
	    {
	        North,
	        South,
	        East,
	        West
	    }
	
		public enum TileType
		{	
			Void     = 0,
		    Rock 	 = 1,
		    Corridor = 2,
		    Room 	 = 3,
		    DoorNS 	 = 4,
		    DoorEW 	 = 5,      
		}
	
	#endregion
	
	#region Members
	
		private SideType northSide = SideType.Wall;
        private SideType southSide = SideType.Wall;
        private SideType eastSide  = SideType.Wall;        
		private SideType westSide  = SideType.Wall;
	
        private bool visited;        
        private bool isCorridor;

	#endregion
	
	#region Properties
	
        public SideType NorthSide
        {
            get { return northSide;  }
            set { northSide = value; }
        }

        public SideType SouthSide
        {
            get { return southSide;  }
            set { southSide = value; }
        }

        public SideType EastSide
        {
            get { return eastSide;  }
            set { eastSide = value; }
        }

        public SideType WestSide
        {
            get { return westSide;  }
            set { westSide = value; }
        }
	
		public bool Visited
        {
            get { return visited;  }
            set { visited = value; }
        }
	
		public bool IsCorridor
        {
            get { return isCorridor; }
            set { isCorridor = value;}
        }

        public bool IsDeadEnd
        {
            get { return WallCount == 3; }
        }        

        public int WallCount
        {
            get
            {
            	int wallCount = 0;
                if (northSide == SideType.Wall) wallCount++;
                if (southSide == SideType.Wall) wallCount++;
                if (westSide  == SideType.Wall)  wallCount++;
                if (eastSide  == SideType.Wall)  wallCount++;
                return wallCount;
            }
        }

	#endregion
	
	#region Methods	
		
        public DirectionType CalculateDeadEndCorridorDirection()
        {
            if (!IsDeadEnd) throw new Exception();

            if (northSide == SideType.Empty) return DirectionType.North;
            if (southSide == SideType.Empty) return DirectionType.South;
            if (westSide  == SideType.Empty) return DirectionType.West;
            if (eastSide  == SideType.Empty) return DirectionType.East;

            throw new Exception();
        }

	#endregion
}
