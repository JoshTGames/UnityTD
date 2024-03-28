using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AstralCandle.Entity;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "Has Selected", menuName = "ScriptableObjects/DialogueSystem/FunctionChecks/Has Selected")]
public class HasSelectedType : DialogueFunctionCheck{
    [SerializeField] Entity _type;
    public override FunctionState DependencyCheck() => (PlayerControls.instance.entities.HasTypeInSelected(_type.GetType()))? FunctionState.Success : FunctionState.Fail;
}
