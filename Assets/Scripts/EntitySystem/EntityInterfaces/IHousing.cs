
/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// Allows entities to house other entities
    /// </summary>
    public interface IHousing{
        public EntityERR AddOccupant(Entity entity);
    }
}