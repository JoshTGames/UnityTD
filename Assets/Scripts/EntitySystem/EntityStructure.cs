using System;
using System.Collections.Generic;
using AstralCandle.TowerDefence;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// EntityHealth with the ability of housing other entities
    /// </summary>
    public abstract class EntityStructure : EntityHealth, IHousing{
        [Header("Structure Settings")]
        [SerializeField] int maxOccupants = 1;
        protected int occupants;
        [SerializeField] protected Entity spawnedEntity;
        [SerializeField] protected Repair repairSettings;
        public GameLoop.AudioSettings spawnSFX;

        protected EntityStructure(int ownerId) : base(ownerId){}

        public float GetEfficiency() => (float)occupants / maxOccupants;

        /// <summary>
        /// Returns the number of occupants in this structure
        /// </summary>
        public int GetOccupants() => occupants;
        /// <summary>
        /// Returns the max number of occupants for this structure
        /// </summary>
        public int GetMaxOccupants() => maxOccupants;


        public EntityERR AddOccupant(Entity entity){
            ITask task = entity as ITask;
            if(task == null || !entity || !_collider){ return EntityERR.INVALID_CALL; } // If the entity is not capable of performing tasks (This should not be called, but more of a guard clause)
            else if(entity.OwnerId != OwnerId){ return EntityERR.NOT_PERMITTED; } // Ensures we are only adding units of our own
            else if(occupants >= maxOccupants){ return EntityERR.MAX_OCCUPANTS; } // If full, we cannot add any more units
            else if(Vector3.Distance(_collider.ClosestPoint(entity.transform.position), entity.transform.position) > task.GetInteractRadius()){ return EntityERR.NOT_IN_RANGE; } // If we are not within range to interact with this entity

            occupants++;
            PlayerControls.instance.entities.Deselect(entity);
            entity.DestroyEntity();
            return EntityERR.SUCCESS;
        }

        public EntityERR RemoveOccupant(out Entity entity){
            entity = null;
            if(occupants <= 0){ return EntityERR.NO_OCCUPANTS; }

            entity = Instantiate(spawnedEntity, transform.position, spawnedEntity.transform.rotation, GameObject.Find("_GAME_RESOURCES_").transform);
            PlayerControls.instance.entities.ShiftClickSelectEntity(entity);
            occupants--;
            return EntityERR.SUCCESS;
        }

        public override void Run(GameLoop.WinLose state){
            base.Run(state);
            if(state != GameLoop.WinLose.In_Game){ return; }
            
            repairSettings.RepairEntity(this);
            if(isHovered){
                EntityTooltip tooltipInstance = EntityTooltip.instance;
                if(!tooltipInstance.contents.ContainsKey("occupants")){ return; }
                EntityTooltip.Contents content = tooltipInstance.contents["occupants"];                

                content.SetPercent((float)occupants / maxOccupants);
                content.SetText($"{occupants}/{maxOccupants}");
            }
        }

        public override void OnIsHovered(bool isHovered){
            base.OnIsHovered(isHovered);
            if(isHovered){
                EntityTooltip tooltipInstance = EntityTooltip.instance;
                tooltipInstance.tooltip.AddContents(
                    tooltipInstance.ContentsUIPrefab,
                    tooltipInstance.TooltipObject, 
                    ref tooltipInstance.contents, 
                    new EntityTooltip.Contents("occupants", tooltipInstance.occupantData.colour, tooltipInstance.occupantData.icon, $"{occupants}/{maxOccupants}")
                );
                tooltipInstance.contents["occupants"].SetPercent((float)occupants / maxOccupants);
            }
        }

        protected override void Start(){
            base.Start();
            if(OwnerId != PlayerControls.instance?.ownerId){ return; }
            GameLoop.instance?.playerStructures.Add(this);
        }

        protected override void OnDestroy(){
            GameLoop.instance?.playerStructures.Remove(this);
            base.OnDestroy();
        }

        protected override void OnDamage(int value){
            base.OnDamage(value);
            repairSettings.ResetTime();
        }

        [Serializable] public class Repair{
            [SerializeField, Range(0, 1)] float healAmount;
            [SerializeField] float timeTillHeal;

            float elapsedTime;
            public void ResetTime() => elapsedTime = timeTillHeal;

            public void RepairEntity(EntityStructure entity){
                elapsedTime -= Time.fixedDeltaTime;
                if(entity.GetHealth() >= 1 || entity.GetEfficiency() <= 0){ ResetTime(); }

                if(elapsedTime <= 0){
                    ResetTime();
                    entity.Heal(Mathf.FloorToInt(entity.GetMaxHealth() * healAmount * entity.GetEfficiency()));
                }
            }
        }
    }
}