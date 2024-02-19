using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using AstralCandle.TowerDefence.AI;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    /// <summary>
    /// Inheriting from 'StructureEntity' appends
    /// </summary>
    public abstract class TowerEntity : StructureEntity{
        [SerializeField, Tooltip("The layers our entities can be found on")] protected LayerMask entityLayer;
        [SerializeField, Tooltip("The damage this entity can do")] int maxDamage = 5;
        [SerializeField, Tooltip("The duration inbetween attacks")] protected double attackCooldown = 2;
        [SerializeField, Tooltip("The max distance from where we can attack a target")] float attackDistance = 0.5f;
        [SerializeField, Tooltip("The attack type we will inflict upon targets")] protected DamageType attackType;
        [SerializeField, Tooltip("The type of target this entity will prioritise")] protected Targetting targetting; 
        
        /// <summary>
        /// The cooldown until the entity can attack again
        /// </summary>
        double cooldown;

        /// <summary>
        /// The amount of damage to inflict upon enemies based on the structure efficiency
        /// </summary>
        protected int damage{ get => Mathf.FloorToInt(structureEfficiency * maxDamage); }

        /// <summary>
        /// Attempts to attack a parsed entity
        /// </summary>
        /// <param name="entity">The entity to attack</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        protected virtual ERR Attack(HealthEntity entity){
            ERR success = CanAttack(entity);
            if(success != ERR.SUCCESS){ return success; }

            cooldown = Time.timeAsDouble + attackCooldown;
            return success;
        }

        /// <summary>
        /// Queries the entity to see if we can attack them
        /// </summary>
        /// <param name="entity">The entity, we are querying if we can attack</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        protected ERR CanAttack(HealthEntity entity){
            if(!entity){ return ERR.INVALID_CALL; }
            else if((entity.transform.position - transform.position).magnitude > attackDistance){ return ERR.NOT_IN_RANGE; }
            else if(Time.timeAsDouble < cooldown){ return ERR.UNDER_COOLDOWN; }
            else if(entity.owner == owner){ return ERR.NO_FRIENDLY_FIRE; }
            else if(occupants <= 0){ return ERR.NO_OCCUPANTS; }

            return ERR.SUCCESS;
        }

        /// <summary>
        /// Collects all enemies within attack radius
        /// </summary>
        /// <returns>A list of all the enemies within reach</returns>
        protected List<HealthEntity> FindEnemies(){
            Collider[] entities = Physics.OverlapSphere(transform.position, attackDistance, entityLayer);
            List<HealthEntity> enemies = new List<HealthEntity>();
            foreach(Collider col in entities){
                HealthEntity hE = col.GetComponent<HealthEntity>();

                // Guard clause
                if(!hE || hE.owner == owner){ continue; }
                enemies.Add(hE);
            }
            return enemies;
        }

        protected override bool OnDrawGizmos(){
            if(base.OnDrawGizmos() == false){ return false; }
            #if UNITY_EDITOR
            // Entity
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.up, attackDistance);
            #endif
            return true;
        }
    
        /// <summary>
        /// The type of target this entity will prioritise
        /// </summary>
        [Serializable] public enum Targetting{
            CLOSEST,
            FURTHEST,
            STRONGEST
        }
    }
}