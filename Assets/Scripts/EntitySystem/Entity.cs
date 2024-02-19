using System.Collections;
using System.Collections.Generic;
using AstralCandle.TowerDefence;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Entity : MonoBehaviour, ISelectable{
        [SerializeField, Tooltip("The name of this entity")] string _name;
        [SerializeField, TextArea, Tooltip("Information about this entity")] string _description;
        [SerializeField, Tooltip("If true, will show all debugging gizmos associated to this entity")] bool showDebug;

        int _ownerId = -1; // -1 means it is not owned

        /// <summary>
        /// The owner of this entity
        /// </summary>
        public int OwnerId{ get => _ownerId; }
        Vector3? cachedSize;
        SelectionCircle selectionObject;
        bool isDestroyed = false;

        public Entity(int ownerId) => this._ownerId = ownerId;



        //--- FUNCTIONS
        public virtual string GetName() => _name;
        public virtual string GetDescription() => _description;


        public void OnIsHovered(bool isHovered){
            cachedSize = (isHovered && cachedSize == null)? transform.localScale : cachedSize;
            transform.localScale = (isHovered && cachedSize != null)? (Vector3)cachedSize * 1.1f : (Vector3)cachedSize;

            EntityTooltip.instance.tooltip = (isHovered)? new EntityTooltip.Tooltip(_name, _description): null;
            if(!isHovered){ cachedSize = null; }
        }

        public void OnIsSelected(SelectionCircle obj){
            if(obj){ 
                if(!selectionObject){
                    selectionObject = Instantiate(obj, transform.position, obj.transform.rotation, GameObject.Find("_GAME_RESOURCES_")?.transform); 
                }
                selectionObject.entity = this;
            }
            else{
                if(!selectionObject){ return; }
                selectionObject.entity = null;
            }
        }

        public bool IsDestroyed() => isDestroyed;


        /// <summary>
        /// Initialises this entity ready for running behaviour on
        /// </summary>
        protected virtual void Start() => PlayerControls.instance.entities.SubscribeToEntities(this);

        /// <summary>
        /// Behaviour which is triggered upon destroying this entity
        /// </summary>
        protected virtual void OnDestroy(){
            PlayerControls.instance.entities.SubscribeToEntities(this, true);
            isDestroyed = true;
        }

        /// <summary>
        /// Called by a manager script (The main function which triggers the behaviour of this entity)
        /// </summary>
        protected abstract void Run();

        /// <summary>
        /// Used to display editor gizmos
        /// </summary>
        /// <returns>True/False for if this function is allowed to run</returns>
        protected virtual bool OnDrawGizmos() => showDebug;
    }
}