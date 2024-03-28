using System.Collections;
using System.Collections.Generic;
using AstralCandle.Entity;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "New build", menuName = "ScriptableObjects/BuildSystem/New build")]
public class BuildProfile : ScriptableObject{
    [SerializeField] EntityStructure building;
    [SerializeField] Resource[] requiredResources;

    public EntityStructure Building{ get => building; }
    public Resource[] RequiredResources{ get => requiredResources; }

    /// <summary>
    /// Compares the stashed resources of the keep to the requirements of this structure.
    /// </summary>
    /// <returns>True/False based on success</returns>
    public bool CanAfford(){
        for(int i = 0; i < requiredResources.Length; i++){
            Resource r = requiredResources[i];
            if(!Keep.instance.resources.ContainsKey(r.resource)){ return false; }
            else if(Keep.instance.resources[r.resource] < r.quantity){ return false; }
        }
        return true;        
    }

    [System.Serializable] public class Resource{
        /// <summary>
        /// The resource that will be required to build this structure
        /// </summary>
        public ResourceData resource;

        /// <summary>
        /// The quantity of the resource to make this structure
        /// </summary>
        public int quantity;
    }
}
