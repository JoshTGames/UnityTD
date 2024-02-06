using UnityEngine;
using System.Collections.Generic;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    public abstract class Tower : Structure{
        int maxDamage;

        /// <summary>
        /// How long the tower has to wait until it can attack again 
        /// </summary>
        protected float attackCooldown{
            get;
            private set;
        }

        /// <summary>
        /// The detection radius of this tower
        /// </summary>
        protected float attackRadius{
            get;
            private set;
        }

        /// <summary>
        /// The type of damage this tower inflicts on enemies
        /// </summary>
        protected DamageTypes damageType{
            get;
            private set;
        }

        /// <summary>
        /// Dictates the type of enemies this tower will prioritise
        /// </summary>
        public Targetting strategy;
        

        /// <summary>
        /// Constructs this tower
        /// </summary>
        /// <param name="owner">The owner of this entity</param>
        /// <param name="obj">The transform associated to this entity</param>
        /// <param name="maxHealth">The max health this object has</param>
        /// <param name="resistances">Any resistances appended here will stop the infliction and reduce the damage by 2/3</param>
        /// <param name="maxOccupants">The max occupants which can sit inside this structure</param>
        /// <param name="maxDamage">The maximum possible damage this structure can do</param>
        /// <param name="attackCooldown">How long the tower has to wait until it can attack again</param>
        /// <param name="attackRadius">The detection radius of this tower</param>
        /// <param name="strategy">Dictates the type of enemies this tower will prioritise</param>
        /// <param name="isImmortal">If true, this object cannot take damage</param>
        protected Tower(string owner, Transform obj, int maxHealth, HashSet<DamageTypes> resistances, int maxOccupants, int maxDamage, int attackCooldown, float attackRadius, DamageTypes damageType, Targetting strategy, bool isImmortal = false) : base(owner, obj, maxHealth, resistances, maxOccupants, isImmortal){
            this.maxDamage = maxDamage;
            this.attackCooldown = attackCooldown;
            this.attackRadius = attackRadius;
            this.damageType = damageType;
            this.strategy = strategy;
        }

        /// <summary>
        /// The amount of damage to inflict upon enemies based on the structure efficiency
        /// </summary>
        protected int damage{ get => Mathf.FloorToInt(structureEfficiency * maxDamage); }

        /// <summary>
        /// Attempts to attack the parsed entity
        /// </summary>
        /// <param name="entity">The entity we want to attack</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        protected abstract ERR Attack(Entity entity);

        /// <summary>
        /// Dictates the type of enemies this tower will prioritise
        /// </summary>
        public enum Targetting{
            CLOSEST = 0,
            FURTHEST = 1,
            STRONGEST = 2
        }
    }
}