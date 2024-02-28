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
    Animator animator;

    Quaternion prvLookDirection;
    Vector3 lookDirection, lookVelocity;


    public Orc(int ownerId) : base(ownerId){}


    void ManageMovement() => animator.Play((entityTask != null)? "Walk" : "Idle");

    void ManageLean(){
        lookDirection = Vector3.SmoothDamp(lookDirection, (entityTask != null)? entityTask.moveDirection : Vector3.zero, ref lookVelocity, smoothing);
        Vector3 localMove = transform.InverseTransformDirection(lookDirection);
        Vector3 lean = new Vector3(localMove.z * leanFactor, 0, -localMove.x * leanFactor);

        Quaternion lookRot = (lookDirection != Vector3.zero)? Quaternion.LookRotation(lookDirection) : prvLookDirection;
        prvLookDirection = lookRot;
        transform.rotation = lookRot * Quaternion.Euler(lean);
    }

    protected void LateUpdate() {
        ManageMovement();        
        ManageLean();
    }

    protected override void Start(){
        base.Start();
        animator = GetComponentInChildren<Animator>();
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
    /// <returns>A list of all enemies within range of this entity</returns>
    protected List<EntityHealth> FindEnemies(){
        Collider[] entities = Physics.OverlapSphere(transform.position, GetInteractRadius(), entityLayer);
        List<EntityHealth> enemies = new List<EntityHealth>();
        foreach(Collider col in entities){
            EntityHealth hE = col.GetComponent<EntityHealth>();

            // Guard clause
            if(!hE || hE.OwnerId == OwnerId){ continue; }
            enemies.Add(hE);
        }
        return enemies;
    }


    public virtual EntityERR Attack(EntityHealth entity){
        if(!entity){ return EntityERR.INVALID_CALL; }
        if(OwnerId == entity.OwnerId){ return EntityERR.IS_FRIENDLY; }
        else if(elapsedTime > 0){ return EntityERR.UNDER_COOLDOWN; }
        else if(!InRange(entity._collider.ClosestPoint(transform.position))){ return EntityERR.NOT_IN_RANGE; }

        elapsedTime = cooldown;

        entity.Damage(damage, damageInfliction, this);
        return EntityERR.SUCCESS;
    }

    public float GetCooldown() => Mathf.Clamp01(elapsedTime / cooldown);


    protected override void OnDamage(){}
    protected override void OnDeath(){
        Destroy(gameObject);
    }
    protected override void OnHeal(){}
    protected override void OnImmortalHit(){}


    protected override void Run(){        
        elapsedTime -= Time.fixedDeltaTime;
        List<EntityHealth> enemies = FindEnemies();
        if(enemies.Count <= 0){ return; }
        if(enemies.Count >0 && entityTask == null){
            SetTask(enemies[0]._collider.ClosestPoint(transform.position), () => Attack(enemies[0]));
        }
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
