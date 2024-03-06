
/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// Allows entities to attack entities with 'IHealth' 
    /// </summary>
    public interface IAttack{
        /// <summary>
        /// Attempts to attack an entity
        /// </summary>
        /// <param name="entity">The entity we want to attack</param>
        /// <param name="ignoreCooldown">If true, will ignore the cooldown</param>
        /// <return>An EntityERR code displaying if it was successful or not</return>
        public EntityERR Attack(EntityHealth entity, bool ignoreCooldown = false);

        /// <summary>
        /// Returns the cooldown till this entity can attack
        /// </summary>
        public float GetCooldown();
    }
} 