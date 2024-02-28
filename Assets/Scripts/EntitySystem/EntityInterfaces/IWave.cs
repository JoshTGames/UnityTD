
/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

using UnityEngine;

namespace AstralCandle.Entity{
    /// <summary>
    /// Allows entities to be used correctly with the wave system
    /// </summary>
    public interface IWave{
        /// <summary>
        /// Attempts to remove this entity from the loop
        /// </summary>
        public void RemoveEntityFromWave(int id) => GameLoop.instance?.CurrentGame?.RemoveEntity(id);
    }
}