using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    /// <summary>
    /// Inheriting from 'HealthEntity' appends
    /// </summary>
    public abstract class StructureEntity : HealthEntity{
        [SerializeField, Tooltip("The max amount of occupants this structure can hold")] int maxOccupants;
        protected int occupants{
            get;
            private set;
        }

        /// <summary>
        /// Calculates the efficiency of this structure based on the number of occupants 
        /// </summary>
        protected float structureEfficiency{ get => (float)occupants / maxOccupants; }

        /// <summary>
        /// Attempts to add an occupant to this structure
        /// </summary>
        /// <returns>An Entity 'ERR' enum code</returns>
        public ERR AddOccupant(){
            if(occupants >= maxOccupants){ return ERR.MAX_OCCUPANTS; }
            
            occupants++;
            return ERR.SUCCESS;
        }

        /// <summary>
        /// Attempts to remove an occupant from this structure
        /// </summary>
        /// <returns>An Entity 'ERR' enum code</returns>
        public ERR RemoveOccupant(){
            if(occupants <= 0){ return ERR.NO_OCCUPANTS; }
            occupants--;
            return ERR.SUCCESS;
        }
    }
}