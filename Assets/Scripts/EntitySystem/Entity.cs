using AstralCandle.Animation;
using AstralCandle.TowerDefence;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// Base class for all entities; Allows for selection and tooltip abilities for the player
    /// </summary>
    public abstract class Entity : MonoBehaviour, ISelectable{
        [SerializeField, Tooltip("The name of this entity")] string _name;
        [SerializeField, TextArea, Tooltip("Information about this entity")] string _description;
        [SerializeField] Spawning spawnSettings;
        [SerializeField, Tooltip("If true, will show all debugging gizmos associated to this entity")] bool showDebug;

        [SerializeField] int _ownerId = -1; // -1 means it is not owned

        /// <summary>
        /// The owner of this entity
        /// </summary>
        public int OwnerId{ get => _ownerId; }
        public Collider _collider{
            get;
            private set;
        }

        SelectionCircle selectionObject;
        bool isDestroyed = false, isHovered = false, easeIn = true;

        public Entity(int ownerId) => this._ownerId = ownerId;



        //--- FUNCTIONS
        public virtual string GetName() => _name;
        public virtual string GetDescription() => _description;


        public void OnIsHovered(bool isHovered){
            this.isHovered = isHovered;
            EntityTooltip.instance.tooltip = (isHovered)? new EntityTooltip.Tooltip(_name, _description): null;
        }

        public void OnIsSelected(SelectionCircle obj){
            if(isDestroyed){ return; }
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
        
        protected void DestroyEntity() => easeIn = false;

        /// <summary>
        /// Initialises this entity ready for running behaviour on
        /// </summary>
        protected virtual void Start(){
            PlayerControls.instance.entities.SubscribeToEntities(this);
            _collider = GetComponent<Collider>();
            easeIn = true;
            spawnSettings.cachedScale = transform.localScale;
        }

        protected virtual void LateUpdate(){
            Vector3 scale = spawnSettings.Scale(transform, easeIn);
            transform.localScale = scale * (isHovered? 1.1f : 1);
            if(transform.localScale.sqrMagnitude <= 0){ Destroy(gameObject); } // Deletes itself
        }

        protected virtual bool OnValidate(){
            if(Application.isPlaying){ return false; }
            _collider = GetComponent<Collider>();
            return true;
        }

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

        [System.Serializable] public sealed class Spawning{
            [SerializeField] AnimationInterpolation animationSettings;
            [HideInInspector] public Vector3 cachedScale;

            /// <summary>
            /// Adds a bit of flare to spawning the selection circle in
            /// </summary>
            /// <param name="obj">This transform</param>
            /// <param name="easeIn">Are we spawning in this object?</param>
            /// <returns>The interpolated scale</returns>
            public Vector3 Scale(Transform obj, bool easeIn = true){
                float value = animationSettings.Play(!easeIn);
                return Vector3.LerpUnclamped(Vector3.zero, cachedScale, value);
            }
        }
    }
}