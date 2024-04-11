using System.Collections;
using System.Collections.Generic;
using AstralCandle.Entity;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "Enable buildUI?", menuName = "ScriptableObjects/DialogueSystem/Actions/Enable buildUI?")]
public class DoEnableBuildUI : DialogueAction{    
    [SerializeField] bool enableUI;
    public override void Run(){
        BuildUI.instance.enableSystem = enableUI;
    }
}
