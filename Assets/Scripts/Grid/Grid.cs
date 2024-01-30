using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.GridSystem{
    public static class Grid{
        /// <summary>
        /// The size of each cell in 3D space
        /// </summary>
        public static int CELL_SIZE = 1;

        /// <summary>
        /// Used to help find the center of a position
        /// </summary>
        public static float HALF_CELL_SIZE{ get => (float)CELL_SIZE / 2; }

        static int RoundToGrid(float x) => Mathf.RoundToInt(x / CELL_SIZE) * CELL_SIZE;
        static int FloorToGrid(float x) => Mathf.FloorToInt(x / CELL_SIZE) * CELL_SIZE;
        static int CeilToGrid(float x) => Mathf.CeilToInt(x / CELL_SIZE) * CELL_SIZE;

        /// <summary>
        /// Rounds the parsed position to the grid
        /// </summary>
        /// <param name="value">The position you want to round to the grid</param>
        /// <returns>The rounded position</returns>
        public static Vector3 RoundToGrid(Vector3 value) => new Vector3(RoundToGrid(value.x), RoundToGrid(value.y), RoundToGrid(value.z));
        /// <summary>
        /// Floors the parsed position to the grid || Useful for checking which cell an entity is within
        /// </summary>
        /// <param name="value">The position you want to floor to the grid</param>
        /// <returns>The floored position</returns>
        public static Vector3 FloorToGrid(Vector3 value) => new Vector3(FloorToGrid(value.x), FloorToGrid(value.y), FloorToGrid(value.z));
        /// <summary>
        /// Ceils the parsed position to the grid
        /// </summary>
        /// <param name="value">The position you want to ceil to the grid</param>
        /// <returns>The ceiled position</returns>
        public static Vector3 CeilToGrid(Vector3 value) => new Vector3(CeilToGrid(value.x), CeilToGrid(value.y), CeilToGrid(value.z));
    }
}