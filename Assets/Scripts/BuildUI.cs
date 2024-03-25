using System.Collections;
using System.Collections.Generic;
using AstralCandle.Animation;
using UnityEngine;
using UnityEngine.UI;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class BuildUI : MonoBehaviour{
    [SerializeField] AnimationInterpolation easeInSettings;
    public bool isOpen{
        get;
        private set;
    }

    BuildSystem building;
    public BuildSystem Building{
        get => building;
        set{
            if(building?.Entity != null){
                Destroy(building.Entity);
            }
            building = value;
        }
    }
    GameObject UI;
    Scrollbar scroll;
    bool resetScrollValue;

    private void Start(){
        UI = transform.GetChild(0).gameObject;
        scroll = GetComponentInChildren<Scrollbar>();
    }

    private void LateUpdate() {
        float value = easeInSettings.Play(!isOpen);
        UI.SetActive(easeInSettings.percent > 0);
        transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, value);

        if(UI.activeSelf && resetScrollValue){
            resetScrollValue = false;
            scroll.value = 0;
        }
    }

    public void ToggleOpen(){
        isOpen = !isOpen;
        if(isOpen){ resetScrollValue = true; }
    }
}
