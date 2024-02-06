using UnityEngine;
using System.Collections.Generic;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    public abstract class Character : Entity{
        /// <summary>
        /// The max speed this character can move
        /// </summary>
        protected float maxMoveSpeed{
            get;
            private set;
        }

        /// <summary>
        /// The damage this character will inflict
        /// </summary>
        protected int damage{
            get;
            private set;
        }

        /// <summary>
        /// How long the character has to wait until it can attack again 
        /// </summary>
        protected float attackCooldown{
            get;
            private set;
        }

        /// <summary>
        /// The distance between this character and another entity from where an attack can be made
        /// </summary>
        protected float attackRadius{
            get;
            private set;
        }

        /// <summary>
        /// Each character can hold only a single item. However, items can have variable quantity associated to them
        /// </summary>
        protected Item heldItem;

        /// <summary>
        /// Constructs this character
        /// </summary>
        /// <param name="owner">The owner of this entity</param>
        /// <param name="obj">The transform associated to this entity</param>
        /// <param name="maxHealth">The max health this object has</param>
        /// <param name="resistances">Any resistances appended here will stop the infliction and reduce the damage by 2/3</param>
        /// <param name="maxMoveSpeed">The max speed this character can move</param>
        /// <param name="damage">The damage this character will inflict</param>
        /// <param name="attackCooldown">How long the character has to wait until it can attack again</param>
        /// <param name="attackRadius">The distance between this character and another entity from where an attack can be made</param>
        /// <param name="isImmortal">If true, this object cannot take damage</param>
        /// <returns></returns>
        protected Character(string owner, Transform obj, int maxHealth, HashSet<DamageTypes> resistances, float maxMoveSpeed, int damage, float attackCooldown, float attackRadius, bool isImmortal = false) : base(owner, obj, maxHealth, resistances, isImmortal){            
            this.maxMoveSpeed = maxMoveSpeed;
            this.damage = damage;
            this.attackCooldown = attackCooldown;
            this.attackRadius = attackRadius;
        }

        /// <summary>
        /// Attempts to attack the parsed entity
        /// </summary>
        /// <param name="entity">The entity we want to attack</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        protected abstract ERR Attack(Entity entity);

        /// <summary>
        /// Attempts to harvest from a resource
        /// </summary>
        /// <param name="resource">The resource we want to harvest from</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        protected abstract ERR Harvest(Resource resource);

        /// <summary>
        /// Attempts to pickup an item 
        /// </summary>
        /// <param name="item">The item we wish to pick up</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        protected abstract ERR Pickup(Item item);
    }
}