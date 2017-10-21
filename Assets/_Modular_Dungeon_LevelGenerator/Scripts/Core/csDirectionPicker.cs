// Copyright (C) 2012 Vertex Stream Games
// Only for use, modification and/or re-distribution in a compiled project.
// No re-sale of modified code permitted.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csDirectionPicker 	
{
	#region Members

        private List<csDungeonCell.DirectionType> 
			directionsPicked 
				= new List<csDungeonCell.DirectionType>();
	
        private csDungeonCell.DirectionType 
			previousDirection;
        
		private int changeDirectionModifier;

	#endregion	
	
	#region Properties

        public bool HasNextDirection
        {
            get { return directionsPicked.Count < 4; }
        }

        private bool MustChangeDirection
        {
            get
            {
                // changeDirectionModifier of 100 will always change direction
                // value of 0 will never change direction
                return ((directionsPicked.Count > 0) || (changeDirectionModifier > csRandom.Instance.Next(0, 99)));
            }
        }
        
	#endregion

    #region Methods

        private csDungeonCell.DirectionType PickDifferentDirection()
        {
            csDungeonCell.DirectionType directionPicked;
            do
            {
                directionPicked = (csDungeonCell.DirectionType)csRandom.Instance.Next(3);
            } 
			while ((directionPicked == previousDirection) && (directionsPicked.Count < 3));

            return directionPicked;
        }

        public csDungeonCell.DirectionType GetNextDirection()
        {
            if (!HasNextDirection) throw new InvalidOperationException("No directions available");

            csDungeonCell.DirectionType directionPicked;

            do
            {
                directionPicked = MustChangeDirection ? PickDifferentDirection() : previousDirection;
            } while (directionsPicked.Contains(directionPicked));

            directionsPicked.Add(directionPicked);

            return directionPicked;
        }

        #endregion
	
	#region Constructor

        public void Constructor
			(csDungeonCell.DirectionType previousDirection, int changeDirectionModifier)
        {
            this.previousDirection = previousDirection;
            this.changeDirectionModifier = changeDirectionModifier;
        }

	#endregion

}
