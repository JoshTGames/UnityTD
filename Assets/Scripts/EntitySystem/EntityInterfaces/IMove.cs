using System;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// Allows entities to move around the scene
    /// </summary>
    public interface IMove{
        /// <summary>
        /// Moves an entity to a position whilst attempting to perform an action should it be required
        /// </summary>
        /// <param name="position">The position the entity will move to</param>
        /// <param name="action">The action an entity wants to perform</param>
        public void Move(Vector3 position, Func<EntityERR> action = null);
    }
}