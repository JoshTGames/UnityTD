using System.Collections.Generic;
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
        List<Entity> housesEntities;


        protected EntityStructure(int ownerId) : base(ownerId){}

        public float GetEfficiency() => (float)housesEntities.Count / maxOccupants;

        /// <summary>
        /// Returns the number of occupants in this structure
        /// </summary>
        protected int GetOccupants() => housesEntities.Count;
        /// <summary>
        /// Returns the max number of occupants for this structure
        /// </summary>
        protected int GetMaxOccupants() => maxOccupants;

        public EntityERR AddOccupant(Entity entity){
            ITask task = entity as ITask;
            if(task == null || entity == null){ return EntityERR.INVALID_CALL; } // If the entity is not capable of performing tasks (This should not be called, but more of a guard clause)
            else if(entity.OwnerId != OwnerId){ return EntityERR.NOT_PERMITTED; } // Ensures we are only adding units of our own
            else if(housesEntities.Count >= maxOccupants){ return EntityERR.MAX_OCCUPANTS; } // If full, we cannot add any more units
            else if(Vector3.Distance(_collider.ClosestPoint(entity.transform.position), entity.transform.position) > task.GetInteractRadius()){ return EntityERR.NOT_IN_RANGE; } // If we are not within range to interact with this entity

            housesEntities.Add(entity);
            Destroy(entity.gameObject);
            return EntityERR.SUCCESS;
        }

        public EntityERR RemoveOccupant(out Entity entity){
            entity = null;
            if(housesEntities.Count <= 0){ return EntityERR.NO_OCCUPANTS; }

            Entity spawnedEntity = Instantiate(housesEntities[0], transform.position, housesEntities[0].transform.rotation, GameObject.Find("_GAME_RESOURCES_").transform);
            housesEntities.RemoveAt(0);
            return EntityERR.SUCCESS;
        }

        protected override void Start(){
            base.Start();
            housesEntities = new List<Entity>();
            if(OwnerId != PlayerControls.instance?.ownerId){ return; }
            GameLoop.instance?.playerStructures.Add(this);
        }

        protected override void OnDestroy(){
            GameLoop.instance?.playerStructures.Remove(this);
            base.OnDestroy();
        }
    }
}