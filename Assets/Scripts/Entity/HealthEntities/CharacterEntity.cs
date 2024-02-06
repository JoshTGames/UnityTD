using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    /// <summary>
    /// Inheriting from 'HealthEntity' appends
    /// </summary>
    public abstract class CharacterEntity : HealthEntity{
        [SerializeField, Tooltip("The max velocity this entity can reach")] protected float maxMoveSpeed = 10;
        [SerializeField, Tooltip("The damage this entity can do")] protected int damage = 5;
        [SerializeField, Tooltip("The duration inbetween attacks")] protected float attackCooldown = 2;
        [SerializeField, Tooltip("The max distance from the target entity where this entity can attack")] float attackDistance = 0.5f;

        /// <summary>
        /// The item this entity is holding. | Can only hold a single item. However, item quantity is variable
        /// </summary>
        // protected ItemEntity heldItem;

        /// <summary>
        /// Attempts to attack a parsed entity
        /// </summary>
        /// <param name="entity">The entity to attack</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        protected abstract ERR Attack(HealthEntity entity);
        
        /// <summary>
        /// Attempts to harvest a parsed entity
        /// </summary>
        /// <param name="entity">The entity we wish to harvest from</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        // protected abstract ERR Harvest(ResourceEntity entity);

        /// <summary>
        /// Attempts to pickup a parsed entity
        /// </summary>
        /// <param name="entity">The entity we wish to pickup</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        // protected abstract ERR Pickup(ItemEntity entity);

        /// <summary>
        /// Attempts to move this entity to a desired position
        /// </summary>
        /// <param name="position">The world-space position the entity should travel to</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        protected abstract ERR MoveTo(Vector3 position);
    }
}