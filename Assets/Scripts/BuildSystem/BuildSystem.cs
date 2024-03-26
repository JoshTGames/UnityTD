using System.Collections;
using System.Collections.Generic;
using AstralCandle.Entity;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class BuildSystem{
    public Entity Entity{
        get;
        private set;
    }
    public Vector3 Position{
        get;
        private set;
    }
    float yOffset;

    public BuildSystem(Entity entity, RaycastHit ray, float specularAmount = 0, float dithering = 0.3f){
        this.Entity = entity;
        Collider col = Entity.GetComponent<Collider>();
        yOffset = col.bounds.extents.y + ray.collider.bounds.center.y + ray.collider.bounds.extents.y;
        SetPosition(ray);
        entity.transform.position = Position;

        col.enabled = false;
        entity.isEnabled = false;
        entity.enabled = false;
        entity.Material.SetFloat("_SpecularAmount", specularAmount);
        entity.Material.SetFloat("_Dithering", dithering);
    }

    public void Build(){}

    public void SetPosition(RaycastHit ray){
        Vector3 clampedPos = GameLoop.Grid.RoundToGrid(ray.point, 2);
        clampedPos.y = yOffset;
        Position = clampedPos;
    }
}
