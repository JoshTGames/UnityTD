
/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// Essential to provide user -> entity interaction
    /// </summary>
    public interface ISelectable{
        /// <summary>
        /// Returns the name of the entity
        /// </summary>
        public string GetName();

        /// <summary>
        /// Returns information about the entity
        /// </summary>
        public string GetDescription();

        /// <summary>
        /// Dictates what should happen when this entity is being hovered over
        /// </summary>
        /// <param name="isHovered">Whether this entity is hovered</param>
        public void OnIsHovered(bool isHovered);

        /// <summary>
        /// Dictates what should happen when this entity is selected
        /// </summary>
        /// <param name="obj">If object is parsed, then this object is selected</param>
        public void OnIsSelected(SelectionCircle obj = null);

        /// <summary>
        /// Used to query if the entity is destroyed as only the unity instance of the object is destroyed when 'OnDestroy' initially 
        /// </summary>
        public bool IsDestroyed();              
    }
}