using UnityEngine;
using System.Collections.Generic;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    public abstract class Resource : Entity{
        public Item.ItemType itemType{
            get;
            private set;
        }

        /// <summary>
        /// Constructs this object
        /// </summary>
        /// <param name="owner">The owner of this entity</param>
        /// <param name="obj">The transform associated to this entity</param>
        /// <param name="maxHealth">The max health this object has</param>
        /// <param name="resistances">Any resistances appended here will stop the infliction and reduce the damage by 2/3</param>
        /// <param name="itemType">The items that will be able to be harvested from this resource</param>
        /// <param name="isImmortal">If true, this object cannot take damage</param>
        protected Resource(string owner, Transform obj, int maxHealth, HashSet<DamageTypes> resistances, Item.ItemType itemType, bool isImmortal = false) : base(owner, obj, maxHealth, resistances, isImmortal){
            this.itemType = itemType;
        }        
    }
}