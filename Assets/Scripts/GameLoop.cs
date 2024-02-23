using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEditor;

public class GameLoop : MonoBehaviour{
    
    [SerializeField] bool showDebug;
    [SerializeField] Transform map;
    [SerializeField] Settings settings;

    [SerializeField] int _wave = 1;
    [SerializeField] int tst = 1;
    public static GameLoop instance;
    public UnityEvent<Transform> OnScaled;

    /// <summary>
    /// The concurrent wave we are on
    /// </summary>
    public int wave{
        get => _wave;
        set{
            if(value == _wave){ return; }
            _wave = value;
            previousScale = targetScale;

            float mapScale = Utilities.Remap(wave, 1, settings.maxWaves, settings.mapSize.min, settings.mapSize.max);
            mapScale = Mathf.Clamp(Grid.FloorToGrid(mapScale, settings.cellSize), settings.mapSize.min, settings.mapSize.max);

            targetScale = new Vector3(mapScale, 1, mapScale);
            elapsedTime = 0;
            playFX = true;
        }
    }

    Vector3 previousScale, targetScale;
    float elapsedTime = 0;
    bool playFX = false;

    void Start(){
        instance = this;   

        targetScale = map.localScale;
        previousScale = targetScale;
    }

    private void Update() {
        wave = tst;
        elapsedTime += Time.deltaTime;
        float percent = Mathf.Clamp01(elapsedTime / settings.scalingDuration);
        float curve = settings.scalingAnimation.Evaluate(percent);

        
        map.localScale = Vector3.LerpUnclamped(previousScale, targetScale, curve);

        if(percent >= 1 && playFX){ 
            OnScaled?.Invoke(map); 
            playFX = false;
        }
    }

    private void OnDrawGizmos() {
        #if UNITY_EDITOR
        if(!showDebug || !map){ return; }
        Handles.DrawWireCube(map.position, new Vector3(settings.mapSize.min, map.localScale.y, settings.mapSize.min));
        Handles.DrawWireCube(map.position, new Vector3(settings.mapSize.max, map.localScale.y, settings.mapSize.max));
        #endif
    }

    [Serializable] public class Settings{
        public AnimationCurve scalingAnimation;
        public float scalingDuration = 2f;
        public Utilities.MinMax mapSize = new Utilities.MinMax(15, 100);
        public int cellSize = 1;
        public int maxWaves = 20;
    }

    /// <summary>
    /// Maps positions to a grid
    /// </summary>
    public static class Grid{
        public static float RoundToGrid(float value, int cellSize) => Mathf.RoundToInt(value / cellSize) * cellSize; 
        public static Vector3 RoundToGrid(Vector3 value, int cellSize) => new Vector3(RoundToGrid(value.x, cellSize), RoundToGrid(value.y, cellSize), RoundToGrid(value.z, cellSize)); 
        public static float FloorToGrid(float value, int cellSize) => Mathf.FloorToInt(value / cellSize) * cellSize; 
        public static Vector3 FloorToGrid(Vector3 value, int cellSize) => new Vector3(FloorToGrid(value.x, cellSize), FloorToGrid(value.y, cellSize), FloorToGrid(value.z, cellSize)); 
    }
}
