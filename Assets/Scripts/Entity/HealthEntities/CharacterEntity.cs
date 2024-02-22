using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AstralCandle.TowerDefence.AI;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    /// <summary>
    /// Inheriting from 'HealthEntity' appends
    /// </summary>
    public abstract class CharacterEntity : HealthEntity{
        [SerializeField] AI _aISettings;
        [SerializeField, Tooltip("The max velocity this entity can reach")] protected float maxMoveSpeed = 10;
        [SerializeField, Tooltip("Dictates how far this character leans when moving")] float leanFactor = 10f;
        [SerializeField, Tooltip("The damage this entity can do")] protected int damage = 5;
        [SerializeField, Tooltip("The duration inbetween attacks")] protected double attackCooldown = 2;
        [SerializeField, Tooltip("The max distance from where we can attack a target")] float attackDistance = 0.5f;
        [SerializeField, Tooltip("The attack type we will inflict upon targets")] protected DamageType attackType;


        public AI aISettings{ get => _aISettings; } // Getter
        BT_Node behaviour;
        Animator animator;

        readonly int IDLE = Animator.StringToHash("Idle");
        readonly int WALK = Animator.StringToHash("Walk");

        /// <summary>
        /// Used to calculate how to move towards the targetPosition
        /// </summary>
        protected BT_MoveTo moveTo{
            get;
            private set;
        }

        /// <summary>
        /// The entity we want to interact with | Similar to targetPosition
        /// </summary>
        [HideInInspector] public BaseEntity targetEntity;

        /// <summary>
        /// The position we want the entity to be at | NOT USED TO MOVE CHARACTER, instead used in calculations to get there
        /// </summary>
        [HideInInspector] public Vector3 targetPosition;
        /// <summary>
        /// The direction we want to move the entity | Used to move character
        /// </summary>
        [HideInInspector] public Vector3 moveDirection;
        Quaternion prvLookDirection;
        Vector3 lookDirection, lookVelocity;
        [SerializeField] float smoothing = 0.1f;

        

        /// <summary>
        /// The cooldown until the entity can attack again
        /// </summary>
        double cooldown;
        
        public void DestroyEntity() => Destroy(gameObject);        

        /// <summary>
        /// The item this entity is holding. | Can only hold a single item. However, item quantity is variable
        /// </summary>
        // protected ItemEntity heldItem;

        /// <summary>
        /// Attempts to attack a parsed entity
        /// </summary>
        /// <param name="entity">The entity to attack</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        protected virtual ERR Attack(HealthEntity entity){
            ERR success = CanAttack(entity);
            if(success != ERR.SUCCESS){ return success; }

            cooldown = Time.timeAsDouble + attackCooldown;
            return success;
        }

        /// <summary>
        /// Queries the entity to see if we can attack them
        /// </summary>
        /// <param name="entity">The entity, we are querying if we can attack</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        protected ERR CanAttack(HealthEntity entity){
            if(!entity){ return ERR.INVALID_CALL; }
            else if((entity.transform.position - transform.position).magnitude > attackDistance){ return ERR.NOT_IN_RANGE; }
            else if(Time.timeAsDouble < cooldown){ return ERR.UNDER_COOLDOWN; }
            else if(entity.owner == owner){ return ERR.NO_FRIENDLY_FIRE; }
            
            return ERR.SUCCESS;
        }
        
        /// <summary>
        /// Attempts to harvest a parsed entity
        /// </summary>
        /// <param name="entity">The entity we wish to harvest from</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        // protected abstract ERR Harvest(ResourceEntity entity);

        /// <summary>
        /// Attempts to pickup a parsed entity
        /// </summary>
        /// <param name="entity">The entity we wish to pickup</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        // protected abstract ERR Pickup(ItemEntity entity);

        /// <summary>
        /// Attempts to move this entity to a desired position || CALLED FROM FIXED UPDATE
        /// </summary>
        /// <param name="direction">The direction the entity wants to move to</param>
        /// <returns>An Entity 'ERR' enum code</returns>
        protected abstract ERR MoveTo(Vector3 direction);

        /// <summary>
        /// Used to create the behaviour tree this entity will use to make decisions on its behaviour
        /// </summary>
        /// <returns>The root behaviour node which this entity will run</returns>
        protected abstract void CreateTree(out BT_Node behaviour);
        

        /// <summary>
        /// Runs the behaviour node associated to this entity
        /// </summary>
        protected virtual void Run(){
            switch(behaviour?.Evaluate()){
                case BT_Node.NodeState.Running:
                    break;
                case BT_Node.NodeState.Success:
                    break;
                case BT_Node.NodeState.Fail:
                    break;
            }
        }
    
        void ManageMovement(){
            MoveTo(moveDirection);
            int moveState = (moveDirection != Vector3.zero) ? WALK : IDLE;
            animator.CrossFade(moveState, 0.1f, 0);
        }
        void ManageLean(){
            lookDirection = Vector3.SmoothDamp(lookDirection, moveDirection, ref lookVelocity, smoothing);

            Vector3 localMove = transform.InverseTransformDirection(lookDirection);
            Vector3 lean = new Vector3(localMove.z * leanFactor, 0, -localMove.x * leanFactor);

            Quaternion lookRot = (lookDirection != Vector3.zero)? Quaternion.LookRotation(lookDirection) : prvLookDirection;
            prvLookDirection = lookRot;
            transform.rotation = lookRot * Quaternion.Euler(lean);
        }

        protected override void Update() => Run(); // Runs the decision logic
        protected override void FixedUpdate() {
            base.FixedUpdate();
            ManageMovement();
            ManageLean();
        }
        
        protected override void Start() {
            base.Start();
            targetPosition = transform.position;
            animator = GetComponentInChildren<Animator>();
            ContextSteering steering = new ContextSteering(_aISettings.detectionResolution, _aISettings.detectionRadius, _aISettings.obstacles, _aISettings.extraInterest, _aISettings.dangerMultiplier);
            moveTo = new BT_MoveTo(this, steering, _aISettings.deltaDistanceToTarget);
            CreateTree(out behaviour);
        }


        protected override bool OnDrawGizmos(){
            if(base.OnDrawGizmos() == false){ return false; }
            #if UNITY_EDITOR
            // AI
            Vector3 detectionRing = transform.position + (Vector3.up * _aISettings.offset);
            Handles.color = Color.green;
            Handles.DrawWireDisc(detectionRing, Vector3.up, _aISettings.detectionRadius);
            for(int i = 0; i < _aISettings.detectionResolution; i++){
                float angle = Utilities.CalculateRadianAngle(i, _aISettings.detectionResolution);
                Handles.DrawLine(detectionRing, detectionRing + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * _aISettings.detectionRadius);
            }

            // Entity
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.up, attackDistance);
            if(moveDirection != null){
                Handles.DrawLine(detectionRing, detectionRing + moveDirection * aISettings.detectionRadius);
            }
            #endif
            return true;
        }

        [System.Serializable] public class AI{
            [Header("Steering Settings")]
            [Tooltip("The amount of directions around the entity we check")] public int detectionResolution = 16;
            [Tooltip("The distance away from the entity we check")] public float detectionRadius = 5;
            [Tooltip("The objects this entity will try avoid")] public LayerMask obstacles;
            [Tooltip("")] public float offset;
            [Tooltip("Change this to utilise more directions of the entity. If 0, it is unlikely this entity will be able to reverse")] public float extraInterest = 1.25f;
            [Tooltip("Use this to increase the danger reading of obstacles")] public float dangerMultiplier = 5;
            [Tooltip("The distance away from target until the entity stops moving")] public float deltaDistanceToTarget = Mathf.Epsilon;
        }
    }
}