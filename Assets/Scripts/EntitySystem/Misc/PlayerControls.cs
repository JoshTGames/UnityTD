using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using AstralCandle.Entity;
using AstralCandle.TowerDefence;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class PlayerControls : MonoBehaviour{
    public static PlayerControls instance;
    [SerializeField, Tooltip("Not much point to change, but ensures owned entities can be compared to the player")] int _ownerId = 0;
    [SerializeField] float zoomSensitivity = 1;
    [SerializeField] float minOrthroSize = 5f;
    [SerializeField, Range(0.1f, 0.5f)] float minZoom;
    [SerializeField] float zoomSmoothing = 0.1f;
    [SerializeField] float pivotingSensitivity = 1;
    [SerializeField] SelectionCircle selectionGameObject;
    [SerializeField] DragSettings dragSettings;
    [SerializeField] LayerMask entityMask, mapMask;
    [SerializeField] Collider map;


    public EntitySelection<Entity> entities;
    public int ownerId{ get => _ownerId; }

    Camera cam;

    /// <summary>
    /// Used for extra behaviour
    /// </summary>
    bool shiftDown, altDown;
    public bool IsPivoting{
        get;
        private set;
    }

    float _targetZoom = 1, zoomVelocity;

    Vector2 mousePivotRotation;
    Quaternion pivotRotation;
    Vector3 pivotVelocity;

    Vector3 peakPosition, peakVelocity;

    float TargetZoom{
        get => _targetZoom;
        set{
            if(value == _targetZoom){ return; }
            _targetZoom = value;
            peakPosition = Vector3.Lerp(map.transform.position, GetPeakPosition(transform.parent.position), 1 - value);
        }
    }

    #region Drag selection variables
    Vector3? selectStartPosition;
    Rect selectionBox;
    #endregion

    Vector3 _cursorPosition, previousCursorPosition;
    public Vector3 cursorPosition{
        get => _cursorPosition;
        private set{
            previousCursorPosition = _cursorPosition;
            _cursorPosition = value;
            entities.hovered = (selectStartPosition == null && !IsPivoting)? entities.PositionOverEntity(value, entityMask): null;
            if(entities.hovered == null){ EntityTooltip.instance.tooltip = null; } // Should hopefully stop glitching where text stays active

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

    /// <summary>
    /// Calculates the orthrographic size for the camera so that the map is always within frame
    /// </summary>
    /// <returns>The size the camera should be if its to have the contents stay within frame</returns>
    float CalculateOrthrographicSize(){
        float mapScale = GameLoop.instance.MapScale;
        float height = mapScale;
        float width = mapScale * cam.pixelHeight / cam.pixelWidth;

        return Mathf.Max(width, height) * .5f;
    }

    /// <summary>
    /// Calculates the position to focus on when zooming in
    /// </summary>
    /// <param name="safePosition">The position to fall back on if a new position can be found</param>
    /// <returns>The position the player is wanting to zoom in on</returns>
    Vector3 GetPeakPosition(Vector3 safePosition){
        Ray ray = cam.ScreenPointToRay(cursorPosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, cam.farClipPlane, mapMask);
        if(!hit.collider){ return safePosition; }
        Vector3 dir = hit.point - map.transform.position;

        return map.transform.position + dir * .5f;
    }

    private void LateUpdate(){
        dragSettings.UpdateDragVisual((selectStartPosition != null)? (Vector3)selectStartPosition : Vector3.zero, cursorPosition, selectStartPosition != null, !altDown);

        float size = CalculateOrthrographicSize();
        float newSize = Utilities.Remap(TargetZoom, 0, 1, Mathf.Max(size * minZoom, minOrthroSize), size);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, newSize, ref zoomVelocity, zoomSmoothing);
        transform.parent.position = Vector3.SmoothDamp(transform.parent.position, peakPosition, ref peakVelocity, zoomSmoothing);
        transform.parent.rotation = Utilities.SmoothDampQuaternion(transform.parent.rotation, pivotRotation, ref pivotVelocity, zoomSmoothing);
    }

    private void Awake(){
        instance = this;
        cam = Camera.main;
        previousCursorPosition = _cursorPosition;
        entities = new EntitySelection<Entity>(Camera.main, selectionGameObject);
    }

    public void OnCursor(InputValue value) => cursorPosition = value.Get<Vector2>();
    public void OnZoom(InputValue value) => TargetZoom = Mathf.Clamp(TargetZoom + (value.Get<float>() * zoomSensitivity), 0, 1);
    public void OnShiftSelect(InputValue value) => shiftDown = value.Get<float>() > 0;
    public void OnAltSelect(InputValue value) => altDown = value.Get<float>() > 0;
    public void OnSelect(InputValue value){
        if(IsPivoting){ return; }   
        switch(value.Get<float>()){
            case 1: // Start                    
                selectStartPosition = cursorPosition;
                selectionBox = new Rect(); // Creates a new
                if(altDown){ break; }

                Entity newEntity = entities.hovered as Entity; 
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
                foreach(Entity entity in entities.allEntities){
                    // If  within the bounds of the rect
                    if(selectionBox.Contains(cam.WorldToScreenPoint(entity.transform.position))){ entities.DragSelect(entity, altDown); }
                }
                selectStartPosition = null;
                break;
        }
    }
    public void OnAction(InputValue value){
        if(IsPivoting){ return; }
        Camera cam = Camera.main;

        // Figure out task
        Entity hoveredEntity = entities.hovered as Entity; 
        if(hoveredEntity){ // Trying to interact with another entity...
            IHousing structure = hoveredEntity as IHousing;
            
            // If not structure or we dont own it...
            if(structure == null || hoveredEntity.OwnerId != ownerId){ return; }

            // Iterate through each of our selected characters and query the structure
            foreach(Entity e in entities.selected){
                ITask entity = e as ITask;
                if(entity == null || e.OwnerId != ownerId){ continue; } // If not a character or is not owned by the player...
                
                switch(structure.AddOccupant(e)){
                    case EntityERR.SUCCESS:
                        Destroy(e.gameObject);
                        break;
                    case EntityERR.NOT_IN_RANGE:
                        entity.SetTask(hoveredEntity.transform.position, () => structure.AddOccupant(e));
                        break;
                }
            }
        }                
        else{ // SetTask to cursor
            // Stops raycasts being calculated if nothing is selected
            if(entities.selected.Count <= 0){ return; }

            // Calculates the world position to SetTask to
            Ray ray = cam.ScreenPointToRay(cursorPosition);
            RaycastHit hit;
            if(!Physics.Raycast(ray, out hit, cam.farClipPlane, mapMask)){ return; }

            foreach(Entity e in entities.selected){
                ITask entity = e as ITask;                    
                if(!e || e.OwnerId != ownerId || entity == null){ continue; } // If not a character or is not owned by the player...

                // Set targetPosition in character(s)
                Vector3 pos = new Vector3(hit.point.x, e.transform.position.y, hit.point.z);
                entity.SetTask(pos);
            }
        }          
    }
    public void OnPivot(InputValue value) => IsPivoting = value.Get<float>() >0;
    public void OnCursorVelocity(InputValue value){
        Cursor.visible = !IsPivoting && EntityTooltip.instance.tooltip == null;
        Cursor.lockState = (IsPivoting)? CursorLockMode.Locked: CursorLockMode.None;

        if(!IsPivoting){ return; }
        Vector2 _value = value.Get<Vector2>();
        mousePivotRotation += new Vector2(_value.x, -_value.y) * pivotingSensitivity;        

        float pitch = mousePivotRotation.x * Mathf.Deg2Rad; // Left/Right
        // float yaw = mousePivotRotation.y * Mathf.Deg2Rad; // Up/Down
        Quaternion pitchRot = Quaternion.AngleAxis(pitch, Vector3.up);
        // Quaternion yawRot = Quaternion.AngleAxis(yaw, transform.right);
        pivotRotation = Quaternion.Normalize(pitchRot);
    }
    [System.Serializable] public class DragSettings{
        [SerializeField, Tooltip("The UI visual element")] Image dragUI;
        
        [SerializeField, Tooltip("The colours to set the UI based on state")] Color addSelection = Color.green, reSetTaskSelection = Color.red;

        /// <summary>
        /// Visualiser for when we drag across the screen
        /// </summary>
        public void UpdateDragVisual(Vector3 start, Vector3 end, bool show, bool isAdding = true){
            // Apply a colour based on state
            dragUI.color = (isAdding)? addSelection : reSetTaskSelection;

            // Sets position to the center between both points
            dragUI.rectTransform.position = (start + end) / 2;

            // Sets the size of the transform according to the displacement between both points
            dragUI.rectTransform.sizeDelta = new Vector2(Mathf.Abs(start.x - end.x), Mathf.Abs(start.y - end.y));

            // Enables/disables the element
            dragUI.gameObject.SetActive(show);
        }
    }
}
