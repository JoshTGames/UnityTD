using System.Collections;
using System.Collections.Generic;
using AstralCandle.Entity;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "Spawn Entities", menuName = "ScriptableObjects/DialogueSystem/Actions/Spawn Entities")]
public class SpawnEntity : DialogueAction{    
    [SerializeField] Entity[] entities;
    public override void Run() => GameLoop.instance.SpawnEntities(ref Tutorial.instance.spawnPositions, entities);
}
