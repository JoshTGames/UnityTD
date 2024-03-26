using System.Collections.Generic;
using AstralCandle.Entity;
using UnityEngine;
using UnityEngine.Events;
public class Keep : EntityDefensiveStructure, IStorage{
    public static Dictionary<ResourceData, int> resources = new();
    [SerializeField] OccupantGeneration occupantGenerationSettings;
    

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

    protected override void OnImmortalHit(){}

    public override void Run(){
        base.Run();
        occupantGenerationSettings.SpawnOccupant(GetMaxOccupants(), ref occupants);
    }

    protected override void Start(){
        base.Start();
        resources = new();
    }

    [System.Serializable] public class OccupantGeneration{
        public ResourceData resource;
        public int quantity = 10;

        public float occupantGenerationTimer = 10;
        public UnityEvent onOccupantGeneration;
        float elapsedOccupantGenerationTimer = 0;

        /// <summary>
        /// Spawns occupants in this structure every x seconds if we have sufficient amount of resources
        /// </summary>
        /// <param name="maxOccupants">The max number of occupants that can exist in this structure</param>
        /// <param name="occupants">The concurrent number of occupants in this structure</param>
        public void SpawnOccupant(int maxOccupants, ref int occupants){
            elapsedOccupantGenerationTimer -= Time.fixedDeltaTime;
            if(!resources.ContainsKey(resource) || resources[resource] < quantity || occupants >= maxOccupants){
                elapsedOccupantGenerationTimer = occupantGenerationTimer;
                return;
            }

            if(elapsedOccupantGenerationTimer <= 0){
                elapsedOccupantGenerationTimer = occupantGenerationTimer;
                resources[resource] -= quantity;
                occupants++;
                onOccupantGeneration?.Invoke();
            }
        }
    }
}
