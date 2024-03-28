using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AstralCandle.Entity;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "Human not holding anything", menuName = "ScriptableObjects/DialogueSystem/FunctionChecks/Human not holding anything")]
public class HumanNotHoldingAnything : DialogueFunctionCheck{
    [SerializeField] Human human;
    public override FunctionState DependencyCheck(){
        List<Entity> humans = PlayerControls.instance.entities.GetAllOfType(human);
        foreach(Entity e in humans){
            Human h = e as Human;
            if(h == null){ continue; }

            if(h.heldResource != null){ return FunctionState.Fail; }
        }
        return FunctionState.Success;
    }
}
