using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "New Resource", menuName = "ScriptableObjects/EntitySystem/New Resource")]
public class ResourceData : ScriptableObject{
    public Sprite icon;
    public sealed class Resource{
        public ResourceData resource;
        public int quantity;
        public Resource(ResourceData resource, int quantity){
            this.resource = resource;
            this.quantity = quantity;
        }
    }
}
