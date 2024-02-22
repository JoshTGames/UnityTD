using System;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// Allows entities to perform various actions
    /// </summary>
    public interface ITask{
        /// <summary>
        /// Places a task on an entity with a position and action
        /// </summary>
        /// <param name="position">The position the entity will move to</param>
        /// <param name="action">The action an entity wants to perform</param>
        public void SetTask(Vector3 position, Func<EntityERR> action = null);

        /// <summary>
        /// Gets the interaction radius of the entity 
        /// </summary>
        public float GetInteractRadius();
    }
}