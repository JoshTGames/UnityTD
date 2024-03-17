using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "ScriptableObjects/EntitySystem/New Resource")]
public class ResourceData : ScriptableObject{
    public sealed class Resource{
        public ResourceData resource;
        public int quantity;
        public Resource(ResourceData resource, int quantity){
            this.resource = resource;
            this.quantity = quantity;
        }
    }
}
