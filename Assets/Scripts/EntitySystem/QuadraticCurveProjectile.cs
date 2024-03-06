using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    /// <summary>
    /// Allows us to form quadratic curves for projectiles
    /// </summary>
    [System.Serializable] public static class QuadraticCurveProjectiles{
        /// <summary>
        /// Calculates a point on a curve given a time value
        /// </summary>
        /// <param name="origin">Start position</param>
        /// <param name="target">End position</param>
        /// <param name="t">A value between 0-1</param>
        /// <param name="originYOffset">Added onto the origin and used to calculate the center (control) point</param>
        /// <returns>A position along the curve based on time (0-1)</returns>
        public static Vector3 Evaluate(Vector3 origin, Vector3 target, float t, float originYOffset = 0){
            Vector3 ctrlPoint = origin + (target - origin) / 2;
            ctrlPoint.y = origin.y + originYOffset;

            Vector3 AToC = Vector3.LerpUnclamped(origin, ctrlPoint, t);
            Vector3 CToB = Vector3.LerpUnclamped(ctrlPoint, target, t);
            return Vector3.LerpUnclamped(AToC, CToB, t);        
        }
    }
}