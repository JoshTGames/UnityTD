using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "BuildMenu Open", menuName = "ScriptableObjects/DialogueSystem/FunctionChecks/BuildMenu Open")]
public class BuildMenuOpen : DialogueFunctionCheck{
    public override FunctionState DependencyCheck() => (BuildUI.instance.isOpen)? FunctionState.Success : FunctionState.Fail;
}
