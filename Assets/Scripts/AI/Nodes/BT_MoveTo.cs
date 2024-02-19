using UnityEngine;
using AstralCandle.TowerDefence.AI;
using AstralCandle.TowerDefence;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class BT_MoveTo : BT_Node{
    CharacterEntity entity;
    ContextSteering steering;
    float deltaDistance;


    public BT_MoveTo(CharacterEntity entity, ContextSteering steering, float deltaDistance){
        this.entity = entity;
        this.steering = steering;
        this.deltaDistance = deltaDistance;
    }

    public override NodeState Evaluate(){
        Vector3 thisPosition = entity.transform.position;
        Vector3 displacement = entity.targetPosition - thisPosition;
        // If at destination
        if(displacement.sqrMagnitude <= deltaDistance){ 
            entity.moveDirection = Vector3.zero;
            return NodeState.Success; 
        }
        Vector3 direction = Vector3.ClampMagnitude(entity.targetPosition - thisPosition, 1); // (entity.targetPosition - thisPosition).normalized
        entity.moveDirection = steering.Solve(thisPosition + (Vector3.up * entity.aISettings.offset), direction);
        return NodeState.Running;
    }
}
