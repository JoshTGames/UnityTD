using AstralCandle.TowerDefence.AI;
using AstralCandle.TowerDefence;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class BT_Interact : BT_Node{
    CharacterEntity entity;
    public BT_Interact(CharacterEntity entity) => this.entity = entity;
    public override NodeState Evaluate(){
        if(entity.targetEntity == null){ return NodeState.Fail; }
        BaseEntity.ERR err = entity.targetEntity.Interact(entity);
        
        if(err == BaseEntity.ERR.SUCCESS){ 
            entity.DestroyEntity();
            return NodeState.Success;
        }
        else if(err == BaseEntity.ERR.NOT_IN_RANGE){
            entity.targetPosition = entity.targetEntity.transform.position;
        }
        else if(err == BaseEntity.ERR.MAX_OCCUPANTS){ // If not able to interact with entity...
            entity.targetPosition = entity.transform.position;
        }
        

        return NodeState.Fail;
    }
}
