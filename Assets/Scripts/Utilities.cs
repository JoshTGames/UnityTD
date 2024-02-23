using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

public static class Utilities{
    /// <summary>
    /// Remaps an input value from the fromMin to fromMax range and converts it and maps it to a new set of values between toMin and toMax
    /// </summary>
    /// <param name="inputValue">The value to be modified</param>
    /// <param name="fromMin">The minimum possible raw value</param>
    /// <param name="fromMax">The maximum possible raw value</param>
    /// <param name="toMin">The minimum new value</param>
    /// <param name="toMax">The maximum new value</param>
    /// <returns>The new 'inputValue'</returns>
    public static float Remap(float inputValue, float fromMin, float fromMax, float toMin, float toMax){
        float i = (((inputValue - fromMin) / (fromMax - fromMin)) * (toMax - toMin) + toMin);
        i = Mathf.Clamp(i, toMin, toMax);
        return i;
    }    

    /// <summary>
    /// Same as Vector3.SmoothDamp(), but works for Quaternions
    /// </summary>
    /// <param name="current">The current rotation</param>
    /// <param name="target">The rotation to reach</param>
    /// <param name="currentVelocity">A reference to a velocity variable</param>
    /// <param name="smoothTime">Changes how responsive </param>
    /// <returns>A interpolated value</returns>
    public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed = Mathf.Infinity, float deltaTime = -1){
        deltaTime = (deltaTime != -1)? deltaTime : Time.deltaTime;

        Vector3 c = current.eulerAngles;
        Vector3 t = target.eulerAngles;
        return Quaternion.Euler(
            Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime, maxSpeed, deltaTime),
            Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime, maxSpeed, deltaTime),
            Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime, maxSpeed, deltaTime)
        );
    }

    /// <summary>
    /// Scans a given point radially and returns if a raycast has hit an object
    /// </summary>
    /// <param name="origin">Base position</param>
    /// <param name="rotation">Base rotation</param>
    /// <param name="radius">Circle radius</param>
    /// <param name="startIndex">The point we wish to start the circle at</param>
    /// <param name="iterations">The number of points the circle has</param>
    /// <param name="axis">The axis we wish to draw the circle</param>
    /// <param name="layer">The layer(s) we want our raycasting functing to check for</param>
    /// <param name="hit">The hit object should it exist</param>
    /// <param name="showRay">If true, will show the drawn raycasts</param>
    /// <returns>True/False depending on if hit is true</returns>
    public static bool ArcCast(Vector3 origin, Quaternion rotation, float radius, int startIndex, int iterations, Vector3 axis, LayerMask layer, out RaycastHit hit, bool showRay = false){
        Matrix4x4 matrix = Matrix4x4.TRS(origin, rotation, Vector3.one);
        for(int i = 0; i < iterations; i++){
            int currIndex = i + startIndex;
            int nextIndex = (currIndex + 1) % iterations;

            Vector3 currentPoint = CalculateCircularVectorAngle(currIndex, iterations, axis);
            Vector3 nextPoint = CalculateCircularVectorAngle(nextIndex, iterations, axis); // Calculates the next direction 
            float distance = ((nextPoint - currentPoint) * radius).magnitude;

            Vector3 start = matrix.MultiplyPoint3x4(currentPoint * radius);
            Vector3 direction = nextPoint - currentPoint;            
            Vector3 localDirection = matrix.MultiplyVector(direction.normalized);            
            

            if(Physics.Raycast(start, localDirection, out hit, distance, layer)){ return true; }
            if(showRay){ Debug.DrawRay(start, matrix.MultiplyVector(direction * radius), Color.red); }            
        }
        hit = new RaycastHit();
        return false;
    }


    /// <summary>
    /// This function clamps the magnitude of a vector between a min and max value
    /// </summary>
    /// <param name="v">The vector value to clamp</param>
    /// <param name="min">The min length of the vector</param>
    /// <param name="max">The max length of the vector</param>
    /// <returns>The clamped vector positioned inbetween the min and max value</returns>
    public static Vector3 ClampMagnitude(Vector3 v, float min, float max){
        float sm = v.sqrMagnitude;        
        if(sm > max * max){ return v.normalized * max; }
        else if(sm < min * min){ return v.normalized * min; }
        
        return v;
    }
    
    /// <summary>
    /// Clamps an angle between 0,360
    /// </summary>
    /// <param name="angle">Current angle</param>
    /// <param name="min">Min angle</param>
    /// <param name="max">Max angle</param>
    /// <returns>The clamped angle</returns>
    public static float ClampAngle(float angle, float min, float max){
        if (angle > 180f) { angle -= 360; }
        angle = Mathf.Clamp(angle, min, max);

        if (angle < 0f) { angle += 360; }
        return angle;
    }
    
    /// <summary>
    /// Useful inside Sin functions, calculates a given point on a circle
    /// </summary>
    /// <param name="index">The current point to be calculate in the circle</param>
    /// <param name="iterations">The max number of points to sum up the circle</param>
    /// <returns>A radian angle</returns>
    public static float CalculateRadianAngle(float index, int iterations) => index * 2 * Mathf.PI / iterations;

    /// <summary>
    /// Used to calculate the vector direction representing a point on a circumferance of a circle
    /// </summary>
    /// <param name="index">The current point to be calculate in the circle</param>
    /// <param name="iterations">The max number of points to sum up the circle</param>
    /// <param name="axisToAffect">The axis to create the circle on (Should be either 0,1)</param>
    /// <returns>A vector representing a point on the circumferance of a circle</returns>
    public static Vector3 CalculateCircularVectorAngle(float index, int iterations, Vector3 axisToAffect){
        float angle = CalculateRadianAngle(index, iterations);
        return Vector3.Scale(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), Mathf.Cos(angle)), axisToAffect);
    }
    
    /// <summary>
    /// Useful for creating "Wrap-around" values between 0 and the maxValue. (By default will iterate upwards)
    /// </summary>
    /// <param name="value">The value we wish to modify</param>
    /// <param name="maxValue">The max value </param>
    /// <param name="doesReverse">Reverses the operation if true</param>
    public static void ModulusCounter(ref int value, int maxValue, bool doesReverse = false){
        if(doesReverse){
            value = (value - 1 + maxValue) % maxValue;
            return;
        }
        value = (value + 1) % maxValue;
    }

    [Serializable] public class MinMaxF{
        public float min, max;

        public MinMaxF(float min, float max){
            this.min = min;
            this.max = max;
        }
    }

    [Serializable] public class MinMax{
        public int min, max;

        public MinMax(int min, int max){
            this.min = min;
            this.max = max;
        }
    }

    public class Velocity{
        Vector3 previousPosition;
        public Vector3 value{
            get;
            private set;
        }
        public Velocity(Vector3 position) => this.previousPosition = position;
        /// <summary>
        /// This function should be called from 'FixedUpdate' for accurate results
        /// </summary>
        /// <param name="position">The current position</param>
        /// <returns>The velocity given the last position over time</returns>
        public Vector3 CalculateVelocity(Vector3 position){
            value = (position - previousPosition) / Time.fixedDeltaTime;
            this.previousPosition = position;
            return value;
        }
    }
}
