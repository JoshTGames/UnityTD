using System;
using System.Collections;
using System.Collections.Generic;
using AstralCandle.Animation;
using AstralCandle.TowerDefence;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// EntityStructure with the ability to attack other entites
    /// </summary>
    public abstract class EntityDefensiveStructure : EntityStructure, IAttack{
        [Header("Attack Settings")]
        [SerializeField] LayerMask entityLayer;
        [SerializeField] IHealth.InflictionType damageInfliction;
        [SerializeField] int damage = 5;
        [SerializeField] protected float attackRadius = 2;
        [SerializeField] float projectileHeightOffset = 0;
        [SerializeField] float cooldown = 1;
        [SerializeField] GameObject projectile;
        [SerializeField] float projectileSpeed;
        [SerializeField] AnimationCurve easeInCurve;
        [SerializeField] float easeInDuration = 1;
        [SerializeField] AnimationInterpolation attackAnimationSettings;
        
        List<Projectile> activeProjectiles;
        float elapsedTime;

        protected EntityDefensiveStructure(int ownerId) : base(ownerId){}


        /// <summary>
        /// Queries the entity position against this to see if its within attacking range
        /// </summary>
        /// <param name="other">The position of the entity we want to attack</param>
        /// <returns>True/False depending on if the entity is within attacking range</returns>
        protected bool InRange(Vector3 other, float radius) => Vector3.Distance(_collider.ClosestPoint(other), other) <= radius;

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
                if(!hE || hE.OwnerId == OwnerId || hE.OwnerId < 0 || hE.MarkedForDestroy || !hE.isEnabled){ continue; }
                enemies.Add(hE);
            }
            return enemies;
        }

        public virtual EntityERR Attack(EntityHealth entity, bool ignoreCooldown = false){
            if(!entity || entity.MarkedForDestroy){ return EntityERR.INVALID_CALL; }
            else if(GetEfficiency() <= 0){ return EntityERR.NO_OCCUPANTS; }
            else if(OwnerId == entity.OwnerId){ return EntityERR.IS_FRIENDLY; }
            else if(elapsedTime > 0 && !ignoreCooldown){ return EntityERR.UNDER_COOLDOWN; }
            else if(!InRange(entity.transform.position, attackRadius)){ return EntityERR.NOT_IN_RANGE; }

            int calculatedDMG = Mathf.FloorToInt(damage * (1f / GetMaxOccupants()));
            entity.Damage(calculatedDMG, damageInfliction, this);
            return EntityERR.SUCCESS;
        }

        public float GetCooldown() => Mathf.Clamp01(elapsedTime / cooldown);

        public override void Run(){
            base.Run();
            if(isDead && activeProjectiles.Count <= 0 && !IsDestroyed()){ 
                DestroyEntity();
                return; 
            }

            for(int i = 0; i < activeProjectiles.Count; i++){
                if(isDead){ activeProjectiles[i].doEaseIn = false; }

                activeProjectiles[i].EaseIn();
                if(!activeProjectiles[i].Run(projectileSpeed)){ 
                    continue; 
                }

                if(!activeProjectiles[i].obj){
                    activeProjectiles.RemoveAt(i);
                    i--;
                }
            }

            // Will only drop cooldown if structure is populated
            if(GetEfficiency() <= 0){ 
                elapsedTime = cooldown;
                return; 
            }


            elapsedTime -= Time.fixedDeltaTime;
            List<EntityHealth> enemies = FindEnemies();
            if(enemies.Count <= 0 || elapsedTime > 0){ return; }
            elapsedTime = cooldown;

            // Attacking
            attackAnimationSettings.ResetTime();
            int occupants = GetOccupants();
            for(int i = 0; i < occupants; i++){
                EntityHealth e = enemies[i % enemies.Count];

                float rndX = UnityEngine.Random.Range(-_collider.bounds.extents.x, _collider.bounds.extents.x);
                float rndZ = UnityEngine.Random.Range(-_collider.bounds.extents.z, _collider.bounds.extents.z);
                Vector3 startPos = _collider.bounds.center + new Vector3(rndX, _collider.bounds.extents.y, rndZ);

                AnimationInterpolation newSettings = new AnimationInterpolation(easeInCurve, easeInDuration);
                activeProjectiles.Add(new Projectile(projectile, startPos, e.transform, projectileHeightOffset, newSettings, () => Attack(e, true)));
            }
        }

        protected override void Start(){
            base.Start();
            activeProjectiles = new();
            attackAnimationSettings.ResetTime();
        }

        protected override void LateUpdate(){
            base.LateUpdate();
            float value = attackAnimationSettings.Play();
            meshRenderer.transform.localScale += (Vector3.one * .05f) * value;
        }

        protected override bool OnValidate() {
            if(!base.OnValidate()){ return false; }

            Vector3 startPos = _collider.bounds.center + new Vector3(0, _collider.bounds.extents.y);
            Vector3 endPos = _collider.bounds.center - new Vector3(0, _collider.bounds.extents.y) + transform.forward * attackRadius;
            

            return true;
        }

        protected override bool OnDrawGizmos(){
            #if UNITY_EDITOR
            if(!base.OnDrawGizmos()){ return false; }

            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, transform.up, attackRadius);

            // Vector3 prvPosition = curve.Evaluate(0);
            // for(int i = 1; i < 20; i++){
            //     Vector3 position = curve.Evaluate(i / 20f);
            //     Handles.DrawLine(prvPosition, position, 0.5f);
            //     prvPosition = position;
            // }
            #endif
            return true;
        }
    
        [Serializable] public class Projectile{
            AnimationInterpolation spawnSettings;
            public Transform obj{
                get;
                private set;
            }

            Vector3 cachedSize;
            Vector3 startPos, previousTargetPosition;
            Transform target;
            float originYOffset = 0;
            Func<EntityERR> OnComplete;

            float elapsed = 0;
            public bool doEaseIn = true;

            public Projectile(GameObject projectile, Vector3 startPos, Transform target, float originYOffset, AnimationInterpolation spawnSettings, Func<EntityERR> OnComplete = null){
                this.obj = Instantiate(projectile, startPos, Quaternion.identity, GameObject.Find("_GAME_RESOURCES_").transform).transform;
                this.startPos = startPos;
                this.target = target;
                this.originYOffset = originYOffset;
                this.spawnSettings = spawnSettings;
                this.OnComplete = OnComplete;

                previousTargetPosition = target.position;
                this.cachedSize = obj.localScale;
                this.doEaseIn = true;
            }


            /// <summary>
            /// Runs this projectile
            /// </summary>
            /// <param name="t">The speed of the projectile</param>            
            /// <returns>true if sequence is completed</returns>
            public bool Run(float t){
                if(!obj || !doEaseIn){ return !doEaseIn; }
                elapsed += Time.fixedDeltaTime * t;

                if(target){ previousTargetPosition = target.position; }
                
                Vector3 newPos = QuadraticCurveProjectiles.Evaluate(startPos, previousTargetPosition, elapsed, originYOffset);
                if(newPos == obj.position && elapsed < 1){ return false; }
                else if(newPos != obj.position){ obj.forward = (newPos - obj.position).normalized; }
                obj.position = newPos;

                if(elapsed >= 1 && doEaseIn){ 
                    OnComplete?.Invoke();
                    doEaseIn = false;
                }
                return !doEaseIn;
            }


            public void EaseIn(){
                if(!obj){ return; }

                float value = spawnSettings.Play(!doEaseIn);
                obj.localScale = Vector3.LerpUnclamped(Vector3.zero, cachedSize, value);
                if(obj.localScale.sqrMagnitude <= 0){ Destroy(obj.gameObject); }
            }
        }
    }
}