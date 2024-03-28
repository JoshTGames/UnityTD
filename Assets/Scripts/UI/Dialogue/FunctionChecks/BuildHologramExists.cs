using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AstralCandle.Entity;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "BuildHologram Exists", menuName = "ScriptableObjects/DialogueSystem/FunctionChecks/BuildHologramExists")]
public class BuildHologramExists : DialogueFunctionCheck{
    [SerializeField] bool mustExist;
    public override FunctionState DependencyCheck(){
        bool exists = BuildUI.instance.Building != null;
        return (exists == mustExist)? FunctionState.Success : FunctionState.Fail;
    }
}
