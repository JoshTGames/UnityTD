using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "Has panned", menuName = "ScriptableObjects/DialogueSystem/FunctionChecks/Has panned")]
public class HasPanned : DialogueFunctionCheck{
    public override FunctionState DependencyCheck() => (PlayerControls.instance.IsPivoting)? FunctionState.Success : FunctionState.Fail;
}
