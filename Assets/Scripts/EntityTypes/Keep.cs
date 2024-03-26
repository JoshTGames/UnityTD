using System.Collections.Generic;
using System.Transactions;
using AstralCandle.Entity;
using UnityEngine;
public class Keep : EntityDefensiveStructure, IStorage{
    public static Dictionary<ResourceData, int> resources = new();


    public Keep(int ownerId) : base(ownerId){}

    public EntityERR Deposit(ResourceData.Resource resourceData, IPickup entity){
        EntityCharacter _entity = entity as EntityCharacter;

        if(!_entity){ return EntityERR.INVALID_CALL; }
        else if(OwnerId != _entity.OwnerId){ return EntityERR.NOT_PERMITTED; }
        else if(!InRange(_entity.transform.position, _entity.GetInteractRadius())){ return EntityERR.NOT_IN_RANGE; }

        bool hasKey = resources.ContainsKey(resourceData.resource);
        if(!hasKey){ resources.Add(resourceData.resource, resourceData.quantity); }
        else{ resources[resourceData.resource] += resourceData.quantity; }

        entity.RemoveEquipped();
        return EntityERR.SUCCESS;
    }

    public ResourceData.Resource GetResource(){
        throw new System.NotImplementedException();
    }

    public EntityERR Withdraw(ResourceData.Resource resource, IStorage storage)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnImmortalHit()
    {
        throw new System.NotImplementedException();
    }

    protected override void Start(){
        base.Start();
        resources = new();
    }
}
