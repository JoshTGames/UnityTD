using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public abstract class DialogueFunctionCheck : ScriptableObject{    
    public virtual FunctionState DependencyCheck() => FunctionState.Not_Implemented;

    /// <summary>
    /// Used to dictate if this dialogue requires waiting for a function to return successful
    /// </summary>
    public enum FunctionState{
        Not_Implemented,
        Fail,
        Success,        
    }
}
