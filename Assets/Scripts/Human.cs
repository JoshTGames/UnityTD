using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AstralCandle.TowerDefence;

public class Human : Character{
    public Human(string owner, Transform obj, int maxHealth, HashSet<DamageTypes> resistances, float maxMoveSpeed, int damage, float attackCooldown, float attackRadius, bool isImmortal = false) : base(owner, obj, maxHealth, resistances, maxMoveSpeed, damage, attackCooldown, attackRadius, isImmortal){}

    protected override ERR Attack(Entity entity)
    {
        throw new System.NotImplementedException();
    }

    protected override ERR Harvest(Resource resource)
    {
        throw new System.NotImplementedException();
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

    protected override ERR Pickup(Item item)
    {
        throw new System.NotImplementedException();
    }
}
