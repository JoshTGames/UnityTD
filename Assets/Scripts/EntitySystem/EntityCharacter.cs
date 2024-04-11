using System;
using UnityEngine;
using UnityEditor;
using AstralCandle.TowerDefence.AI;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    /// <summary>
    /// A health entity which can move and perform tasks
    /// </summary>
    public abstract class EntityCharacter : EntityHealth, ITask{
        [Header("Character Settings")]
        [SerializeField, Tooltip("The speed this entity will move at")] protected float moveSpeed = 5f;
        [SerializeField, Tooltip("The distance between entities till the entity can interact")] float interactRadius = 2f;        
        [SerializeField] AISettings aISettings;
        ContextSteering steering;
        Animator animator;
        
        

        /// <summary>
        /// If a task is present, the entity will try and perform the task
        /// </summary>
        Task entityTask;
        protected Task EntityTask{
            get => entityTask;
            private set{
                if(value == entityTask){ return; }
                entityTask = value;
            }
        }
        
        public static readonly int IDLE = Animator.StringToHash("Idle");
        public static readonly int WALK = Animator.StringToHash("Walk");

        int _animState;
        protected int AnimState{
            get => _animState;
            set{
                if(value == _animState){ return; }
                animator.CrossFade(value, 0.1f, 0);
                _animState = value;
            }
        }
        
        public EntityCharacter(int ownerId) : base(ownerId){}

        /// <summary>
        /// Checks to see if we need to remove the task
        /// </summary>
        /// <param name="success">The error code dictating if the action was successful</param>
        /// <returns>true/false</returns>
        bool CheckAndRemoveTask(EntityERR success){
            float dist = Vector3.Distance(EntityTask.position, transform.position);
            if(dist <= aISettings.deltaDistanceToTarget){ return true; }
            else if(success != EntityERR.NOT_IN_RANGE && success != EntityERR.UNDER_COOLDOWN){ return true; } // If task was successful or not actionable, then remove it
            return false;
        }

        public void SetTask(Vector3 position, Func<EntityERR> action = null){
            position.y = transform.position.y;
            EntityTask = new Task(position, action);
        }
        public float GetInteractRadius() => interactRadius;
        
        public override void Run(GameLoop.WinLose state){
            base.Run(state);
            if(state != GameLoop.WinLose.In_Game){ return; }
            
            if(EntityTask != null){                
                EntityERR success = EntityTask.RunTask(transform.position, aISettings.deltaDistanceToTarget, steering);
                
                if(CheckAndRemoveTask(success)){ 
                    EntityTask = null;
                    return;
                }               

                // Move character
                transform.position += EntityTask.moveDirection * (moveSpeed * Time.fixedDeltaTime);
            }            
        }

        //--- Base functions
        protected override void Start(){
            base.Start();
            steering = new ContextSteering(aISettings.steeringCircleResolution, aISettings.steeringCircleRadius, aISettings.steeringObstacles, aISettings.extraInterest, aISettings.dangerMultiplier);
            animator = GetComponentInChildren<Animator>();
        }

        protected override void OnDestroy(){
            base.OnDestroy();

            // If this entity is a wave-based character then attempt to remove it from the loop on destroy
            (this as IWave)?.RemoveEntityFromWave(this.GetInstanceID());
        }

        protected override bool OnDrawGizmos(){
            #if UNITY_EDITOR
            if(!base.OnDrawGizmos()){ return false; }

            // Steering behaviour settings
            Handles.color = Color.green;
            Vector3 detectionRingPosition = transform.position + Vector3.up * aISettings.steeringCircleYOffset;

            Handles.DrawWireDisc(detectionRingPosition, transform.up, aISettings.steeringCircleRadius, 2f);
            for(int i = 0; i < aISettings.steeringCircleResolution; i++){
                float angle = Utilities.CalculateRadianAngle(i, aISettings.steeringCircleResolution);
                Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                Handles.DrawDottedLine(detectionRingPosition, detectionRingPosition + direction * aISettings.steeringCircleRadius, 5f);
            }
            Handles.color = Color.cyan;
            if(EntityTask != null){
                Handles.DrawLine(detectionRingPosition, detectionRingPosition + EntityTask.moveDirection * aISettings.steeringCircleRadius, 2f);
            }

            // Interact radius
            Handles.color = Color.white;
            Handles.DrawWireDisc(_collider.bounds.center + new Vector3(0, -_collider.bounds.extents.y), Vector3.up, interactRadius, 2f);
            #endif
            return true;
        }

        /// <summary>
        /// Used to make a character move towards a target and attempt to perform an action associated to it
        /// </summary>
        public class Task{
            public Vector3 position{
                get;
                private set;
            }
            Func<EntityERR> action;

            public Vector3 moveDirection{
                get;
                private set;
            }


            public Task(Vector3 position, Func<EntityERR> action = null){
                this.position = position;
                this.action = action;
            }

            public void UpdatePosition(Vector3 position) => this.position = position;
            
            /// <summary>
            /// Attempts to perform the task
            /// </summary>
            /// <param name="entityPosition">The concurrent position of this entity</param>
            /// <param name="deltaDistance">The distance between the target position and entity position till we are within range</param>
            /// <param name="steering">Our move direction solver script</param>
            /// <returns>An EntityERR code displaying if it was successful or not</returns>
            public EntityERR RunTask(Vector3 entityPosition, float deltaDistance, ContextSteering steering){
                moveDirection = steering.Solve(entityPosition, (position - entityPosition).normalized);

                EntityERR success = (Vector3.Distance(position, entityPosition) <= deltaDistance)? EntityERR.SUCCESS : EntityERR.NOT_IN_RANGE;
                if(action != null){ success = action.Invoke(); }

                return success;
            }
        }
        [Serializable] public sealed class AISettings{
            [Tooltip("Any layers here will be attempted to be avoided when moving")] public LayerMask steeringObstacles;
            [Tooltip("The amount of rays that will be fired; querying the environment"), Range(4, 32)] public int steeringCircleResolution = 16;
            [Tooltip("The distance the rays are fired out")] public float steeringCircleRadius = 2f;
            [Tooltip("Moves the offset on the Y axis")] public float steeringCircleYOffset = 0f;
            [Tooltip("Adding interest, increases the amount of directions this entity can take")] public float extraInterest = 1;
            [Tooltip("Adding danger increases the responsiveness of an obstacle being avoided")] public float dangerMultiplier = 5;
            [Tooltip("The distance to the target position till we stop moving")] public float deltaDistanceToTarget = 0.01f;
        }
    }
}