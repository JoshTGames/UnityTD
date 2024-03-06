using AstralCandle.Entity;

public class adsfjsdfusdfjs : EntityDefensiveStructure{
    public adsfjsdfusdfjs(int ownerId) : base(ownerId){}

    protected override void OnDamage(){}

    protected override void OnHeal(){}

    protected override void OnImmortalHit(){}

    protected void FixedUpdate() => Run();
}
