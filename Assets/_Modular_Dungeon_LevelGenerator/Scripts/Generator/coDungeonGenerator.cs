// Copyright (C) 2012 Vertex Stream Games
// Only for use, modification and/or re-distribution in a compiled project.
// No re-sale of modified code permitted.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode()]
public class coDungeonGenerator 
	: MonoBehaviour 
{
	#region Enums	
		
		public enum 
			enDungeonSize
		{
			Tiny 	= 4,
			Small   = 8,
			Medium  = 12,
			Large   = 24,
			Huge    = 32,
		};	
	
		public enum 
			enRoomSize
		{
			Tiny   = 1,
			Small  = 2,			
			Medium = 3,
			Large  = 4,
			Huge   = 5,
		};
		
		public enum 
			enCorridors
		{
			Maze 		 = 30,
			Dungeon		 = 60,	
			Sparse		 = 80,		
		}	
	
		public enum 
			enTwists
		{
			Straight = 0,
			Minor    = 50,
			Major    = 100,			
		};
	
		public enum 
			enDeadEnds
		{
			Allow  = 0,
			Remove = 100,
		}
	
	#endregion
	
	#region Members
	
		public csDungeon 	 m_dungeon 	   = null;	
		public enDungeonSize m_dungeonSize = enDungeonSize.Small;	
		public int 			 m_roomCount   = 5;		
		public enRoomSize	 m_roomSizeMIN = enRoomSize .Small;
		public enRoomSize 	 m_roomSizeMAX = enRoomSize .Small;		
		public enCorridors	 m_corridors   = enCorridors.Dungeon;
		public enTwists 	 m_twists      = enTwists	.Minor;			
		public enDeadEnds    m_deadEnds    = enDeadEnds .Allow;		
	
	#endregion
	
	#region Properties
	
		/// <summary>
		/// Gets the dungeon.
		/// </summary>
		/// <value>
		/// The dungeon.
		/// </value>
		public csDungeon Dungeon
		{
			get { return m_dungeon;  }			
		}	
	
		/// <summary>
		/// Gets or sets the size of the dungeon.
		/// </summary>
		/// <value>
		/// The size of the dungeon.
		/// </value>
		public enDungeonSize DungeonSize
		{
			get { return m_dungeonSize;  }
			set { m_dungeonSize = value; }
		}	
	
		/// <summary>
		/// Gets or sets the room count.
		/// </summary>
		/// <value>
		/// The room count.
		/// </value>
		public int RoomCount
		{
			get { return m_roomCount;  }
			set 
			{ 
				m_roomCount = Mathf.Clamp(value, 1, 25); 
			}
		}
	
		/// <summary>
		/// Gets or sets the MIN room size.
		/// </summary>
		/// <value>
		/// The MIN room size.
		/// </value>
		public enRoomSize RoomSizeMIN
		{
			get { return m_roomSizeMIN;  }
			set { m_roomSizeMIN = value; }
		}
	
		/// <summary>
		/// Gets or sets the MAX room size.
		/// </summary>
		/// <value>
		/// The MAX room size.
		/// </value>
		public enRoomSize RoomSizeMAX
		{
			get { return m_roomSizeMAX;  }
			set { m_roomSizeMAX = value; }
		}
		
		/// <summary>
		/// Gets or sets the corridor type.
		/// </summary>
		/// <value>
		/// The corridor type.
		/// </value>
		public enCorridors Corridors
		{
			get { return m_corridors;  }
			set { m_corridors = value; }
		}
	
		/// <summary>
		/// Gets or sets the twist type.
		/// </summary>
		/// <value>
		/// The twist type.
		/// </value>
		public enTwists Twists
		{
			get { return m_twists;  }
			set { m_twists = value; }
		}
		
		/// <summary>
		/// Gets or sets the dead end rule.
		/// </summary>
		/// <value>
		/// The dead end rule.
		/// </value>
		public enDeadEnds DeadEnds
		{
			get { return m_deadEnds;  }
			set { m_deadEnds = value; }
		}				
		
	#endregion
	
	#region Functions	
	
		/// <summary>
		/// Destroy the existing Dungeon.
		/// </summary>
		public void DestroyDungeon()
		{
			m_dungeon = null;
		
			List<GameObject> children 
				= new List<GameObject>();
			
			foreach (Transform child in this.transform)
				children.Add(child.gameObject);
			
			children.ForEach(child => 
				GameObject.DestroyImmediate(child));

			// Thanks to Andy Leedy for pointing out this memory leak / crash!
		 	children.Clear();
       		children.TrimExcess();
		}
	
		/// <summary>
		/// Creates a new Dungeon
		/// </summary>
		public void CreateDungeon()
		{
			// Delete existing dungeon
			DestroyDungeon();
		
			// Create Room Generator
			csDungeonRoomGenerator rG 
				= new csDungeonRoomGenerator();			
			
			// Initialise Room Generator
			rG.Constructor
				(m_roomCount, 
				 (int)m_roomSizeMIN,
				 (int)m_roomSizeMAX,
				 (int)m_roomSizeMIN,
				 (int)m_roomSizeMAX);				
		
			// Create Dungeon Generator
			csDungeonGenerator dG 
				= new csDungeonGenerator();
		
			// Initialise Dungeon Generator
			dG.Constructor
				((int)m_dungeonSize,
				 (int)m_dungeonSize,
				 (int)m_twists,
			     (int)m_corridors,
				 (int)m_deadEnds,
				 rG);		 
			
			// Create a Dungeon
			m_dungeon = dG.Generate();		
		}	
	
	#endregion
}

#endif