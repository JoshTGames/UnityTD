using System.Collections;
using System.Collections.Generic;
using AstralCandle.Animation;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinLose : MonoBehaviour{
    [SerializeField] AnimationInterpolation easeIn;

    [SerializeField] Image image;
    float defaultImageAlpha;

    [SerializeField] Transform statePanel;
    [SerializeField] float yOffset;
    Vector3 targetPanelPosition;
    [SerializeField] Color win, lose;

    [SerializeField] TMP_Text label;
    private void Start(){
        easeIn.ResetTime();

        defaultImageAlpha = image.color.a;
        targetPanelPosition = statePanel.position;
    }

    private void LateUpdate() {
        float value = easeIn.Play(label.text == "");

        Color panelC = image.color;
        panelC.a = defaultImageAlpha * value;
        image.color = panelC;

        statePanel.position = Vector3.LerpUnclamped(targetPanelPosition + new Vector3(0, yOffset), targetPanelPosition, value);
    }


    public void SetState(string text){
        label.text = text;
        label.color = (text == "You Win!")? win: lose;
    }



}
