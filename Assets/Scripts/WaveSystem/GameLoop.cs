using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEditor;
using AstralCandle.Entity;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class GameLoop : MonoBehaviour{    
    public static GameLoop instance;
    const int NUMBER_OF_SPAWN_DIRECTIONS = 4; // UP, DOWN, LEFT, RIGHT

    [SerializeField] bool showDebug;
    [SerializeField] Transform map;
    [SerializeField] Settings settings;
    [SerializeField] WaveProfile[] waves;

    int _wave = 0, group = 0;
    bool spawnGroup = false;
    public float intermission{
        get;
        private set;
    } = 0;
    float spawnDelayElapsed = 0;
    float mapScale;

    /// <summary>
    /// The concurrent wave we are on
    /// </summary>
    public int wave{
        get => _wave;
        private set{
            if(value == _wave){ return; }
            _wave = value;
            previousScale = targetScale;

            targetScale = new Vector3(mapScale, 1, mapScale);
            scalingElapsedTime = 0;
            playFX = true;
        }
    }

    /// <summary>
    /// Shows the con-current group that we will use to spawn entities
    /// </summary>
    WaveProfile.SpawnGroupData CurrentWaveGroup{ get => (waves[wave].waveData.Length > group)? waves[wave].waveData[group] : null; }

    /// <summary>
    /// IDs of the characters
    /// </summary>
    HashSet<int> aliveEntities = new();

    #region COSMETIC
    public UnityEvent<Transform> OnScaled;
    Vector3 previousScale, targetScale;
    float scalingElapsedTime = 0;
    bool playFX = false;
    #endregion

    void Start(){
        instance = this;   

        targetScale = map.localScale;
        previousScale = targetScale;

        intermission = waves[wave].intermission;

        UpdateMapScale();
    }

    private void Update() {
        #region ANIMATION
        scalingElapsedTime += Time.deltaTime;
        float percent = Mathf.Clamp01(scalingElapsedTime / settings.scalingDuration);
        float curve = settings.scalingAnimation.Evaluate(percent);
        
        map.localScale = Vector3.LerpUnclamped(previousScale, targetScale, curve);

        if(percent >= 1 && playFX){ 
            OnScaled?.Invoke(map); 
            playFX = false;
        }
        #endregion

        intermission -= (intermission >0)? Time.deltaTime : 0;
        spawnDelayElapsed -= (spawnDelayElapsed >0)? Time.deltaTime : 0;

        if(intermission > 0){ return; }

        if(spawnDelayElapsed <= 0 && CurrentWaveGroup != null){
            for(int i = 0; i < CurrentWaveGroup.quantity; i++){
                foreach(IWave _char in CurrentWaveGroup.group.entities){
                    if(_char == null){ continue; } // Stops any non-'IWave' entites from being spawned
                    EntityCharacter character = _char as EntityCharacter;

                    float angle = Utilities.CalculateRadianAngle((int)CurrentWaveGroup.spawnDirection, NUMBER_OF_SPAWN_DIRECTIONS);
                    Vector3 directionalOffset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (mapScale/2);

                    bool isHorizontal = (int)CurrentWaveGroup.spawnDirection == 1 || (int)CurrentWaveGroup.spawnDirection == 3;
                    float rndValue = UnityEngine.Random.Range(-1f, 1f);

                    
                    // Left/Right
                    directionalOffset.x = (isHorizontal)? rndValue * mapScale/3 : directionalOffset.x; 
                    directionalOffset.y = 1.25f;
                    // Up/Down
                    directionalOffset.z = (!isHorizontal)? rndValue * mapScale/3 : directionalOffset.z; 


                    // Spawns the character and adds it to alive entities list
                    aliveEntities.Add(
                        Instantiate(
                            character, 
                            map.position + directionalOffset, 
                            Quaternion.identity, 
                            GameObject.Find("_GAME_RESOURCES_").transform
                        ).GetInstanceID()
                    );
                }
            }
            spawnDelayElapsed = CurrentWaveGroup.spawnDelay;
            group++;
        }                
    }

    /// <summary>
    /// Removes the entity from the data so we can start the next wave
    /// </summary>
    /// <param name="id"></param>
    public void RemoveEntity(int id){
        if(!aliveEntities.Contains(id)){ return; }
        aliveEntities.Remove(id);
        
        // If there are no alive entities and there are no more groups to spawn...
        if(aliveEntities.Count <= 0 && CurrentWaveGroup == null && waves.Length-1 > wave){
            wave++;
            group = 0;
            intermission = waves[wave].intermission;
            UpdateMapScale();
        }
    }

    void UpdateMapScale(){
        mapScale = Utilities.Remap(wave, 1, waves.Length, settings.mapSize.min, settings.mapSize.max);
        mapScale = Mathf.Clamp(Grid.FloorToGrid(mapScale, settings.cellSize), settings.mapSize.min, settings.mapSize.max);
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
