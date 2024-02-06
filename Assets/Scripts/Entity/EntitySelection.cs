using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    public class EntitySelection<TEntityObj>{   
        /// <summary>
        /// All the selectable entities in the scene 
        /// </summary>
        public HashSet<TEntityObj> allEntities{
            get;
            private set;
        } = new HashSet<TEntityObj>(); 

        /// <summary>
        /// Houses all the concurrently selected entities
        /// </summary>
        public HashSet<TEntityObj> selected{
            get;
            private set;
        } = new HashSet<TEntityObj>(); 
        ISelectable _hovered;

        /// <summary>
        /// activates hover behaviour on entities
        /// </summary>
        public ISelectable hovered{
            get => _hovered;
            set{
                _hovered?.OnHover(false);
                _hovered = value;
                _hovered?.OnHover(true);
            }
        }
        Camera cam;

        public EntitySelection(Camera cam){
            this.cam = cam;
        }

        
        #region Selection functions
        /// <summary>
        /// Adds the clicked entity into the selected data set
        /// </summary>
        /// <param name="entity">The entity we want to add to data set</param>
        public void ClickSelect(TEntityObj entity){
            ISelectable selectable = (ISelectable)entity;
            
            DeselectAll();
            selected.Add(entity);
            selectable?.OnSelect(true);
        }

        /// <summary>
        /// Allows for multiple entities to be selected. In addition, removing already selected entities when re-selected
        /// </summary>
        /// <param name="entity">The entity we want to add or potentially remove from the data set</param>
        public void ShiftClickSelectEntity(TEntityObj entity){
            ISelectable selectable = (ISelectable)entity;

            switch(selected.Contains(entity)){
                case true:
                    selected.Remove(entity);
                    selectable?.OnSelect(false);
                    break;
                case false:
                    selected.Add(entity);
                    selectable?.OnSelect(true);
                    break;
            }
        }

        /// <summary>
        /// Allows for multiple entities to be selected
        /// </summary>
        /// <param name="entity">The entity we want to add to the data set</param>
        /// <param name="alternateMode">When true, will remove instead of add</param>
        public void DragSelect(TEntityObj entity, bool alternateMode = false){
            ISelectable selectable = (ISelectable)entity;

            if(!alternateMode && !selected.Contains(entity)){ 
                selected.Add(entity);                 
                selectable?.OnSelect(true);
            }
            else if(alternateMode && selected.Contains(entity)){ 
                selected.Remove(entity); 
                selectable?.OnSelect(false);
            }
        }

        /// <summary>
        /// Removes all selected items
        /// </summary>
        public void DeselectAll(){
            foreach(TEntityObj entity in selected){
                ISelectable selectable = (ISelectable)entity; 
                selectable?.OnSelect(false);                
            }
            selected.Clear();
        }

        /// <summary>
        /// Deselect a specific entity
        /// </summary>
        /// <param name="entity">The entity we would like to remove from the selected data set</param>
        public void Deselect(TEntityObj entity){
            ISelectable selectable = (ISelectable)entity; 

            if(!selected.Contains(entity)){ return; } // Safe guard
            selected.Remove(entity);
            selectable?.OnSelect(false);                
        }
        #endregion

        /// <summary>
        /// Adds/Removes entities from the data set of entities. Useful for when querying a drag selection
        /// </summary>
        /// <param name="entity">The entity we want to add/remove</param>
        /// <param name="unsubscribe">If true, will remove entity from the entities data set</param>
        public void SubscribeToEntities(TEntityObj entity, bool unsubscribe = false){
            bool containsEntity = allEntities.Contains(entity);
            switch(unsubscribe){
                case true:
                    if(!containsEntity){ return; }
                    allEntities.Remove(entity);
                    break;
                case false:
                    if(containsEntity){ return; }
                    allEntities.Add(entity);
                    break;
            }
        }

        /// <summary>
        /// Queries the screenPosition to find an entity
        /// </summary>
        /// <param name="screenPosition">The position on the screen we want to query</param>
        /// <param name="layers">The layers our raycast call will check</param>
        /// <returns>The entity we hit</returns>   
        public TEntityObj PositionOverEntity(Vector3 screenPosition, LayerMask layers){
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(screenPosition);
            // If nothing has been hit then return
            if(!Physics.Raycast(ray, out hit, cam.farClipPlane, layers)){ return default(TEntityObj); }

            TEntityObj collectedEntity = hit.transform.GetComponent<TEntityObj>();
            return collectedEntity;
        }

    }

    /// <summary>
    /// Used to show tool tips and descriptions of objects when mouse is hovering over objects
    /// </summary>
    public interface ISelectable{
        public void OnHover(bool hovered);
        public void OnSelect(bool selected);
    }
}