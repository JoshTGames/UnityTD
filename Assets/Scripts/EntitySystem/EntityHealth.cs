using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using AstralCandle.Animation;
using System;
using AstralCandle.TowerDefence;

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
        [SerializeField] AnimationInterpolation hitAnimationSettings;
        [SerializeField] DamagePopup damagePopupObject;
        [ColorUsage(true, hdr:true)]
        [SerializeField] Color hitColour = Color.white, healColour = Color.green;      
        [SerializeField] DropSettings[] dropSettings; 
        [SerializeField] float healNoiseStep = -1;
        public GameLoop.AudioSettings idleNoise, selectedNoise, actionNoise, hurtNoise, healNoise;
        


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
                if(isDead){ return; }
                
                int newHp = Mathf.Clamp(value, 0, maxHealth);
                if(immortal){
                    OnImmortalHit();
                    return;
                }
                else if(newHp <= 0){ OnDeath(); }
                else if(newHp < _health){ OnDamage(_health - newHp); }
                else if(newHp > _health){ OnHeal(newHp - _health); }
                _health = newHp;
            }
        }

        protected bool isDead;


        protected EntityHealth(int ownerId) : base(ownerId){}

        //--- Health functions
        public float GetHealth() => Mathf.Clamp01((float)Health / maxHealth);
        public float GetMaxHealth() => maxHealth;

        public virtual void Heal(int value) => Health += Mathf.Abs(value); // Ensures the value is positive

        public virtual void Damage(int value, IHealth.InflictionType infliction = IHealth.InflictionType.BALLISTIC, Entity attacker = null){
            value = -Mathf.Abs(value); // Ensures the value is negative
            bool hasResistance = resistances.Contains(infliction);

            // If holds resistance then inflict 2/3 of the damage
            Health += (hasResistance)? Mathf.FloorToInt(value / 3) * 2 : value;

            if(!hasResistance){ ApplyInfliction(infliction); }
        }


        //--- Base functions
        public override void Run(GameLoop.WinLose state){
            if(state != GameLoop.WinLose.In_Game){ return; }
            if(isHovered){
                EntityTooltip tooltipInstance = EntityTooltip.instance;
                if(!tooltipInstance.contents.ContainsKey("hp")){ return; }
                EntityTooltip.Contents content = tooltipInstance.contents["hp"];

                float hp = GetHealth();
                string txt = $"{Mathf.CeilToInt(hp * 100)}%";

                content.SetPercent(hp);
                content.SetText(txt);
            }
        }

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
            hitAnimationSettings.ResetTime(true);            
        }

        protected override void LateUpdate(){
            base.LateUpdate();
            float value = hitAnimationSettings.Play();
            meshRenderer.material.SetFloat("_Opacity", value);
            meshRenderer.transform.localScale -= (Vector3.one * 0.25f) * value;

            idleNoise.PlaySound(source);
            idleNoise.ActualCooldown -= Time.deltaTime;

            selectedNoise.ActualCooldown -= Time.deltaTime;
            actionNoise.ActualCooldown -= Time.deltaTime;
            hurtNoise.ActualCooldown -= Time.deltaTime;
        }

        public override void OnIsHovered(bool isHovered){
            base.OnIsHovered(isHovered);
            if(isHovered){
                float hp = GetHealth();
                EntityTooltip tooltipInstance = EntityTooltip.instance;
                tooltipInstance.tooltip.AddContents(
                    tooltipInstance.ContentsUIPrefab,
                    tooltipInstance.TooltipObject, 
                    ref tooltipInstance.contents, 
                    new EntityTooltip.Contents("hp", tooltipInstance.healthData.colour, tooltipInstance.healthData.icon, $"{Mathf.CeilToInt(hp * 100)}%")
                );
                tooltipInstance.contents["hp"].SetPercent(hp);
            }
        }

        //--- Abstract functions
        protected abstract void OnImmortalHit();
        protected virtual void OnDeath(){
            if(isDead){ return; }
            isDead = true;

            DestroyEntity();

            // Drops
            for(int i = 0; i < dropSettings.Length; i++){
                DropSettings drop = dropSettings[i];
                float dice = UnityEngine.Random.Range(0f, 1f);
                bool doSpawnDrop = dice <= drop.chanceToDrop;
                if(!doSpawnDrop){ continue; }

                float rndX = UnityEngine.Random.Range(-_collider.bounds.extents.x, _collider.bounds.extents.x);
                float rndZ = UnityEngine.Random.Range(-_collider.bounds.extents.z, _collider.bounds.extents.z);
                Vector3 startPos = _collider.bounds.center + new Vector3(rndX, -_collider.bounds.extents.y, rndZ);

                EntityResource r = Instantiate(drop.resource, startPos, Quaternion.identity, GameObject.Find("_GAME_RESOURCES_").transform);
                r.transform.position += new Vector3(0, r.GetComponent<Collider>().bounds.extents.y * 1.25f);

                int quantityToDrop = UnityEngine.Random.Range(drop.quantity.min, drop.quantity.max + 1);
                r.quantity = quantityToDrop;
            }
        }
        protected virtual void OnDamage(int value){ 
            meshRenderer.material.SetColor("_FlashColour", hitColour);
            hitAnimationSettings.ResetTime();

            float rndX = UnityEngine.Random.Range(-_collider.bounds.extents.x, _collider.bounds.extents.x);
            float rndZ = UnityEngine.Random.Range(-_collider.bounds.extents.z, _collider.bounds.extents.z);
            Vector3 startPos = _collider.bounds.center + new Vector3(rndX, _collider.bounds.extents.y, rndZ);

            DamagePopup dmgPopup = Instantiate(damagePopupObject, startPos, Quaternion.identity, GameObject.Find("_GAME_RESOURCES_").transform);
            dmgPopup.SetText($"-{value}");
            dmgPopup.SetColour((OwnerId != PlayerControls.instance.ownerId)? Color.white : hitColour);

            hurtNoise.PlaySound(source);
        }
        protected virtual void OnHeal(int value){ 
            meshRenderer.material.SetColor("_FlashColour", healColour);
            hitAnimationSettings.ResetTime();

            float rndX = UnityEngine.Random.Range(-_collider.bounds.extents.x, _collider.bounds.extents.x);
            float rndZ = UnityEngine.Random.Range(-_collider.bounds.extents.z, _collider.bounds.extents.z);
            Vector3 startPos = _collider.bounds.center + new Vector3(rndX, _collider.bounds.extents.y, rndZ);

            DamagePopup dmgPopup = Instantiate(damagePopupObject, startPos, Quaternion.identity, GameObject.Find("_GAME_RESOURCES_").transform);
            dmgPopup.SetText($"+{value}");
            dmgPopup.SetColour(healColour);
            healNoise.PlaySoundWithIncrease(source, GetHealth());
        }


        
        [Serializable] public class DropSettings{
            public EntityResource resource;
            [Range(0, 1)] public float chanceToDrop;
            public Utilities.MinMax quantity = new(0, 0);
        }
    }
}