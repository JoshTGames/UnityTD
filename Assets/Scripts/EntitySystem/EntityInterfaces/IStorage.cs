
/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// Allows entities to deposit/withdraw resources from one another
    /// </summary>
    public interface IStorage{
        /// <summary>
        /// Attempts to deposit a resource into this entity
        /// </summary>
        /// <param name="resource">The resource we want to deposit</param>
        /// <param name="entity">The entity that is holding the resource we want to deposit</param>
        /// <returns>An EntityERR code displaying if it was successful or not</returns>
        public EntityERR Deposit(ResourceData.Resource resource, IPickup entity);
        /// <summary>
        /// Attempts to withdraw a resource from this entity
        /// </summary>
        /// <param name="resource">The resource we want to withdraw</param>
        /// <returns>An EntityERR code displaying if it was successful or not</returns>
        public EntityERR Withdraw(ResourceData.Resource resource, IStorage storage);

        /// <summary>
        /// Should return the held resource
        /// </summary>
        public ResourceData.Resource GetResource();
    }
}