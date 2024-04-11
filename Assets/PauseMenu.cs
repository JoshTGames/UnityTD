using System.Collections;
using System.Collections.Generic;
using AstralCandle.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour{
    [SerializeField] AnimationInterpolation easeIn;

    GameObject ui;

    float? timeTillMenu;

    private void Start() {
        ui = transform.GetChild(0).gameObject;
        Application.targetFrameRate = 30;
    }



    private void LateUpdate() {
        if(!PlayerControls.instance){ return; }
        float value = easeIn.Play(!PlayerControls.instance.Paused);
        
        ui.SetActive(easeIn.percent > 0);
        ui.transform.localScale = Vector3.one * value;


        if(timeTillMenu != null && Time.time >= (float)timeTillMenu){
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ChangeScene(string sceneName) => SceneManager.LoadScene(sceneName);
    public void Exit() => Application.Quit();

    public void ReturnToMenu(float inDuration){
        timeTillMenu = Time.time + inDuration;
    }
}
