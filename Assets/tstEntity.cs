using UnityEngine;
using AstralCandle.TowerDefence;
using AstralCandle.TowerDefence.AI;
using System.Collections.Generic;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class TstEntity : CharacterEntity{
    protected override ERR Attack(HealthEntity entity){ throw new System.NotImplementedException(); }

    protected override ERR MoveTo(Vector3 direction){
        transform.position += direction * (maxMoveSpeed * Time.fixedDeltaTime);
        return ERR.SUCCESS;
    }

    protected override void OnDamage()
    {
        Debug.Log($"{health} || {GetHealthPercentage()}/1");
    }

    protected override void OnDeath()
    {
    }

    protected override void OnHeal()
    {
    }

    protected override void OnImmortalHit()
    {
    }

    protected override void CreateTree(out BT_Node behaviour){
        BT_Interact interact = new BT_Interact(this);
        behaviour = new BT_Selector(new BT_Node[]{interact, moveTo});
    }

    public override ERR Interact(BaseEntity entity) => ERR.INVALID_CALL;
}
