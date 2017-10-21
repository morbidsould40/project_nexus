// Copyright (C) 2012 Vertex Stream Games
// Only for use, modification and/or re-distribution in a compiled project.
// No re-sale of modified code permitted.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class csRandom
{
    #region Singleton

        private csRandom()
        {
        }

        public static csRandom Instance
        {
            get { return Nested.instance; }
        }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit

            internal static readonly csRandom instance = new csRandom();

            static Nested()
            {
            }
        }

    #endregion

    #region Fields

    	private readonly csMersenneTwister mersenneTwister = new csMersenneTwister();

    #endregion

    #region Methods

        public int Next(int maxValue)
        {
            return mersenneTwister.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return mersenneTwister.Next(minValue, maxValue);
        }

    #endregion
}