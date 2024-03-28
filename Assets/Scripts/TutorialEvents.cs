using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvents : MonoBehaviour{
    public static TutorialEvents instance;

    public bool hasPanned;
    public bool hasClickedHuman;
    public bool hasHarvestedResource;
    public bool hasHarvestedResources;
    public bool hasDepositedResources;
    public bool hasEnteredTheKeep;
    public bool hasPickedUpCoins;
    public bool hasOpenedBuildMenu;
    public bool hasClickedOnTower;
    public bool hasPlacedTower;
    // public bool has



    public bool IsPanning() => PlayerControls.instance.IsPivoting;



    void Start(){
        instance = this;
    }

}
