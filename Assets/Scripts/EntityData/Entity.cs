using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    /// <summary>
    /// Used for adding affects onto entities, If entity has resistance to one of these and it is attempted on this entity. The damage is 2/3 of total damage & infliction is ignored
    /// </summary>
    public enum DamageTypes{
        BALLISTIC = 0, // Nothing
        FIRE = 3, // 5% of health every second
        ELECTRICAL = 2, // Stuns player for duration
        BLEEDING = 5 // 5% of health every second
    } 

    /// <summary>
    /// USEFUL FOR AI
    /// </summary>
    public enum ERR{
        SUCCESS = 0, // If successful function call
        NOT_IN_RANGE = -1, // If entity is not in range to perform task
        INVALID_CALL = -2, // If the function was incorrectly called
        IMMORTAL_ENTITY = -3, // If entity is immortal
        MAX_OCCUPANCY = -4, // If structure is full
        NO_OCCUPANTS = -5, // If structure has no occupants 
        UNABLE_TO_ATTACK = -6, // If the entity is unable to attack
        IS_FROZEN = -7
    }
    
    public abstract class Entity{
        /// <summary>
        /// The owner of this entity 
        /// </summary>
        public string owner{
            get;
            private set;
        }

        /// <summary>
        /// The physical object this class is associated to 
        /// </summary>
        public Transform obj{
            get;
            private set;
        }

        // Each object associated to a cell will have health
        int _health;
        int maxHealth;
        bool isImmortal;

        /// <summary>
        /// If true, will halt most actions 
        /// </summary>
        protected bool isFrozen{ 
            get;
            private set;
        }
        
        /// <summary>
        /// Any resistances appended here will stop the infliction and reduce the damage by 2/3 
        /// </summary>
        protected HashSet<DamageTypes> resistances{
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
        /// <param name="isImmortal">If true, this object cannot take damage</param>
        public Entity(string owner, Transform obj, int maxHealth, HashSet<DamageTypes> resistances, bool isImmortal = false){
            this.owner = owner;
            this.obj = obj;
            this.maxHealth = maxHealth;
            this._health = maxHealth;
            this.resistances = resistances;
            this.isImmortal = isImmortal;
        }

        
        // This calls other functions when modified
        int health{
            get => _health;
            set{
                int newHp = Mathf.Clamp(value, 0, maxHealth);
                if(isImmortal){
                    OnImmortalHit();
                    return;
                }
                else if(newHp <= 0){ OnDeath(); } // Death
                else if(newHp > _health){ OnHeal(); } // Heal
                else if(newHp < _health){ OnDamage(); } // Damaged

                _health = newHp;
            }
        }

        /// <summary>
        /// Damages the entity
        /// </summary>
        /// <param name="value">The amount you want to take from this entity</param>
        /// <param name="infliction">The damage type to place on this entity</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        public ERR Damage(int value, DamageTypes infliction = DamageTypes.BALLISTIC){
            if(value != -Mathf.Abs(value)){ // Stops healing on damage 
                Debug.LogWarning($"Trying to heal {obj.name} when calling the 'Damage' function!");
                return ERR.INVALID_CALL; 
            } 
            // Calculates the damage. If has resistance against a type of damage, The damage taken is 2/3.
            bool hasResistance = resistances.Contains(infliction);
            int damage = (hasResistance)? Mathf.FloorToInt((value/3)) * 2: value;
            health -= damage;

            // If there is no resistance against this infliction... 
            if(!hasResistance){ ApplyInfliction(infliction); }
            if(isImmortal){ return ERR.IMMORTAL_ENTITY; }
            return ERR.SUCCESS;
        }

        /// <summary>
        /// Heals the entity
        /// </summary>
        /// <param name="value">The amount you want to add to this entity</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        public ERR Heal(int value){
            if(value != Mathf.Abs(value)){ 
                Debug.LogWarning($"Trying to damage {obj.name} when calling the 'Heal' function!");
                return ERR.INVALID_CALL; 
            }
            health += value;
            return ERR.SUCCESS;
        }

        /// <summary>
        /// Applies an infliction on the target
        /// </summary>
        /// <param name="infliction">The infliction type we will be appending onto the entity</param>
        /// <returns></returns>
        async void ApplyInfliction(DamageTypes infliction){
            float elapsedTime = 0;

            /// <summary>
            /// Applies bleed on this entity over time
            /// </summary>
            /// <returns></returns>
            async void ApplyBleed(){
                while(elapsedTime < (int)infliction){
                    health -= Mathf.FloorToInt(0.05f * maxHealth);
                    elapsedTime += 1;
                    await Task.Delay(1000);
                }
            }
            string _infliction = infliction.ToString();
            if(_infliction == DamageTypes.FIRE.ToString() || _infliction == DamageTypes.BLEEDING.ToString()){ ApplyBleed(); }
            else if(_infliction == DamageTypes.ELECTRICAL.ToString()){ 
                isFrozen = true;
                await Task.Delay((int)infliction * 1000);
                isFrozen = false;
            }              
        }


        /// <summary>
        /// Called when health grows
        /// </summary>
        protected abstract void OnHeal();
        /// <summary>
        /// Called when health drops
        /// </summary>
        protected abstract void OnDamage();
        /// <summary>
        /// Called when health drops to 0
        /// </summary>
        protected abstract void OnDeath();

        /// <summary>
        /// Called when this object is hit but is immortal
        /// </summary>
        protected abstract void OnImmortalHit();        
    }
}