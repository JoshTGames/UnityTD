using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    /// <summary>
    /// The base entity class which provides basic functionality; allowing for 'OnHover' & 'OnSelect' interactivity
    /// </summary>
    public abstract class BaseEntity : MonoBehaviour, ISelectable{
        [SerializeField, Tooltip("Identifies who owns this entity. -1 means it is not owned. By default '0' is the player")] int _ownerId = -1;
        [SerializeField, Tooltip("When selected, this will get instantiated")] protected SelectionCircle selectionObject;
        protected SelectionCircle instantiatedSelection{ // This will hold the actual selectionObject
            get;
            private set;
        } 

        /// <summary>
        /// Identifies who owns this entity. -1 means it is not owned. By default '0' is the player
        /// </summary>
        public int owner{ get => _ownerId; }

        /// <summary>
        /// The behaviour that should occur when mouse is hovering over this entity
        /// </summary>
        /// <param name="hovered">Are we hovering over this entity?</param>
        public virtual void OnHover(bool hovered) => transform.localScale = (hovered)? Vector3.one * 1.1f : Vector3.one;

        /// <summary>
        /// The behaviour that should occur when this entity is selected
        /// </summary>
        /// <param name="selected">Is this entity selected?</param>
        public void OnSelect(bool selected){
            switch(selected){
                case true:
                    if(!instantiatedSelection){ 
                        instantiatedSelection = Instantiate(selectionObject, transform.position, selectionObject.transform.rotation, GameObject.Find("_GAME_RESOURCES_")?.transform);
                    }                    
                    instantiatedSelection.entity = this;
                    break;
                case false:
                    if(!instantiatedSelection){ break; }
                    instantiatedSelection.entity = null;
                    break;
            }
        }
        
        /// <summary>
        /// Adds this entity into the entities data set
        /// </summary>
        protected virtual void Start() => PlayerControls.instance.entities.SubscribeToEntities(this);
        /// <summary>
        /// Removes this entity from the entities data set
        /// </summary>
        protected virtual void OnDestroy(){ 
            PlayerControls.instance.entities.SubscribeToEntities(this, true);
            if(instantiatedSelection){ Destroy(instantiatedSelection.gameObject); }
        }
    }
}