using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour{
    public static ProgressBar instance;

    [Header("Progress Settings")]
    [SerializeField] AnimationCurve curve;
    [SerializeField] Image bar, delayedBar;
    [SerializeField] TMP_Text state, value;
    [SerializeField] float updateDuration = 1f;
    [SerializeField, Range(0, 1)] float delayedSmoothing;

    float elapsedTime, smoothVelocity;
    float _targetValue = 0, previousValue = 0;
    float TargetValue{
        get => _targetValue;
        set{
            if(value == _targetValue){ return; }
            previousValue = _targetValue;
            _targetValue = value;
            elapsedTime = 0;
        }
    }

    /// <summary>
    /// Updates the target for our bars to reach for
    /// </summary>
    public void UpdateTarget(float value) => TargetValue = value;
    /// <summary>
    /// Updates the colour of the bar 
    /// </summary>
    public void UpdateColour(Color color) => bar.color = color;
    /// <summary>
    /// Updates the text of the bar 
    /// </summary>
    public void UpdateState(string state) => this.state.text = state;
    /// <summary>
    /// Updates the text of the bar 
    /// </summary>
    public void UpdateValue(string value) => this.value.text = value;

    void UpdateProgress(){
        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / updateDuration);
        float animation = curve.Evaluate(progress);

        bar.fillAmount = Mathf.LerpUnclamped(previousValue, TargetValue, animation);
        delayedBar.fillAmount = Mathf.SmoothDamp(delayedBar.fillAmount, bar.fillAmount, ref smoothVelocity, delayedSmoothing);
    }

    void LateUpdate() {
        UpdateProgress();
    }

    private void Awake() => instance = this;
}
