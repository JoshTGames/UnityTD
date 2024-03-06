using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AstralCandle.Entity;

public class Orc : EntityCharacter, IAttack, IWave{
    [Header("Attack Settings")]
    [SerializeField] LayerMask entityLayer;
    [SerializeField] IHealth.InflictionType damageInfliction;
    [SerializeField] int damage = 5;
    [SerializeField] float attackRadius = 2;
    [SerializeField] float cooldown = 1;
    float elapsedTime;

    [SerializeField, Tooltip("Dictates how far this character leans when moving")] float leanFactor = 10f;
    [SerializeField] float smoothing = 0.1f;
    Quaternion prvLookDirection;
    Vector3 lookDirection, lookVelocity;

    EntityHealth _target;
    EntityHealth Target{
        get => _target;
        set{
            if(value == null){ return; }
            Vector3 entityPos = value._collider.ClosestPoint(transform.position);
            Vector3 targetPos = entityPos + ((transform.position - entityPos).normalized * (attackRadius/2));
            targetPos.y = value.transform.position.y;

            if(value == _target && entityTask != null){ 
                entityTask.UpdatePosition(targetPos);
                return; 
            }
            _target = value;
            SetTask(targetPos, () => Attack(value));
        }
    }


    public Orc(int ownerId) : base(ownerId){}


    void ManageMovement() => AnimState = (entityTask != null)? WALK : IDLE; 

    void ManageLean(){
        lookDirection = Vector3.SmoothDamp(lookDirection, (entityTask != null)? entityTask.moveDirection : Vector3.zero, ref lookVelocity, smoothing);
        Vector3 localMove = transform.InverseTransformDirection(lookDirection);
        Vector3 lean = new Vector3(localMove.z * leanFactor, 0, -localMove.x * leanFactor);

        Quaternion lookRot = (lookDirection != Vector3.zero)? Quaternion.LookRotation(lookDirection) : prvLookDirection;
        prvLookDirection = lookRot;
        transform.rotation = lookRot * Quaternion.Euler(lean);
    }

    protected override void LateUpdate() {
        base.LateUpdate();
        ManageMovement();        
        ManageLean();
    }


    /// <summary>
    /// Queries the entity position against this to see if its within attacking range
    /// </summary>
    /// <param name="other">The position of the entity we want to attack</param>
    /// <returns>True/False depending on if the entity is within attacking range</returns>
    protected bool InRange(Vector3 other) => Vector3.Distance(transform.position, other) <= attackRadius;

    /// <summary>
    /// Searches for enemies within attack range of this entity
    /// </summary>
    /// <returns>A target to attack</returns>
    protected EntityHealth FindEnemy(){
        EntityHealth entity = null;
        #region HUMAN SEARCH
        Collider[] entities = Physics.OverlapSphere(transform.position, GetInteractRadius(), entityLayer);
        foreach(Collider col in entities){
            EntityCharacter eC = col.GetComponent<EntityCharacter>();
            if(!eC || eC.OwnerId == OwnerId){ continue; }
            else if(entity == null){ 
                entity = eC; 
                continue;
            }

            float aDist = Vector3.Distance(transform.position, eC.transform.position);
            float bDIst = Vector3.Distance(transform.position, entity.transform.position);

            if(aDist < bDIst){ entity = eC; }           
        }
        #endregion
        if(entity){ return entity; }

        #region STRUCTURE SEARCH     
        List<EntityStructure> plrStructures = GameLoop.instance.playerStructures;
        if(plrStructures.Count <= 0){ return null; }
        
        entity = plrStructures[0];
        for(int i = 0; i < plrStructures.Count; i++){
            EntityStructure pS = plrStructures[i];

            float aDist = Vector3.Distance(transform.position, pS.transform.position);
            float bDIst = Vector3.Distance(transform.position, entity.transform.position);
            if(aDist < bDIst){ entity = pS; }
        }
        #endregion
        return entity;
    }


    public virtual EntityERR Attack(EntityHealth entity, bool ignoreCooldown = false){
        Vector3 range = entity._collider.ClosestPoint(transform.position);

        if(!entity){ return EntityERR.INVALID_CALL; }
        else if(OwnerId == entity.OwnerId){ return EntityERR.IS_FRIENDLY; }
        else if(!InRange(range)){ return EntityERR.NOT_IN_RANGE; }
        else if(elapsedTime > 0 && !ignoreCooldown){ return EntityERR.UNDER_COOLDOWN; }

        elapsedTime = cooldown;

        entity.Damage(damage, damageInfliction, this);
        return EntityERR.SUCCESS;
    }

    public float GetCooldown() => Mathf.Clamp01(elapsedTime / cooldown);


    protected override void OnDamage(){}
    protected override void OnHeal(){}
    protected override void OnImmortalHit(){}


    protected override void Run(){        
        elapsedTime -= Time.fixedDeltaTime;
        Target = FindEnemy();
        if(!Target){ return; }

        base.Run();
    }

    protected override bool OnDrawGizmos(){
        #if UNITY_EDITOR
        if(!base.OnDrawGizmos()){ return false; }

        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.up, attackRadius);
        #endif
        return true;
    }
}
