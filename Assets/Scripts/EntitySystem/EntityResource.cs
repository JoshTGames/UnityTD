using AstralCandle.Animation;
using AstralCandle.TowerDefence;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// An entity which can be picked up by other entities
    /// </summary>
    public abstract class EntityResource : Entity{
        public ResourceData resourceProfile;
        public int quantity;
        [SerializeField, Tooltip("Controls the glow values of this item")] float glowFrequency, glowAmplitude;
        [SerializeField] AnimationInterpolation movementInterpolation;
        [SerializeField] GameLoop.AudioSettings pickupNoise;

        Vector3 previousPosition;
        Entity owner; 
        Entity Owner{ // When applied, this resource will move towards the entity
            get => owner;
            set{
                if(owner != null || value as IPickup == null){ return; }
                previousPosition = transform.position;
                owner = value;
                movementInterpolation.ResetTime();
            }
        }
        protected EntityResource(int ownerId) : base(ownerId){}

        public void Pickup(Entity entity){
            Human h = entity as Human;
            if(!isEnabled || !h || (h.heldResource != null && h.heldResource.resource != resourceProfile)){ return; }

            Owner = entity;
        }

        public override void Run(GameLoop.WinLose state){
            if(state != GameLoop.WinLose.In_Game){ return; }
            if(!isHovered){ return; }
            EntityTooltip.instance.contents["resource"].SetText($"{quantity}");
        }

        public override void OnIsHovered(bool isHovered){
            base.OnIsHovered(isHovered);
            if(isHovered){
                EntityTooltip tooltipInstance = EntityTooltip.instance;
                tooltipInstance.tooltip.AddContents(
                    tooltipInstance.ContentsUIPrefab,
                    tooltipInstance.TooltipObject, 
                    ref tooltipInstance.contents, 
                    new EntityTooltip.Contents("resource", tooltipInstance.resourceColour, resourceProfile.icon, $"{quantity}")
                );
                tooltipInstance.contents["resource"].SetPercent(1);
            }
        }

        protected override void Start(){
            base.Start();
            previousPosition = transform.position;
        }

        protected override void LateUpdate(){
            base.LateUpdate();
            float value = Utilities.Remap(Mathf.Sin(Time.time * glowFrequency) * glowAmplitude, -1, 1, 0, 1);
            meshRenderer.material.SetFloat("_Opacity", value);

            if(Owner){
                Human h = Owner as Human;
                if(!h || (h.heldResource != null && h.heldResource.resource != resourceProfile)){ 
                    owner = null; 
                    return;
                }

                float moveValue = movementInterpolation.Play();
                transform.position = Vector3.LerpUnclamped(previousPosition, owner.transform.position, moveValue);

                if(movementInterpolation.percent == 1){
                    (Owner as IPickup).Pickup(this);
                    pickupNoise.PlaySound(source);
                    DestroyEntity();
                }
            }
            else{
                transform.position = new(transform.position.x, previousPosition.y, transform.position.z);
            }
        }
    }
}