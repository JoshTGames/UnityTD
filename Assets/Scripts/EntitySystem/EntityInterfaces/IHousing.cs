
/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// Allows entities to house other entities
    /// </summary>
    public interface IHousing{
        /// <summary>
        /// Gets the entity's efficiency based on how full the entity is at 
        /// </summary>
        public float GetEfficiency();

        /// <summary>
        /// Attempts to add an entity to this entity
        /// </summary>
        /// <param name="entity">The entity we wish to add to the structure</param>
        /// <returns>An EntityERR code displaying if it was successful or not</returns>
        public EntityERR AddOccupant(Entity entity);

        /// <summary>
        /// Attempts to remove an entity from this entity
        /// </summary>
        /// <param name="entity">The entity we wish to remove from this entity</param>
        /// <returns>An EntityERR code displaying if it was successful or not</returns>
        public EntityERR RemoveOccupant(out Entity entity);
    }
}