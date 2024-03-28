using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "Tutorial Wave Finished", menuName = "ScriptableObjects/DialogueSystem/FunctionChecks/Tutorial Wave Finished")]
public class TutorialWaveFinished : DialogueFunctionCheck{
    public override FunctionState DependencyCheck() => (Tutorial.instance.CompletedWave)? FunctionState.Success : FunctionState.Fail;
}
