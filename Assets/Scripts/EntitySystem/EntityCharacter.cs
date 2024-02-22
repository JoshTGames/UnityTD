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

        /// <summary>
        /// If a task is present, the entity will try and perform the task
        /// </summary>
        protected Task entityTask{
            get;
            private set;
        }
        
        public EntityCharacter(int ownerId) : base(ownerId){}

        public void SetTask(Vector3 position, Func<EntityERR> action = null) => entityTask = new Task(position, action);
        public float GetInteractRadius() => interactRadius;
        
        protected override void Run(){
            if(entityTask != null){
                EntityERR success = entityTask.RunTask(transform.position, aISettings.deltaDistanceToTarget, steering);

                // Move character
                transform.position += entityTask.moveDirection * (moveSpeed * Time.fixedDeltaTime);

                // If task was successful or not actionable, then remove it
                if(success != EntityERR.NOT_IN_RANGE){ entityTask = null; } 
            }
        }

        //--- Base functions
        // DELETE ME
        private void FixedUpdate() => Run();

        protected override void Start(){
            base.Start();
            steering = new ContextSteering(aISettings.steeringCircleResolution, aISettings.steeringCircleRadius, aISettings.steeringObstacles, aISettings.extraInterest, aISettings.dangerMultiplier);
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
            if(entityTask != null){
                Handles.DrawLine(detectionRingPosition, detectionRingPosition + entityTask.moveDirection * aISettings.steeringCircleRadius, 2f);
            }

            // Interact radius
            Handles.color = Color.white;
            Handles.DrawWireDisc(transform.position, Vector3.up, interactRadius, 2f);
            return true;
            #endif
        }

        /// <summary>
        /// Used to make a character move towards a target and attempt to perform an action associated to it
        /// </summary>
        public class Task{
            Vector3 position;
            Func<EntityERR> action;

            public Vector3 moveDirection{
                get;
                private set;
            }


            public Task(Vector3 position, Func<EntityERR> action = null){
                this.position = position;
                this.action = action;
            }
            
            /// <summary>
            /// Attempts to perform the task
            /// </summary>
            /// <param name="entityPosition">The concurrent position of this entity</param>
            /// <param name="deltaDistance">The distance between the target position and entity position till we are within range</param>
            /// <param name="steering">Our move direction solver script</param>
            /// <returns>An EntityERR code displaying if it was successful or not</returns>
            public EntityERR RunTask(Vector3 entityPosition, float deltaDistance, ContextSteering steering){
                EntityERR success = (Vector3.Distance(position, entityPosition) <= deltaDistance)? EntityERR.SUCCESS : EntityERR.NOT_IN_RANGE;
                if(action != null){ success = action.Invoke(); }
                moveDirection = steering.Solve(entityPosition, (position - entityPosition).normalized);
                return success;
            }
        }
        [Serializable] public class AISettings{
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