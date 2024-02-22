using System.Collections;
using System.Collections.Generic;
using AstralCandle.TowerDefence;
using UnityEditor;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    public abstract class EntityDefensiveStructure : EntityStructure, IAttack{
        [Header("Attack Settings")]
        [SerializeField] LayerMask entityLayer;
        [SerializeField] IHealth.InflictionType damageInfliction;
        [SerializeField] int damage = 5;
        [SerializeField] float attackRadius = 2;
        [SerializeField] float cooldown = 1;
        float elapsedTime;

        protected EntityDefensiveStructure(int ownerId) : base(ownerId){}

        /// <summary>
        /// Queries the entity position against this to see if its within attacking range
        /// </summary>
        /// <param name="other">The position of the entity we want to attack</param>
        /// <returns>True/False depending on if the entity is within attacking range</returns>
        protected bool InRange(Vector3 other) => Vector3.Distance(transform.position, other) <= attackRadius;

        /// <summary>
        /// Searches for enemies within attack range of this entity
        /// </summary>
        /// <returns>A list of all enemies within range of this entity</returns>
        protected List<EntityHealth> FindEnemies(){
            Collider[] entities = Physics.OverlapSphere(transform.position, attackRadius, entityLayer);
            List<EntityHealth> enemies = new List<EntityHealth>();
            foreach(Collider col in entities){
                EntityHealth hE = col.GetComponent<EntityHealth>();

                // Guard clause
                if(!hE || hE.OwnerId == OwnerId){ continue; }
                enemies.Add(hE);
            }
            return enemies;
        }

        public virtual EntityERR Attack(EntityHealth entity){
            if(!entity){ return EntityERR.INVALID_CALL; }
            if(GetEfficiency() <= 0){ return EntityERR.NO_OCCUPANTS; }
            if(OwnerId == entity.OwnerId){ return EntityERR.IS_FRIENDLY; }
            else if(elapsedTime > 0){ return EntityERR.UNDER_COOLDOWN; }
            else if(!InRange(entity.transform.position)){ return EntityERR.NOT_IN_RANGE; }

            elapsedTime = cooldown;

            entity.Damage(Mathf.FloorToInt(damage * GetEfficiency()), damageInfliction, this);
            return EntityERR.SUCCESS;
        }

        public float GetCooldown() => Mathf.Clamp01(elapsedTime / cooldown);

        protected override void Run(){
            elapsedTime -= Time.fixedDeltaTime;
            List<EntityHealth> enemies = FindEnemies();
            if(enemies.Count <= 0){ return; }
            Attack(enemies[0]);
        }

        protected override bool OnDrawGizmos(){
            #if UNITY_EDITOR
            if(!base.OnDrawGizmos()){ return false; }

            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, transform.up, attackRadius);
            return true;
            #endif
        }
    }
}