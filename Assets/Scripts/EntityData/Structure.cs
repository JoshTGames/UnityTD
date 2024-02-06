using UnityEngine;
using System.Collections.Generic;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    public abstract class Structure : Entity{
        int occupants, maxOccupants;

        /// <summary>
        /// Constructs this structure
        /// </summary>
        /// <param name="owner">The owner of this entity</param>
        /// <param name="obj">The transform associated to this entity</param>
        /// <param name="maxHealth">The max health this object has</param>
        /// <param name="resistances">Any resistances appended here will stop the infliction and reduce the damage by 2/3</param>
        /// <param name="maxOccupants">The max occupants which can sit inside this structure</param>
        /// <param name="isImmortal">If true, this object cannot take damage</param>
        public Structure(string owner, Transform obj, int maxHealth, HashSet<DamageTypes> resistances, int maxOccupants, bool isImmortal = false) : base(owner, obj, maxHealth, resistances, isImmortal){
            this.maxOccupants = maxOccupants;
        }

        /// <summary>
        /// Attempts to add an occupant to this structure
        /// </summary>
        /// <returns>An Entity 'ERR' enum code</returns>
        public ERR AddOccupant(){
            if(occupants >= maxOccupants){ return ERR.MAX_OCCUPANCY; }
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

        /// <summary>
        /// Calculates the efficiency of this structure based on the number of occupants 
        /// </summary>
        protected float structureEfficiency{ get => (float)occupants / maxOccupants; }
    }
}