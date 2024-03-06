using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// An entity which also has health functionality
    /// </summary>
    public abstract class EntityHealth : Entity, IHealth{
        [Header("Health Settings")]
        [SerializeField, Tooltip("If true, will stop this entity from being inflicted by damage")] bool immortal;
        [SerializeField, Tooltip("The max health this entity can have")] int maxHealth;
        [SerializeField, Tooltip("Any resistances here will reduce incoming damage and stop the infliction being added")] List<IHealth.InflictionType> _resistances;

        /// <summary>
        /// The resistances this entity has. (This is the '_resistances' variable, converted into a hashset)
        /// </summary>
        HashSet<IHealth.InflictionType> resistances;

        int _health;

        /// <summary>
        /// Triggers events when changed
        /// </summary>
        int Health{
            get => _health;
            set{
                int newHp = Mathf.Clamp(value, 0, maxHealth);
                if(immortal){
                    OnImmortalHit();
                    return;
                }
                else if(newHp <= 0){ OnDeath(); }
                else if(newHp < _health){ OnDamage(); }
                else if(newHp > _health){ OnHeal(); }
                _health = newHp;
            }
        }


        protected EntityHealth(int ownerId) : base(ownerId){}

        //--- Health functions
        public float GetHealth() => Mathf.Clamp01((float)Health / maxHealth);

        public virtual void Heal(int value) => Health += Mathf.Abs(value); // Ensures the value is positive

        public virtual void Damage(int value, IHealth.InflictionType infliction = IHealth.InflictionType.BALLISTIC, Entity attacker = null){
            value = -Mathf.Abs(value); // Ensures the value is negative
            bool hasResistance = resistances.Contains(infliction);

            // If holds resistance then inflict 2/3 of the damage
            Health += (hasResistance)? Mathf.FloorToInt(value / 3) * 2 : value;

            if(!hasResistance){ ApplyInfliction(infliction); }
        }


        //--- Base functions

        /// <summary>
        /// Damages the entity over time
        /// </summary>
        /// <param name="infliction">The infliction we want to apply onto the entity</param>
        protected void ApplyInfliction(IHealth.InflictionType infliction){
            float elapsedTime = 0;
            async void ApplyOverDuration(){
                while(elapsedTime < (int)infliction){
                    elapsedTime++;
                    Health -= Mathf.FloorToInt(.025f * maxHealth); // 5% of health
                    await Task.Delay(1000);
                }
            }
            if(infliction == IHealth.InflictionType.BALLISTIC){ return; }
            ApplyOverDuration();
        }

        protected override void Start(){
            base.Start();
            _health = maxHealth;
            resistances = _resistances.ToHashSet();            
        }


        //--- Abstract functions
        protected abstract void OnImmortalHit();
        protected virtual void OnDeath() => DestroyEntity();
        protected abstract void OnDamage();
        protected abstract void OnHeal();
    }
}