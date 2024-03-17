
/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// Allows entities to pickup items off the floor
    /// </summary>
    public interface IPickup{
        /// <summary>
        /// Attempts to pick up an item off the floor
        /// </summary>
        /// <param name="resource">The resource we want to pickup</param>
        /// <returns>An EntityERR code displaying if it was successful or not</returns>
        public EntityERR Pickup(EntityResource resource);

        /// <summary>
        /// Should return the held item
        /// </summary>
        public ResourceData.Resource GetEquipped();

        /// <summary>
        /// Should delete the equipped item
        /// </summary>
        public void RemoveEquipped();
    }
}