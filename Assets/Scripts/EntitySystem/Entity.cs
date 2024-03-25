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
        [SerializeField, Tooltip("A visual way to show the player what this entity is")] Sprite[] entityAttributeIcons;
        [SerializeField] Spawning spawnSettings;
        [SerializeField, Tooltip("If true, will show all debugging gizmos associated to this entity")] bool showDebug;

        [SerializeField, Tooltip("The owner of this entity (-1 means it is not owned)")] int _ownerId = -1; // -1 means it is not owned
        [SerializeField, Tooltip("Used for animation")] protected Renderer meshRenderer;
        [SerializeField, Tooltip("Adding offset means the selection circle will show higher than normal")] float selectionCircleYOffset = 0;

        /// <summary>
        /// The owner of this entity
        /// </summary>
        public int OwnerId{ get => _ownerId; }
        public Material Material{ get => meshRenderer.material; }
        public Collider _collider{
            get;
            private set;
        }
        
        public bool isEnabled = true;
        SelectionCircle selectionObject;
        bool isDestroyed = false;
        protected bool isHovered = false;

        public bool MarkedForDestroy{
            get;
            private set;
        } = true;

        public Entity(int ownerId) => this._ownerId = ownerId;



        //--- FUNCTIONS
        public virtual string GetName() => _name;
        public virtual string GetDescription() => _description;


        public virtual void OnIsHovered(bool isHovered){
            this.isHovered = isHovered;
            EntityTooltip.instance.tooltip = (isHovered)? new EntityTooltip.Tooltip(_name, _description, entityAttributeIcons): null;
        }

        public void OnIsSelected(SelectionCircle obj){
            if(isDestroyed){ return; }
            if(obj){ 
                if(!selectionObject){
                    selectionObject = Instantiate(obj, transform.position, obj.transform.rotation, GameObject.Find("_GAME_RESOURCES_")?.transform); 
                    selectionObject.extraYOffset = selectionCircleYOffset;
                }
                selectionObject.entity = this;
            }
            else{
                if(!selectionObject){ return; }
                selectionObject.entity = null;
            }
        }

        public bool IsDestroyed() => isDestroyed;
        
        public void DestroyEntity() => MarkedForDestroy = true;

        /// <summary>
        /// Initialises this entity ready for running behaviour on
        /// </summary>
        protected virtual void Start(){
            PlayerControls.instance.entities.SubscribeToEntities(this);
            GameLoop.instance.allEntities.Add(this);
            _collider = GetComponent<Collider>();
            MarkedForDestroy = false;
            isEnabled = true;
            spawnSettings.cachedScale = meshRenderer.transform.localScale;
        }

        protected virtual void LateUpdate(){
            Vector3 scale = spawnSettings.Scale(meshRenderer.transform, !MarkedForDestroy);
            meshRenderer.transform.localScale = scale * (isHovered? 1.1f : 1);
            if(meshRenderer.transform.localScale.sqrMagnitude <= 0){ Destroy(gameObject); } // Deletes itself
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
            PlayerControls.instance.entities.selected.Remove(this);
            GameLoop.instance.allEntities.Remove(this);
            isDestroyed = true;            
        }

        /// <summary>
        /// Called by a manager script (The main function which triggers the behaviour of this entity)
        /// </summary>
        public abstract void Run();

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
            /// <param name="MarkedForDestroy">Are we spawning in this object?</param>
            /// <returns>The interpolated scale</returns>
            public Vector3 Scale(Transform obj, bool MarkedForDestroy = true){
                float value = animationSettings.Play(!MarkedForDestroy);
                return Vector3.LerpUnclamped(Vector3.zero, cachedScale, value);
            }
        }
    }
}