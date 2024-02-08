using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    /// <summary>
    /// Inheriting from 'BaseEntity' appends health & resistance functionality to the entity object.
    /// </summary>
    public abstract class HealthEntity : BaseEntity{
        [SerializeField, Tooltip("The max health associated to this entity")] int maxHealth = 100;
        [SerializeField, Tooltip("If true, this entity cannot be harmed")] bool immortal = false;
        [SerializeField, Tooltip("The resistances associated to this entity")] List<DamageType> _resistances;

        /// <summary>
        /// The resistances associated to this entity
        /// </summary>
        HashSet<DamageType> resistances;
        int _health; // SHOULD ONLY BE ACCESSIBLE FROM THE 'health' variable alternative

        /// <summary>
        /// If true, can stop various behaviours from occuring e.g. movement
        /// </summary>
        protected bool isFrozen{
            get;
            private set;
        }
        
        /// <summary>
        /// Event based health - When health is changed it will compare the new health value and then call functions according to the event
        /// </summary>
        protected int health{
            get => _health;
            set{
                int newHealth = Mathf.Clamp(value, 0, maxHealth);
                if(immortal){
                    OnImmortalHit();
                    return;
                } // Immortal
                else if(newHealth <= 0){ OnDeath(); } // Death
                else if(newHealth < _health){ OnDamage(); } // Damaged
                else if(newHealth > _health){ OnHeal(); } // Heal

                _health = newHealth;
            }
        }

        /// <summary>
        /// Called when this entity is hit but is immortal
        /// </summary>
        protected abstract void OnImmortalHit();
        /// <summary>
        /// Called when this entity dies
        /// </summary>
        protected abstract void OnDeath();
        /// <summary>
        /// Called when this entity is damaged
        /// </summary>
        protected abstract void OnDamage();
        /// <summary>
        /// Called when this entity is healed
        /// </summary>
        protected abstract void OnHeal();



        /// <summary>
        /// Converts the '_resistances' variable to a hashset & Sets health to maxHealth
        /// </summary>
        protected override void Start() {
            resistances = _resistances.ToHashSet();
            _health = maxHealth;
            base.Start();
        }


        /// <summary>
        /// Damages the entity
        /// </summary>
        /// <param name="value">The amount to damage this entity by</param>
        /// <param name="infliction">The damage type that will be attempted to place on this entity</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        public ERR Damage(int value, DamageType infliction = DamageType.BALLISTIC){
            if(value != -Mathf.Abs(value)){ // Guard clause to stop healing in the damage function
                Debug.LogWarning($"{this.GetType()}:: Value is attempting to heal entity inside the Damage function!");
                return ERR.INVALID_CALL;
            }

            bool hasResistance = resistances.Contains(infliction);
            int damage = (hasResistance)? Mathf.FloorToInt(value / 3) * 2: value; // If we have resistance to this damage type, then perform 2/3 of the desired damage
            health -= damage;

            if(!hasResistance){ ApplyInfliction(infliction);}

            if(immortal){ return ERR.IS_IMMORTAL; }
            return ERR.SUCCESS;
        }

        /// <summary>
        /// Heals the entity
        /// </summary>
        /// <param name="value">The amount to heal this entity by</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        public ERR Heal(int value){
            if(value != Mathf.Abs(value)){ // Guard clause to stop damage in the heal function
                Debug.LogWarning($"{this.GetType()}:: Value is attempting to inflict damage inside the Heal function!");
                return ERR.INVALID_CALL;
            }
            health += value;
            return ERR.SUCCESS;
        }

        /// <summary>
        /// Gets the health percentage of this entity 
        /// </summary>
        public float GetHealthPercentage() => Mathf.Clamp01((float)health / maxHealth);

        /// <summary>
        /// Applies an infliction on the entity
        /// </summary>
        /// <param name="infliction">The infliction type that will be applied onto the entity</param>
        async void ApplyInfliction(DamageType infliction){
            float elapsedTime = 0;

            async void ApplyBleedOverDuration(){
                while(elapsedTime < (int)infliction){
                    health -= Mathf.FloorToInt(.05f * maxHealth); // 5% of health
                    elapsedTime += 1;
                    await Task.Delay(1000);
                }
            }

            string _infliction = infliction.ToString();
            if(_infliction == DamageType.FIRE.ToString() || _infliction == DamageType.BLEEDING.ToString()){
                ApplyBleedOverDuration();
            }
            else if(_infliction == DamageType.ELECTRIC.ToString()){
                isFrozen = true;
                await Task.Delay((int)infliction * 1000);
                isFrozen = false;
            }
        }

        /// <summary>
        /// Stores the duration of each effect
        /// </summary>
        [Serializable] public enum DamageType{
            BALLISTIC = 0,
            FIRE = 3,
            ELECTRIC = 2,
            BLEEDING = 5
        }

        /// <summary>
        /// Codes useful for AI development
        /// </summary>
        public enum ERR{
            SUCCESS = 0, // Successful call
            INVALID_CALL = -1, // Invalid call (Perhaps wrong function)
            IS_IMMORTAL = -2, // Entity is immortal (Useful for AI)
            NOT_IN_RANGE = -3, // Not in range to target
            NO_OCCUPANTS = -4, // No occupants associated to entity
            MAX_OCCUPANTS = -5, // Entity has reached max occupancy
            IS_FROZEN = -6, // Entity is stunned
            UNABLE_TO_ATTACK = -7 // Entity is unable to attack (Perhaps cooldown)
        }
    }
}