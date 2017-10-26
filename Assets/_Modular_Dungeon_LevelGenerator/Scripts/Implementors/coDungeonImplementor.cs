// Copyright (C) 2012 Vertex Stream Games
// Only for use, modification and/or re-distribution in a compiled project.
// No re-sale of modified code permitted.

#if UNITY_EDITOR

using UnityEngine;
using System.Collections;

public abstract class coDungeonImplementor 
	: MonoBehaviour 
{	
	public virtual bool ImplementDungeon()
	{
		return false;
	}
}

#endif