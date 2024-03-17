
/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// Allows entities to harvest entities with 'IHealth' 
    /// </summary>
    public interface IHarvest{
        /// <summary>
        /// Attempts to attack an entity through harvest
        /// </summary>
        /// <param name="entity">The entity we want to harvest</param>
        /// <param name="ignoreCooldown">If true, will ignore the cooldown</param>
        /// <return>An EntityERR code displaying if it was successful or not</return>
        public EntityERR Harvest(EntityHealth entity, bool ignoreCooldown = false);

        /// <summary>
        /// Returns the cooldown till this entity can harvest
        /// </summary>
        public float GetCooldown();
    }
} 