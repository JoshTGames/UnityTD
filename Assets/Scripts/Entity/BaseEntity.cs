using UnityEngine;
using UnityEditor;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    /// <summary>
    /// The base entity class which provides basic functionality; allowing for 'OnHover' & 'OnSelect' interactivity
    /// </summary>
    public abstract class BaseEntity : MonoBehaviour{
        [SerializeField, Tooltip("When true, will show gizmos")] bool showDebug;
        [SerializeField] string entityName;
        [SerializeField, TextArea] protected string entityDescription;
        [SerializeField, Tooltip("Identifies who owns this entity. -1 means it is not owned. By default '0' is the player")] int _ownerId = -1;
        // [SerializeField, Tooltip("When selected, this will get instantiated")] protected SelectionCircle selectionObject;
        [SerializeField, Tooltip("The distance between entities to interact")] float _interactDistance;
        // protected SelectionCircle instantiatedSelection{ // This will hold the actual selectionObject
        //     get;
        //     private set;
        // } 

        public float interactDistance{ get => _interactDistance; }

        /// <summary>
        /// Identifies who owns this entity. -1 means it is not owned. By default '0' is the player
        /// </summary>
        public int owner{ get => _ownerId; }

        Vector3 cachedSize; // The default size of this object
        bool isDestroyed;

        /// <summary>
        /// 'Interacts' with this entity
        /// </summary>
        /// <param name="entity">The entity this function is being called from</param>
        /// <returns>An Entity 'ERR' enum code/returns>
        public abstract ERR Interact(BaseEntity entity);

        /// <summary>
        /// The behaviour that should occur when mouse is hovering over this entity
        /// </summary>
        /// <param name="hovered">Are we hovering over this entity?</param>
        public virtual void OnHover(bool hovered){
            transform.localScale = (hovered)? cachedSize * 1.1f : cachedSize;
            EntityTooltip.instance.tooltip = (hovered)? new EntityTooltip.Tooltip(entityName, entityDescription): null;
        }

        /// <summary>
        /// The behaviour that should occur when this entity is selected
        /// </summary>
        /// <param name="selected">Is this entity selected?</param>
        // public void OnSelect(bool selected){
        //     switch(selected){
        //         case true:
        //             if(!instantiatedSelection){ 
        //                 instantiatedSelection = Instantiate(selectionObject, transform.position, selectionObject.transform.rotation, GameObject.Find("_GAME_RESOURCES_")?.transform);
        //             }                    
        //             instantiatedSelection.entity = this;
        //             break;
        //         case false:
        //             if(!instantiatedSelection){ break; }
        //             instantiatedSelection.entity = null;
        //             break;
        //     }
        // }
        
        protected virtual void Awake(){}
        protected virtual void Update(){}
        protected virtual void FixedUpdate(){}

        /// <summary>
        /// Adds this entity into the entities data set
        /// </summary>
        protected virtual void Start(){
            cachedSize = transform.localScale;
            PlayerControls.instance.entities.SubscribeToEntities(null);
        }
        /// <summary>
        /// Removes this entity from the entities data set
        /// </summary>
        protected virtual void OnDestroy(){ 
            PlayerControls.instance.entities.SubscribeToEntities(null, true);
            isDestroyed = true;
            // if(instantiatedSelection){ Destroy(instantiatedSelection.gameObject); }
        }
    
        public bool IsDestroyed() => isDestroyed;

        protected virtual bool OnDrawGizmos() {
            if(!showDebug){ return false; }
            #if UNITY_EDITOR
                Handles.DrawWireDisc(transform.position, Vector3.up, interactDistance);
            #endif
            return true;
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
            UNDER_COOLDOWN = -7, // Entity is cannot perform action
            NO_FRIENDLY_FIRE = -8, // Attempting to attack friendly
            NO_PERMISSION = -9, // No permission for this action  
        }
    }
}