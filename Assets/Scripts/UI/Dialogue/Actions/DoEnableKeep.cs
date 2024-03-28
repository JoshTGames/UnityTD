using System.Collections;
using System.Collections.Generic;
using AstralCandle.Entity;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "Enable keep?", menuName = "ScriptableObjects/DialogueSystem/Actions/Enable keep?")]
public class DoEnableKeep : DialogueAction{    
    [SerializeField] bool enableKeep;
    public override void Run() => Keep.instance.isEnabled = enableKeep;
}
