using UnityEngine.Events;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Animation{
    /// <summary>
    /// Provides an easy means of animating values using a curve
    /// </summary>
    [System.Serializable] public class AnimationInterpolation{
        [SerializeField, Tooltip("The animation we will be applying onto a value")] AnimationCurve curve;
        [SerializeField, Tooltip("The duration of this animation")] float duration;
        [SerializeField, Tooltip("Once complete, will allow perform these actions")] UnityEvent onComplete;
        
        float _elapsedTime;        
        float ElapsedTime{
            get => _elapsedTime;
            set => _elapsedTime = Mathf.Clamp(value, 0, duration);
        }
        bool callEvent;

        public AnimationInterpolation(AnimationCurve curve, float duration){
            this.curve = curve;
            this.duration = duration;
        }

        /// <summary>
        /// Resets the timer
        /// </summary>
        public void ResetTime(bool reverse = false){
            callEvent = true;
            ElapsedTime = (!reverse)? 0 : 1;
        }        
        
        /// <summary>
        /// Will play the animation
        /// </summary>
        /// <returns>The interpolated value</returns>
        public float Play(bool reverse = false){
            ElapsedTime += (!reverse)? Time.deltaTime : -Time.deltaTime;
            float percent = Mathf.Clamp01(ElapsedTime / duration);

            if((percent == 1 || percent == 0) && callEvent){ 
                onComplete?.Invoke(); 
                callEvent = false;
            }
            return curve.Evaluate(percent);
        }
    }
}