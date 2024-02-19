
/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// Provides health functionality
    /// </summary>
    public interface IHealth{
        /// <summary>
        /// Returns the health of this entity
        /// </summary>
        public float GetHealth();

        /// <summary>
        /// Raises the health of this entity
        /// </summary>
        /// <param name="value">The amount of health to add to entity</param>
        public void Heal(int value);

        /// <summary>
        /// Decreases the health of this entity
        /// </summary>
        /// <param name="value">The amount of damage to inflict on this entity</param>
        /// <param name="infliction">The damage type to inflict on this entity</param>
        /// <param name="attacker">The entity that is attacking this entity</param>
        public void Damage(int value, InflictionType infliction = InflictionType.BALLISTIC, Entity attacker = null);


        /// <summary>
        /// The various types of damage that can be inflicted upon an entity
        /// </summary>
        [System.Serializable] public enum InflictionType{
            BALLISTIC = 0,
            FIRE = 5,
            BLEEDING = 3,
        }
    }
}