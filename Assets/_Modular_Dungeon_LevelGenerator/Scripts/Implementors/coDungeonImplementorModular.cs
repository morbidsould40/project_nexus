// Copyright (C) 2012 Vertex Stream Games
// Only for use, modification and/or re-distribution in a compiled project.
// No re-sale of modified code permitted.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class coDungeonImplementorModular
	: coDungeonImplementor
{
	#region Enums	

		enum enCellDirection
		{
			North,
			South,
			East,
			West,
		};
	
	#endregion
	
	#region Members	
	
		public GameObject m_prefab_Floor;		
		public GameObject m_prefab_Wall;
		public GameObject m_prefab_Pillar;
		public GameObject m_prefab_Door;
		public GameObject m_prefab_Doorway;
		public GameObject m_prefab_Torch;
		public GameObject m_prefab_Gargoyle;
		public GameObject m_prefab_Dirt;

		public  float m_scale   = 1.515f;
		private float m_scaleH  = 0.0f;
	
	#endregion
	
	#region Properties			

		public GameObject Floor
		{
			get { return m_prefab_Floor;  }
			set { m_prefab_Floor = value; }
		}

		public GameObject Wall
		{
			get { return m_prefab_Wall;  }
			set { m_prefab_Wall = value; }
		}

		public GameObject Door
		{
			get { return m_prefab_Door;  }
			set { m_prefab_Door = value; }
		}

		public GameObject DoorWay
		{
			get { return m_prefab_Doorway;  }
			set { m_prefab_Doorway = value; }
		}
	
	#endregion
	
	#region Functions		
		
		public override bool ImplementDungeon()
		{	
			m_scaleH = m_scale / 2.0f;

			coDungeonGenerator dG
				= (coDungeonGenerator)
					transform.GetComponent
					(typeof(coDungeonGenerator));
		
			if (dG)
			{	
				foreach (Vector2 cell in dG.Dungeon.CellLocations)
				{
					
					if ((dG.Dungeon[cell].SouthSide == csDungeonCell.SideType.Wall &&
						 dG.Dungeon[cell].NorthSide == csDungeonCell.SideType.Wall &&
						 dG.Dungeon[cell].EastSide  == csDungeonCell.SideType.Wall &&
						 dG.Dungeon[cell].WestSide  == csDungeonCell.SideType.Wall))
					{
						Vector3 dirtPos = new Vector3(cell.x * m_scale, 1.0f, cell.y * m_scale);																

						GameObject dirt = 
							(GameObject)Instantiate(this.m_prefab_Dirt, dirtPos, Quaternion.Euler(-90,0,0));

						dirt.transform.parent = this.transform;
					}
					
					if (!(dG.Dungeon[cell].SouthSide == csDungeonCell.SideType.Wall &&
						  dG.Dungeon[cell].NorthSide == csDungeonCell.SideType.Wall &&
						  dG.Dungeon[cell].EastSide  == csDungeonCell.SideType.Wall &&
						  dG.Dungeon[cell].WestSide  == csDungeonCell.SideType.Wall))
					{				
						Vector3	pos = new Vector3(cell.x*m_scale, 0, cell.y*m_scale);										
					
						GameObject floor = 
							(GameObject)Instantiate(this.m_prefab_Floor, pos, Quaternion.Euler(-90,0,0));
	
						floor.transform.parent = this.transform;						
	
						if (dG.Dungeon[cell].SouthSide == csDungeonCell.SideType.Wall)					
						{
							Vector3 wallPos = pos;
							wallPos.z += m_scaleH;

							if (!ObjectExists(wallPos))
							{	
								GameObject wallS = 
									(GameObject)Instantiate
										(this.m_prefab_Wall, wallPos, Quaternion.Euler(-90,-90,0));
		
								wallS.transform.parent = this.transform;	

								if (m_prefab_Torch && Random.Range(0.0f, 1.0f)>0.5f)
								{
									Vector3 torchPos = wallPos;
									torchPos.y += 0.75f;
									torchPos.z -= 0.05f;

									GameObject torchS =
										(GameObject)Instantiate
											(this.m_prefab_Torch, torchPos, Quaternion.Euler(0,90,0));
		
									torchS.transform.parent = this.transform;	
								}
							}					
						}		

						if (dG.Dungeon[cell].NorthSide == csDungeonCell.SideType.Wall)					
						{
							Vector3 wallPos = pos;
							wallPos.z -= m_scaleH;

							if (!ObjectExists(wallPos))
							{
	
								GameObject wallS = 
									(GameObject)Instantiate
										(this.m_prefab_Wall, wallPos, Quaternion.Euler(-90,-90,0));
		
								wallS.transform.parent = this.transform;	

								if (m_prefab_Torch && Random.Range(0.0f, 1.0f)>0.5f)
								{
									Vector3 torchPos = wallPos;
									torchPos.y += 0.75f;
									torchPos.z += 0.05f;

									GameObject torchS =
										(GameObject)Instantiate
											(this.m_prefab_Torch, torchPos, Quaternion.Euler(0,-90,0));
		
									torchS.transform.parent = this.transform;	
								}
							}					
						}		

						if (dG.Dungeon[cell].EastSide == csDungeonCell.SideType.Wall)					
						{
							Vector3 wallPos = pos;
							wallPos.x += m_scaleH;

							if (!ObjectExists(wallPos))
							{
	
								GameObject wallS = 
									(GameObject)Instantiate
										(this.m_prefab_Wall, wallPos, Quaternion.Euler(-90,-0,0));
		
								wallS.transform.parent = this.transform;	

								if (m_prefab_Torch && Random.Range(0.0f, 1.0f)>0.5f)
								{
									Vector3 torchPos = wallPos;
									torchPos.y += 0.75f;
									torchPos.x -= 0.05f;

									GameObject torchS =
										(GameObject)Instantiate
											(this.m_prefab_Torch, torchPos, Quaternion.Euler(0,180,0));
		
									torchS.transform.parent = this.transform;	
								}
							}					
						}		

						if (dG.Dungeon[cell].WestSide == csDungeonCell.SideType.Wall)					
						{
							Vector3 wallPos = pos;
							wallPos.x -= m_scaleH;

							if (!ObjectExists(wallPos))
							{
	
								GameObject wallS = 
									(GameObject)Instantiate
										(this.m_prefab_Wall, wallPos, Quaternion.Euler(-90,-0,0));
		
								wallS.transform.parent = this.transform;

								if (m_prefab_Torch && Random.Range(0.0f, 1.0f)>0.5f)
								{
									Vector3 torchPos = wallPos;
									torchPos.y += 0.75f;
									torchPos.x += 0.05f;

									GameObject torchS =
										(GameObject)Instantiate
											(this.m_prefab_Torch, torchPos, Quaternion.Euler(0,0,0));
		
									torchS.transform.parent = this.transform;	
								}	
							}					
						}		
	
						if (dG.Dungeon[cell].SouthSide == csDungeonCell.SideType.Door)					
						{
							Vector3 wallPos = pos;
							wallPos.z += m_scaleH;

							if (!ObjectExists(wallPos))
							{	
								GameObject wallS = 
									(GameObject)Instantiate(this.m_prefab_Doorway, wallPos, Quaternion.Euler(-90,-90,0));
		
								wallS.transform.parent = this.transform;

								Vector3 doorPos = wallPos;
								doorPos.x += m_scaleH * 0.33f;

								GameObject doorS = 
									(GameObject)Instantiate(this.m_prefab_Door, doorPos, Quaternion.Euler(-0,-0,0));
		
								doorS.transform.parent = this.transform;
							}												
						}			

						if (dG.Dungeon[cell].NorthSide == csDungeonCell.SideType.Door)					
						{
							Vector3 wallPos = pos;
							wallPos.z -= m_scaleH;

							if (!ObjectExists(wallPos))
							{	
								GameObject wallS = 
									(GameObject)Instantiate(this.m_prefab_Doorway, wallPos, Quaternion.Euler(-90,-90,0));
		
								wallS.transform.parent = this.transform;

								Vector3 doorPos = wallPos;
								doorPos.x += m_scaleH * 0.33f;

								GameObject doorS = 
									(GameObject)Instantiate(this.m_prefab_Door, doorPos, Quaternion.Euler(-0,-0,0));
		
								doorS.transform.parent = this.transform;
							}												
						}			

						if (dG.Dungeon[cell].EastSide == csDungeonCell.SideType.Door)					
						{
							Vector3 wallPos = pos;
							wallPos.x += m_scaleH;

							if (!ObjectExists(wallPos))
							{	
								GameObject wallS = 
									(GameObject)Instantiate(this.m_prefab_Doorway, wallPos, Quaternion.Euler(-90,-0,0));
		
								wallS.transform.parent = this.transform;

								Vector3 doorPos = wallPos;
								doorPos.z += m_scaleH * 0.33f;

								GameObject doorS = 
									(GameObject)Instantiate(this.m_prefab_Door, doorPos, Quaternion.Euler(-0,-90,0));
		
								doorS.transform.parent = this.transform;
							}												
						}			

						if (dG.Dungeon[cell].WestSide == csDungeonCell.SideType.Door)					
						{
							Vector3 wallPos = pos;
							wallPos.x -= m_scaleH;

							if (!ObjectExists(wallPos))
							{	
								GameObject wallS = 
									(GameObject)Instantiate(this.m_prefab_Doorway, wallPos, Quaternion.Euler(-90,-0,0));
		
								wallS.transform.parent = this.transform;

								Vector3 doorPos = wallPos;
								doorPos.z += m_scaleH * 0.33f;

								GameObject doorS = 
									(GameObject)Instantiate(this.m_prefab_Door, doorPos, Quaternion.Euler(-0,-90,0));
		
								doorS.transform.parent = this.transform;
							}												
						}	

						Vector3 pillarPos = Vector3.zero;

						pillarPos = pos;						
						pillarPos.z += m_scaleH;
						pillarPos.x += m_scaleH;					
						
						if (!ObjectExists(pillarPos) 							
							&& (dG.Dungeon[cell].SouthSide != csDungeonCell.SideType.Empty
							||  dG.Dungeon[cell].EastSide  != csDungeonCell.SideType.Empty))
						{	
							GameObject pillar = 
								(GameObject)Instantiate(this.m_prefab_Pillar, pillarPos, Quaternion.Euler(-90,-0,0));
	
							pillar.transform.parent = this.transform;
						}		
					
						pillarPos = pos;
						pillarPos.z -= m_scaleH;
						pillarPos.x -= m_scaleH;

						if (!ObjectExists(pillarPos) 							
							&& (dG.Dungeon[cell].NorthSide != csDungeonCell.SideType.Empty
							||  dG.Dungeon[cell].WestSide  != csDungeonCell.SideType.Empty))
						{	
							GameObject pillar = 
								(GameObject)Instantiate(this.m_prefab_Pillar, pillarPos, Quaternion.Euler(-90,-0,0));
	
							pillar.transform.parent = this.transform;
						}		

						pillarPos = pos;
						pillarPos.z += m_scaleH;
						pillarPos.x -= m_scaleH;

						if (!ObjectExists(pillarPos) 							
							&& (dG.Dungeon[cell].SouthSide != csDungeonCell.SideType.Empty
							||  dG.Dungeon[cell].WestSide  != csDungeonCell.SideType.Empty))
						{	
							GameObject pillar = 
								(GameObject)Instantiate(this.m_prefab_Pillar, pillarPos, Quaternion.Euler(-90,-0,0));
	
							pillar.transform.parent = this.transform;
						}	

						pillarPos = pos;
						pillarPos.z -= m_scaleH;
						pillarPos.x += m_scaleH;

						if (!ObjectExists(pillarPos) 							
							&& (dG.Dungeon[cell].NorthSide != csDungeonCell.SideType.Empty
							||  dG.Dungeon[cell].EastSide  != csDungeonCell.SideType.Empty))
						{		
							GameObject pillar = 
								(GameObject)Instantiate(this.m_prefab_Pillar, pillarPos, Quaternion.Euler(-90,-0,0));
	
							pillar.transform.parent = this.transform;
						}	

						Vector3 gargoylePos = Vector3.zero;


						gargoylePos = pos;						
						gargoylePos.y += 0.5f;

						if (!ObjectExists(gargoylePos) && IsBigRoom(dG, cell)
							&& (dG.Dungeon[cell].SouthSide == csDungeonCell.SideType.Wall
							&&  dG.Dungeon[cell].EastSide  == csDungeonCell.SideType.Wall))
						{		
							GameObject gargoyle = 
								(GameObject)Instantiate(this.m_prefab_Gargoyle, gargoylePos, Quaternion.Euler(-90,135,0));
	
							gargoyle.transform.parent = this.transform;
						}		
	
						if (!ObjectExists(gargoylePos) && IsBigRoom(dG, cell)
							&& (dG.Dungeon[cell].SouthSide == csDungeonCell.SideType.Wall
							&&  dG.Dungeon[cell].WestSide  == csDungeonCell.SideType.Wall))
						{		
							GameObject gargoyle = 
								(GameObject)Instantiate(this.m_prefab_Gargoyle, gargoylePos, Quaternion.Euler(-90,45,0));
	
							gargoyle.transform.parent = this.transform;
						}	

						if (!ObjectExists(gargoylePos) && IsBigRoom(dG, cell)
							&& (dG.Dungeon[cell].NorthSide == csDungeonCell.SideType.Wall
							&&  dG.Dungeon[cell].EastSide  == csDungeonCell.SideType.Wall))
						{		
							GameObject gargoyle = 
								(GameObject)Instantiate(this.m_prefab_Gargoyle, gargoylePos, Quaternion.Euler(-90,235,0));
	
							gargoyle.transform.parent = this.transform;
						}		

						if (!ObjectExists(gargoylePos) && IsBigRoom(dG, cell)
							&& (dG.Dungeon[cell].NorthSide == csDungeonCell.SideType.Wall
							&&  dG.Dungeon[cell].WestSide  == csDungeonCell.SideType.Wall))
						{		
							GameObject gargoyle = 
								(GameObject)Instantiate(this.m_prefab_Gargoyle, gargoylePos, Quaternion.Euler(-90,315,0));
	
							gargoyle.transform.parent = this.transform;
						}									
					}					
				}			
			
				return true;
			}
		
			return false;		
		}
	
		private bool ValidLocation(coDungeonGenerator dG, float x, float y)
		{
			foreach (Vector2 cell in dG.Dungeon.CellLocations)
			{
				if (cell.x == x && cell.y == y) return true;
			}

			return false;
		}

		private bool ObjectExists(Vector3 pos)
		{
			foreach (Transform t in transform)
			{
				if (t.position == pos) 
					return true;
			}
			return false;
		}
	
		private bool IsBigRoom(coDungeonGenerator dG, Vector2 pos)
		{	
			foreach (csDungeonRoom room in dG.Dungeon.Rooms)
			{	
				if (room.Bounds.width != 1 && room.Bounds.height != 1)					
				{			
					if (room.Bounds.Contains(pos))
						return true;					
				}
			}					
		
			return false;
		}	
	
	#endregion
}

#endif