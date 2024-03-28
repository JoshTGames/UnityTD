using System.Collections;
using System.Collections.Generic;
using AstralCandle.Animation;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using AstralCandle.Entity;
using UnityEngine.EventSystems;
using AstralCandle.TowerDefence;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class BuildUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{    
    public static BuildUI instance;
    [SerializeField] MiscSettings miscSettings;
    [SerializeField] Slot structureSlotUI;
    [SerializeField] int uiLayer;
    [SerializeField] LayerMask buildingObstacleLayers;
    [SerializeField] BuildSystem.PlacementColours placementColours;
    [SerializeField] BuildProfile[] buildings;
    

    [HideInInspector] public bool isOpen{
        get;
        private set;
    }

    BuildSystem building;
    public BuildSystem Building{
        get => building;
        set{
            if(building?.Entity != null){ Destroy(building.Entity.gameObject); }
            building = value;
        }
    }
    Transform slotParent;
    GameObject UI;
    Scrollbar scroll;
    bool resetScrollValue;

    private void Start(){
        instance = this;

        UI = transform.GetChild(0).gameObject;
        scroll = GetComponentInChildren<Scrollbar>();        

        slotParent = UI.GetComponentInChildren<ScrollRect>().transform.GetChild(0);
        foreach(BuildProfile structure in buildings){
            Slot newSlot = Instantiate(structureSlotUI, slotParent);
            EntityStructure s = Instantiate(structure.Building, newSlot.transform);

            // Apply the name of the structure
            newSlot.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = s.GetName();

            // Apply mesh to the slot
            Transform mesh = s.transform.GetChild(0);
            mesh.SetParent(newSlot.transform.GetChild(1));
            Destroy(s.gameObject);
            mesh.gameObject.layer = uiLayer;
            mesh.transform.localScale *= 45;
            mesh.localPosition = new Vector3(0, -50, -75);

            newSlot.mesh = mesh.GetComponent<Renderer>();
            newSlot.profile = structure;
            newSlot.obstacleLayers = buildingObstacleLayers;
            newSlot.placementColours = placementColours;
        }
    }

    private void LateUpdate() {
        float value = miscSettings.easeInSettings.Play(!isOpen);

        bool showUI = miscSettings.easeInSettings.percent >0;
        UI.SetActive(showUI);
        miscSettings.resourceUI.gameObject.SetActive(showUI);

        Vector3 scale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, value);
        transform.localScale = scale;
        miscSettings.resourceUI.localScale = scale;

        if(UI.activeSelf && resetScrollValue){
            resetScrollValue = false;
            scroll.value = 0;
        }

        foreach(ResourceStash s in miscSettings.stash){
            if(!Keep.instance.resources.ContainsKey(s.associatedResource)){ continue; }
            s.label.text = $"{Keep.instance.resources[s.associatedResource]}";
        }        
    }


    public void ToggleOpen(){
        isOpen = !isOpen;
        EntityTooltip.instance.tooltip = null;
        if(isOpen){ resetScrollValue = true; }
    }

    public void OnPointerEnter(PointerEventData eventData) => PlayerControls.instance.canZoom = false;

    public void OnPointerExit(PointerEventData eventData) => PlayerControls.instance.canZoom = true;

    [Serializable] public class ResourceStash{
        [HideInInspector] public float quantity;
        public TMP_Text label;
        public ResourceData associatedResource;
    }

    [Serializable] public class MiscSettings{
        public AnimationInterpolation easeInSettings;
        public Transform resourceUI;
        public ResourceStash[] stash;
    }
}
