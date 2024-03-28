using UnityEngine;
using UnityEngine.UI;
using AstralCandle.Animation;
using TMPro;
using Unity.VisualScripting;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class ProgressBar : MonoBehaviour{
    public static ProgressBar instance;
    [SerializeField] AnimationInterpolation easeInAnimation;

    [Header("Progress Settings")]
    [SerializeField] AnimationInterpolation animationSettings;
    [SerializeField] Image bar, delayedBar;
    [SerializeField] TMP_Text state, value;
    [SerializeField, Range(0, 1)] float delayedSmoothing;

    Animator _animator;
    float smoothVelocity;
    float _targetValue = 0, previousValue = 0;
    float TargetValue{
        get => _targetValue;
        set{
            if(value == _targetValue){ return; }
            previousValue = _targetValue;
            _targetValue = value;
            animationSettings.ResetTime();
        }
    }

    /// <summary>
    /// Updates the target for our bars to reach for
    /// </summary>
    public void UpdateTarget(float value){
        if(value == TargetValue){ return; }
        TargetValue = value;
    }
    /// <summary>
    /// Updates the colour of the bar 
    /// </summary>
    public void UpdateColour(Color color){
        if(color == this.bar.color){ return; }
        this.bar.color = color;
        _animator.SetTrigger("CircleShake");
    }
    /// <summary>
    /// Updates the text of the bar 
    /// </summary>
    public void UpdateState(string state){
        if(state == this.state.text){ return; }
        this.state.text = state;
        _animator.SetTrigger("StateShake");
    }

    /// <summary>
    /// Updates the text of the bar 
    /// </summary>
    public void UpdateValue(string value){
        if(value == this.value.text){ return; }
        this.value.text = value;
        _animator.SetTrigger("ValueShake");
    }

    void UpdateProgress(){
        float value = animationSettings.Play();

        bar.fillAmount = Mathf.LerpUnclamped(previousValue, TargetValue, value);
        delayedBar.fillAmount = Mathf.SmoothDamp(delayedBar.fillAmount, bar.fillAmount, ref smoothVelocity, delayedSmoothing);
    }

    void LateUpdate(){
        UpdateProgress();
        float value = easeInAnimation.Play(GameLoop.instance?.CurrentGame == null);
        transform.GetChild(0).localScale = Vector3.one * value;
        transform.GetChild(0).gameObject.SetActive(easeInAnimation.percent > 0);
    }
    private void Awake() => instance = this;
    private void Start(){
        _animator = transform.GetChild(0).GetComponent<Animator>();
        easeInAnimation.ResetTime();
    }
}