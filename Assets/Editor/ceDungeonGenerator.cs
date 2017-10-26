// Copyright (C) 2012 Vertex Stream Games
// Only for use, modification and/or re-distribution in a compiled project.
// No re-sale of modified code permitted.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
	
/// <summary>
/// Custom GUI for the Dungeon Generator 
/// </summary>
[CustomEditor(typeof(coDungeonGenerator))]
public class ceDungeonGenerator
	: Editor 
{
	#region Functions
	
		public override void OnInspectorGUI()
		{	
			coDungeonGenerator dG 
				= target as coDungeonGenerator;	
		
			coDungeonImplementor dI 
					= (coDungeonImplementor)
						dG.transform.GetComponent
							(typeof(coDungeonImplementor));			
			
			if (dI)
			{
				EditorGUILayout.Separator();		
				
				dG.DungeonSize = (coDungeonGenerator.enDungeonSize)EditorGUILayout.EnumPopup("Dungeon Size",   dG.DungeonSize);				
				dG.RoomCount   = 								   EditorGUILayout.IntField ("Max Room Count", dG.RoomCount  );						
			    dG.RoomSizeMIN = (coDungeonGenerator.enRoomSize   )EditorGUILayout.EnumPopup("Min Room Size ", dG.RoomSizeMIN);						
				dG.RoomSizeMAX = (coDungeonGenerator.enRoomSize   )EditorGUILayout.EnumPopup("Max Room Size",  dG.RoomSizeMAX);						
				dG.Corridors   = (coDungeonGenerator.enCorridors  )EditorGUILayout.EnumPopup("Corridors", 	   dG.Corridors  );			
				dG.Twists      = (coDungeonGenerator.enTwists     )EditorGUILayout.EnumPopup("Twists", 		   dG.Twists	 );	
			  //dG.DeadEnds    = (coDungeonGenerator.enDeadEnds   )EditorGUILayout.EnumPopup("Dead Ends",      dG.DeadEnds   );					
			
				EditorGUILayout.Separator();
			
				if(GUILayout.Button("Generate", GUILayout.Width(200)))
				{
					dG.DestroyDungeon();
					
					//dG.RoomSizeMIN = coDungeonGenerator.enRoomSize.Small;
					//dG.RoomSizeMAX = coDungeonGenerator.enRoomSize.Small;					
				
					bool bDungonOK = false;
					int  retries = 0;
				
					int oldRoomCount = dG.RoomCount;
				
					while (!bDungonOK && retries < 25)
					{					
						dG.CreateDungeon();				
						bDungonOK = dI.ImplementDungeon();
						if (bDungonOK)
						{
							dG.RoomCount = oldRoomCount;							
						}
						else
						{
							dG.RoomCount--;
							retries++;
							//Debug.LogWarning("Reducing Room Count and re-generating...");
						}					
					}
				
					
				
					if (!bDungonOK)
					{
						Debug.LogError("Tried 25 times to generate valid dungeon; gave up!");
					}
				}
				
				if(GUILayout.Button("Destroy", GUILayout.Width(200)))
				{
					dG.DestroyDungeon();			
				}
			}		
			
			if (GUI.changed)
				EditorUtility.SetDirty (target);
		}	
	
	#endregion 
}

#endif