using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AstralCandle.TowerDefence;
using System.Linq;
using AstralCandle.Entity;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler{
    [SerializeField] float rotationDuration;
    [SerializeField] Color selectionColour = Color.grey, clickedColour = Color.black, disabledColour;
    [SerializeField, Range(0.5f, 1)] float ditherRange = 0.9f;
    [HideInInspector] public Renderer mesh;
    [HideInInspector] public BuildProfile profile;
    [HideInInspector] public LayerMask obstacleLayers;
    [HideInInspector] public BuildSystem.PlacementColours placementColours;

    Camera cam;
    bool doRotate = false;
    bool canAfford = false;
    List<ColourHandler> colours;
    Color currentColour;

    public void OnPointerClick(PointerEventData eventData){
        if(!canAfford){ return; }
        BuildUI.instance.ToggleOpen();

        // Trigger function
        Entity e = Instantiate(profile.Building, PlayerControls.instance.WorldPoint.point, Quaternion.identity, GameObject.Find("_GAME_RESOURCES_").transform);
        BuildUI.instance.Building = new BuildSystem(e, profile, PlayerControls.instance.WorldPoint, obstacleLayers, placementColours);
    }

    public void OnPointerEnter(PointerEventData eventData){
        doRotate = true;
        if(PlayerControls.instance.MouseDown){ return; }
        CreateTooltip();
        if(!canAfford){ return; }

        ApplyColours(selectionColour);
    }

    public void OnPointerExit(PointerEventData eventData){
        doRotate = false;
        if(PlayerControls.instance.MouseDown){ return; }
        EntityTooltip.instance.tooltip = null;
        if(!canAfford){ return; }

        ApplyColours();
    }

    public void OnPointerDown(PointerEventData eventData){
        EntityTooltip.instance.tooltip = null;
        if(!canAfford){ return; }
        ApplyColours(clickedColour);
        BuildUI.instance.onClickSFX.PlaySound(BuildUI.instance.source);
    }

    public void OnPointerUp(PointerEventData eventData){
        if(!canAfford){ return; }
        ApplyColours(selectionColour);
    }


    void CreateTooltip(){
        // Create tooltip
        Sprite[] attributes = profile.Building.GetAttributes().Skip(1).ToArray();
        EntityTooltip instance = EntityTooltip.instance;
        instance.tooltip = new EntityTooltip.Tooltip(profile.Building.GetName(), profile.Building.GetDescription(), attributes);
        // Manage contents
        for(int i = 0; i < profile.RequiredResources.Length; i++){
            BuildProfile.Resource r = profile.RequiredResources[i];
            int currentQuantity = 0;
            if(Keep.instance.resources.ContainsKey(r.resource)){ currentQuantity = Mathf.Clamp(Keep.instance.resources[r.resource], 0, r.quantity); }
            EntityTooltip.Contents content = new EntityTooltip.Contents(r.resource.name, EntityTooltip.instance.resourceColour, r.resource.icon, $"{currentQuantity}/{r.quantity}");

            instance.tooltip.AddContents(instance.ContentsUIPrefab, instance.TooltipObject, ref instance.contents, content);
            instance.contents[r.resource.name].SetPercent((float)currentQuantity / r.quantity);
        }        
    }

    /// <summary>
    /// Bulk applies colour on all our images/labels
    /// </summary>
    /// <param name="colour">The colour we want to multiply our UI with</param>
    void ApplyColours(Color? colour = null){
        currentColour = (colour != null)? (Color)colour : colours[0].defaultColour;
        foreach(ColourHandler cH in colours){ 
            Color c = cH.defaultColour;
            if(colour != null){ c *= (Color)colour; }
            cH.SetColour(c); 
        }
    }

    /// <summary>
    /// As the mesh leaves the viewport, it will make the mesh "dither" out
    /// </summary>
    void DitherMesh(){
        RectTransform slot = transform as RectTransform;
        RectTransform panelParent = transform.parent.parent as RectTransform;
        Vector3 pos = panelParent.InverseTransformPoint(slot.position);

        float relativeX = pos.x / (panelParent.rect.width/2);
        relativeX = Mathf.Clamp(relativeX, -1, 1);
        relativeX = Mathf.Abs(relativeX);
        float lerp = 1 - Utilities.Remap(relativeX, ditherRange, 1, 0, 1);
        mesh.material.SetFloat("_Dithering", lerp);
    }


    private void LateUpdate() {
        DitherMesh();

        // Update tooltip content bars
        EntityTooltip instance = EntityTooltip.instance;
        for(int i = 0; i < profile.RequiredResources.Length && BuildUI.instance.isOpen && EntityTooltip.instance.tooltip != null; i++){
            BuildProfile.Resource r = profile.RequiredResources[i];
            if(!instance.contents.ContainsKey(r.resource.name)){ continue; }

            int currentQuantity = 0;
            if(Keep.instance.resources.ContainsKey(r.resource)){ currentQuantity = Mathf.Clamp(Keep.instance.resources[r.resource], 0, r.quantity); }
            instance.contents[r.resource.name].SetPercent((float)currentQuantity / r.quantity);
            instance.contents[r.resource.name].SetText($"{currentQuantity}/{r.quantity}");
        }

        canAfford = profile.CanAfford();
        if(!canAfford){
            ApplyColours(disabledColour);
            return;
        }
        else if(currentColour == disabledColour){ ApplyColours(); }

        if(!doRotate){ return; }
        mesh.transform.Rotate(transform.up, (360 / rotationDuration) * Time.deltaTime);        
    }

    private void Start() {
        cam = Camera.main;
        Image[] images = GetComponentsInChildren<Image>();
        TMP_Text label = GetComponentInChildren<TMP_Text>();
        colours = new(){ new ColourHandler(label) };
        foreach(Image i in images){ colours.Add(new ColourHandler(i)); }
    }
    

    public class ColourHandler{
        public Image image{
            get;
            private set;
        }
        public TMP_Text label{
            get;
            private set;
        }

        public Color defaultColour{
            get;
            private set;
        }

        public ColourHandler(Image image){
            this.image = image;
            this.defaultColour = image.color;
        }

        public ColourHandler(TMP_Text label){
            this.label = label;
            this.defaultColour = label.color;
        }

        public void SetColour(Color colour){
            if(image){ image.color = colour; }
            else if(label){ label.color = colour; }
        }
    }
}
