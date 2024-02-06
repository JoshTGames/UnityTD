using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AstralCandle.TowerDefence{
    public class PlayerControls : MonoBehaviour{
        [SerializeField] LayerMask entityMask;
        [SerializeField, Tooltip("Not much point to change, but ensures owned entities can be compared to the player")] int ownerId = 0;
        public static PlayerControls instance;
        public EntitySelection<BaseEntity> entities;

        /// <summary>
        /// Used for extra behaviour
        /// </summary>
        bool shiftDown, altDown;

        #region Drag selection variables
        Vector3? selectStartPosition;
        Rect selectionBox;
        #endregion

        Vector3 _cursorPosition;
        Vector3 cursorPosition{
            get => _cursorPosition;
            set{
                _cursorPosition = value;
                entities.hovered = (selectStartPosition == null)? entities.PositionOverEntity(value, entityMask): null;

                // Dragging behaviour
                if(selectStartPosition != null){
                    Vector3 startPos = (Vector3)selectStartPosition;
                    selectionBox.xMin = Mathf.Min(value.x, startPos.x);
                    selectionBox.xMax = Mathf.Max(value.x, startPos.x);

                    selectionBox.yMin = Mathf.Min(value.y, startPos.y);
                    selectionBox.yMax = Mathf.Max(value.y, startPos.y);                    
                }
            }
        }


        private void Awake(){
            instance = this;
            entities = new EntitySelection<BaseEntity>(Camera.main);
        }

        public void OnCursor(InputValue value) => cursorPosition = value.Get<Vector2>();
        // public void OnZoom(InputValue value) => targetZoom += value.Get<float>() * zoomSensitivity;
        public void OnShiftSelect(InputValue value) => shiftDown = value.Get<float>() > 0;
        public void OnAltSelect(InputValue value) => altDown = value.Get<float>() > 0;
        public void OnSelect(InputValue value){        
            switch(value.Get<float>()){
                case 1: // Start
                    selectStartPosition = cursorPosition;
                    selectionBox = new Rect(); // Creates a new
                    BaseEntity newEntity = entities.PositionOverEntity(cursorPosition, entityMask);
                    if(altDown){ break; }
                    
                    switch(shiftDown){
                        case true:
                            entities.ShiftClickSelectEntity(newEntity);
                            break;
                        case false:
                            entities.ClickSelect(newEntity);
                            break;
                    }
                    break;
                case 0: // End
                    Camera cam = Camera.main;
                    // Iterate through all entities; quering each against the selection box positions
                    foreach(BaseEntity entity in entities.allEntities){
                        // If  within the bounds of the rect
                        if(selectionBox.Contains(cam.WorldToScreenPoint(entity.transform.position))){ entities.DragSelect(entity, altDown); }
                    }
                    selectStartPosition = null;
                    break;
            }
        }
        public void OnAction(InputValue value){
            // Figure out task

            foreach(CharacterEntity entity in entities.selected){
                if(entity == null || entity.owner != ownerId){ continue; } // If not a character or is not owned by the player...
            }
            // Query if task is possible
            // Add task to job list
        }
    }
}