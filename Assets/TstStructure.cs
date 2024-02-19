using System.Collections.Generic;
using AstralCandle.TowerDefence;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class TstStructure : TowerEntity{
    public override ERR Interact(BaseEntity entity){
        if(entity.GetHashCode() == this.GetHashCode()){ return ERR.INVALID_CALL; }
        else if(Vector2.Distance(entity.transform.position, transform.position) > entity.interactDistance){ return ERR.NOT_IN_RANGE; }
        else if(entity.owner != owner){ return ERR.NO_PERMISSION; }
        return AddOccupant();
    }

    protected override void OnDamage()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnDeath()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnHeal()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnImmortalHit()
    {
        throw new System.NotImplementedException();
    }

    protected override ERR Attack(HealthEntity entity){
        ERR success = base.Attack(entity);
        if(success != ERR.SUCCESS){ return success; }
        entity.Damage(damage, attackType);
        return success;
    }


    protected override void Update() {
        base.Update();  
        List<HealthEntity> enemies = FindEnemies();
        if(enemies.Count <= 0){ return; }
        Attack(enemies[0]);
    }
}
