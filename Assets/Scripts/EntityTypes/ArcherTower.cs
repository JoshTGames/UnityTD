using AstralCandle.Entity;

public class ArcherTower : EntityDefensiveStructure{
    public ArcherTower(int ownerId) : base(ownerId){}

    protected override void OnImmortalHit(){}
}
