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
    public bool CanPlace{ get; private set; } = true;

    public BuildProfile profile{ get; private set; }
    float yOffset;
    Collider _collider;
    Vector3 colExtents;
    LayerMask obstacles;
    PlacementColours colours;

    public BuildSystem(Entity entity, BuildProfile profile, RaycastHit ray, LayerMask obstacleLayers, PlacementColours colours, float specularAmount = 0, float dithering = 0.3f){
        this.Entity = entity;
        this.profile = profile;
        this._collider = Entity.GetComponent<Collider>();
        this.colExtents = this._collider.bounds.extents;
        this.obstacles = obstacleLayers;
        this.colours = colours;

        float tempOffset = PlayerControls.instance.Map.bounds.extents.y;
        if(ray.collider){ tempOffset = ray.collider.bounds.center.y + ray.collider.bounds.extents.y; }

        this.yOffset = this._collider.bounds.extents.y + tempOffset;
        SetPosition(ray);
        entity.transform.position = Position;

        this._collider.enabled = false;
        this.Entity.isEnabled = false;
        this.Entity.enabled = false;
        this.Entity.Material.SetFloat("_SpecularAmount", specularAmount);
        this.Entity.Material.SetFloat("_Dithering", dithering);
    }

    public bool Build(){
        if(!CanPlace){ return false; }
        foreach(BuildProfile.Resource r in profile.RequiredResources){ Keep.instance.resources[r.resource] -= r.quantity; }
        return true;
    }

    public void SetPosition(RaycastHit ray){
        Vector3 clampedPos = GameLoop.Grid.RoundToGrid(ray.point, 2);
        clampedPos.y = yOffset;
        this.Position = clampedPos;
        Collider[] cols = Physics.OverlapBox(Position, colExtents, Quaternion.identity, obstacles);
        this.CanPlace = cols == null || cols.Length == 0;

        this.Entity.Material.SetColor("_SpecularColour", (CanPlace)? colours.canPlace : colours.cantPlace);
    }

    [System.Serializable] public class PlacementColours{
        [ColorUsage(showAlpha: false, hdr: true)]
        public Color canPlace = Color.green, cantPlace = Color.red;
    }
}
