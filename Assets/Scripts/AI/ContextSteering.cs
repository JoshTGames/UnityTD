using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence.AI{
    /// <summary>
    /// By using this class to steer entities to a desired position will create more organic movement as entities will be able to steer away from obstacles
    /// </summary>
    public class ContextSteering{
        LayerMask layerMask;
        float distance;
        Vector3[] directions;

        float extraInterest, dangerMultiplier;

        /// <summary>
        /// Constructs the steering class
        /// </summary>
        /// <param name="points">The number of directions the entity can move in</param>
        /// <param name="distance">The detection distance of obstacles</param>
        /// <param name="layerMask">The layers we will detect obstacles on</param>
        /// <param name="extraInterest">Playing with this value will change the directions the entity can move in</param>
        /// <param name="dangerMultiplier">Makes danger less appetising to move towards the greater it is</param>
        public ContextSteering(int points, float distance, LayerMask layerMask, float extraInterest = 1.25f, float dangerMultiplier = 5f){
            this.layerMask = layerMask;
            this.distance = distance;
            this.extraInterest = extraInterest;
            this.dangerMultiplier = dangerMultiplier;

            // Populate directions using a circle generation function
            this.directions = new Vector3[points];
            for(int i = 0; i < points; i++){
                float angle = Utilities.CalculateRadianAngle(i, points);
                this.directions[i] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            }
        }   

        /// <summary>
        /// Attempts to steer away from collisions
        /// </summary>
        /// <param name="origin">The start position</param>
        /// <param name="normalDirection">The direction we want to move to</param>
        /// <returns>A new direction; steering away from collisions</returns>
        public Vector3 Solve(Vector3 origin, Vector3 normalDirection){
            Vector3 finalDir = Vector3.zero;

            for(int i = 0; i < directions.Length; i++){
                float interest = Vector3.Dot(normalDirection, directions[i]) + extraInterest; // We add 'extraInterest' so that the entity can steer in more directions
                
                // Calculate danger
                RaycastHit hit;
                Physics.Raycast(origin, directions[i], out hit, distance, layerMask);

                float danger = 0;
                if(hit.collider){
                    danger =  dangerMultiplier * (1 - (hit.distance / distance)); // Increases danger the closer to the target we are
                }

                float newInterest = Mathf.Clamp01(interest - danger);
                finalDir += directions[i] * newInterest;
            }

            // This averages out all the directions
            return finalDir.normalized;
        }
    }
}