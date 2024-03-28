using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AstralCandle.Entity;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "Is Existing", menuName = "ScriptableObjects/DialogueSystem/FunctionChecks/Is Existing")]
public class IsExisting : DialogueFunctionCheck{
    [SerializeField] Entity[] types;
    public override FunctionState DependencyCheck(){
        int isExisting = PlayerControls.instance.entities.GetAllOfType(types).Count;
        return (isExisting == 0)? FunctionState.Success : FunctionState.Fail;
    }
}
