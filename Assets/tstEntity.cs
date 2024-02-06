using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AstralCandle.TowerDefence;

public class tstEntity : CharacterEntity{
    public override void OnHover(bool hovered){
        transform.localScale = (hovered)? Vector3.one * 1.1f : Vector3.one;
    }

    public override void OnSelect(bool selected){
        transform.GetChild(0).gameObject.SetActive(selected);
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
}
