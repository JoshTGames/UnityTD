using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AstralCandle.TowerDefence;

public class tstEntity : CharacterEntity{
    [SerializeField] int thisHp = 100;
    private void FixedUpdate() {
        health = thisHp;
    }
    
    protected override ERR Attack(HealthEntity entity)
    {
        throw new System.NotImplementedException();
    }

    protected override ERR MoveTo(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnDamage()
    {
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
}
