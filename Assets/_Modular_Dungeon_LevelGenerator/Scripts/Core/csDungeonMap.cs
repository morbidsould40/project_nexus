// Copyright (C) 2012 Vertex Stream Games
// Only for use, modification and/or re-distribution in a compiled project.
// No re-sale of modified code permitted.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public abstract class csDungeonMap
    {
        #region Fields

        	protected csDungeonCell[,] cells;
        	protected Rect bounds;	

        #endregion

        #region Constructors

        protected void Constructor(int width, int height)
        {
            cells  = new csDungeonCell[width,height];
            bounds = new Rect(0, 0, width, height);

            // Initialize the array of cells
            foreach (Vector2 location in CellLocations)
                this[location] = new csDungeonCell();
        }

        #endregion

        #region Properties

        public Rect Bounds
        {
            get { return bounds; }
        }

        public csDungeonCell this[Vector2 point]
        {
            get { return this[point.x, point.y]; }
            set { this[point.x, point.y] = value; }
        }

        public csDungeonCell this[float x, float y]
        {
            get { return cells[(int)x, (int)y]; }
            set { cells[(int)x, (int)y] = value; }
        }

        public int Width
        {
            get { return (int)bounds.width; }
        }

        public int Height
        {
            get { return (int)bounds.height; }
        }

        public IEnumerable<Vector2> CellLocations
        {
            get
            {
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                        yield return new Vector2(x, y);
            }
        }

        #endregion

        #region Methods

        public bool HasAdjacentCellInDirection(Vector2 location, csDungeonCell.DirectionType direction)
        {
            // Check that the location falls within the bounds of the map
            if (!Bounds.Contains(location))
                return false;

            // Check if there is an adjacent cell in the direction
            switch (direction)
            {
                case csDungeonCell.DirectionType.North:
                    return location.y > 0;
                case csDungeonCell.DirectionType.South:
                    return location.y < (Height - 1);
                case csDungeonCell.DirectionType.West:
                    return location.x > 0;
                case csDungeonCell.DirectionType.East:
                    return location.x < (Width - 1);
                default:
                    return false;
            }
        }

        protected Vector2? GetTargetLocation(Vector2 location, csDungeonCell.DirectionType direction)
        {
            if (!HasAdjacentCellInDirection(location, direction)) return null;

            switch (direction)
            {
                case csDungeonCell.DirectionType.North:
                    return new Vector2(location.x, location.y - 1);
                case csDungeonCell.DirectionType.West:
                    return new Vector2(location.x - 1, location.y);
                case csDungeonCell.DirectionType.South:
                    return new Vector2(location.x, location.y + 1);
                case csDungeonCell.DirectionType.East:
                    return new Vector2(location.x + 1, location.y);
                default:
                    throw new InvalidOperationException();
            }
        }

        #endregion

    }
