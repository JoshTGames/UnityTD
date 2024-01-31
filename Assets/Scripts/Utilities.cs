using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utilities{

    /// <summary>
    /// When used inside a Sin/Cos function, will return an output used to create a circle
    /// </summary>
    /// <param name="index">The current point to be calculate in the circle</param>
    /// <param name="iterations">The max number of points to sum up the circle</param>
    /// <returns>A value representing a point on the circumferance of a circle</returns>
    static float GetRadianAngle(float index, int iterations) => index * 2 * Mathf.PI / iterations;

    /// <summary>
    /// Used to calculate the vector direction representing a point on a circumferance of a circle
    /// </summary>
    /// <param name="index">The current point to be calculate in the circle</param>
    /// <param name="iterations">The max number of points to sum up the circle</param>
    /// <returns>A vector direction</returns>
    public static Vector3 GetRadianDirectionXZ(float index, int iterations){
        float angle = GetRadianAngle(index, iterations);
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
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

    [Serializable] public class MinMax{
        public float min, max;

        public MinMax(float min, float max){
            this.min = min;
            this.max = max;
        }
    }
}
