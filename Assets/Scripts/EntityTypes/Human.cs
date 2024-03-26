using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AstralCandle.Entity;
using System;
using AstralCandle.TowerDefence;

public class Human : EntityCharacter, IPickup, IHarvest{
    [SerializeField, Tooltip("Dictates how far this character leans when moving")] float leanFactor = 10f;
    [SerializeField] float smoothing = 0.1f;
    [SerializeField] LayerMask resourceLayer;
    [SerializeField] float harvestCooldown = 1;
    float elapsedTime;


    Vector3 targetHarvestPos;
    EntityResourceNode targetResource; 
    public EntityResourceNode TargetResource{
        get => targetResource;
        set{
            if(targetResource?.GetInstanceID() == value?.GetInstanceID()){ return; }
            targetResource = value; 
            if(value && !value.MarkedForDestroy){
                Vector3 entityPos = value._collider.ClosestPoint(transform.position);
                targetHarvestPos = entityPos + ((transform.position - entityPos).normalized * (GetInteractRadius()/2));
                targetHarvestPos.y = value.transform.position.y;
            }
        }
    } 

    // Inventory
    public ResourceData.Resource heldResource{
        get;
        private set;
    }

    Quaternion prvLookDirection;
    Vector3 lookDirection, lookVelocity;


    public Human(int ownerId) : base(ownerId){}

    #region HIDE
    protected override void OnImmortalHit(){}
    #endregion

    void MagnetiseResources(){
        Collider[] resources = Physics.OverlapSphere(_collider.bounds.center + new Vector3(0, -_collider.bounds.extents.y), GetInteractRadius(), resourceLayer);
        foreach(Collider col in resources){
            EntityResource r = col.transform.GetComponent<EntityResource>();
            r.Pickup(this);
        }
    }

    void ManageMovement() => AnimState = (entityTask != null && entityTask.moveDirection != Vector3.zero)? WALK : IDLE; 

    void ManageLean(){
        lookDirection = Vector3.SmoothDamp(lookDirection, (entityTask != null)? entityTask.moveDirection : Vector3.zero, ref lookVelocity, smoothing);

        Vector3 localMove = transform.InverseTransformDirection(lookDirection);
        Vector3 lean = new Vector3(localMove.z * leanFactor, 0, -localMove.x * leanFactor);

        Quaternion lookRot = (lookDirection != Vector3.zero)? Quaternion.LookRotation(lookDirection) : prvLookDirection;
        prvLookDirection = lookRot;
        transform.rotation = lookRot * Quaternion.Euler(lean);
    }

    public override void OnIsHovered(bool isHovered){
            base.OnIsHovered(isHovered);
            if(isHovered && heldResource != null){
                float hp = GetHealth();
                EntityTooltip tooltipInstance = EntityTooltip.instance;
                tooltipInstance.tooltip.AddContents(
                    tooltipInstance.ContentsUIPrefab,
                    tooltipInstance.TooltipObject, 
                    ref tooltipInstance.contents, 
                    new EntityTooltip.Contents("resource", tooltipInstance.resourceColour, heldResource.resource.icon, $"{heldResource.quantity}")
                );
                tooltipInstance.contents["resource"].SetPercent(1);
            }
        }

    public override void Run(){
        base.Run();
        MagnetiseResources();

        elapsedTime -= Time.fixedDeltaTime;
        if(entityTask == null && TargetResource && !TargetResource.MarkedForDestroy){
            SetTask(targetHarvestPos, () => Harvest(TargetResource));
        }

        EntityTooltip tooltipInstance = EntityTooltip.instance;
        if(isHovered && tooltipInstance.contents.ContainsKey("resource")){
            if(heldResource == null){
                tooltipInstance.tooltip.RemoveContent(ref tooltipInstance.contents, "resource");
                return;
            }
            tooltipInstance.contents["resource"].SetText($"{heldResource.quantity}");
        }
    }

    protected override void LateUpdate() {
        base.LateUpdate();
        ManageMovement();        
        ManageLean();
    }    


    public EntityERR Pickup(EntityResource resource){
        if(!resource || resource.MarkedForDestroy){ return EntityERR.INVALID_CALL; }

        // Cannot pickup nothing and ensures we are only holding one item at a time
        if(Vector3.Distance(resource.transform.position, transform.position) > GetInteractRadius()){ return EntityERR.NOT_IN_RANGE; }
        // if(!resource || (heldResource != null && resource != heldResource.resource.GetName())){ return EntityERR.INVALID_CALL; }

        if(heldResource != null && heldResource.resource == resource.resourceProfile){ heldResource.quantity += resource.quantity; } // If pre-existing         
        heldResource ??= new ResourceData.Resource(resource.resourceProfile, resource.quantity); // If resource doesnt exist...

        resource.DestroyEntity();
        return EntityERR.SUCCESS;
    }
    public ResourceData.Resource GetEquipped() => heldResource;

    public void RemoveEquipped(){
        heldResource = null;
    }

    /// <summary>
    /// Queries the entity position against this to see if its within interaction range
    /// </summary>
    /// <param name="other">The position of the entity we want to harvest</param>
    /// <returns>True/False depending on if the entity is within interaction range</returns>
    protected bool InRange(Vector3 other) => Vector3.Distance(_collider.ClosestPoint(other), other) <= GetInteractRadius();

    public EntityERR Harvest(EntityHealth entity, bool ignoreCooldown = false){
        Vector3 range = entity._collider.ClosestPoint(transform.position);

        if(!entity || entity.MarkedForDestroy){ return EntityERR.INVALID_CALL; }
        else if(!InRange(range)){ return EntityERR.NOT_IN_RANGE; }
        else if(elapsedTime > 0 && !ignoreCooldown){ return EntityERR.UNDER_COOLDOWN; }

        elapsedTime = harvestCooldown;

        entity.Damage(1, IHealth.InflictionType.BALLISTIC, this);
        return EntityERR.SUCCESS;
    }

    public float GetCooldown() => Mathf.Clamp01(elapsedTime / harvestCooldown);
}
