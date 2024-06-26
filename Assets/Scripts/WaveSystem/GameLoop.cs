using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using AstralCandle.Entity;
using System.Linq;
using AstralCandle.Animation;
using UnityEngine.Events;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class GameLoop : MonoBehaviour{    
    public static GameLoop instance;
    [HideInInspector] public List<EntityStructure> playerStructures = new();
    [HideInInspector] public List<Entity> allEntities = new();
    
    #region EDITOR VARIABLES
    [SerializeField] bool showDebug;
    [SerializeField] Transform map;
    [SerializeField] Settings settings;
    [SerializeField] WaveProfile[] waves;
    [SerializeField] EntityCharacter humanEntity;
    [SerializeField] int humansToStartWith = 2;
    [SerializeField] bool startOnPlay;
    [SerializeField] UnityEvent Win, Lose;
    
    #endregion
    #region COSMETIC
    Vector3 previousScale, targetScale;
    
    #endregion
    #region PRIVATE VARIABLES
    const int NUMBER_OF_SPAWN_DIRECTIONS = 4; // UP, DOWN, LEFT, RIGHT
    readonly int[] RESOURCE_NODE_DIRECTIONS = new int[4]{0, 90, 180, 270};
    int _wave = -1;
    
    float _mapScale, previousMapScale;    

    [SerializeField] Pause pauseState;
    [HideInInspector] public WinLose state;
    #endregion
     
    public float MapScale{
        get => _mapScale;
        private set{
            if(value == _mapScale){ return; }
            _mapScale = value;
            settings.scaleAnimation.ResetTime();
            previousScale = targetScale;
            targetScale = new(value, map.localScale.y, value);
        } 
    }
    
    /// <summary>
    /// The concurrent wave we are on
    /// </summary>
    public int Wave{
        get => _wave;
        private set{
            if(value == _wave){ return; }
            _wave = value;            
        }
    }
   
    /// <summary>
    /// The current wave we are playing on
    /// </summary>
    public Game CurrentGame{
        get;
        private set;
    }
    
    //--- FUNCTIONS

    /// <summary>
    /// Scales the map in size to match the 'TargetScale' vector
    /// </summary>
    void PlayMapAnimation(){
        float value = settings.scaleAnimation.Play();
        map.localScale = Vector3.LerpUnclamped(previousScale, targetScale, value);
    }

    public List<Vector3> CalculateResourceSpawnLocations(){
        // Calculate resource spawning locations
        HashSet<Vector3> resourceSpawnLocations = new();
        int maxMapSize = (int)Grid.RoundToGrid(MapScale / 2, settings.cellSize) - settings.cellSize;
        int minMapSize = (int)Grid.RoundToGrid(previousMapScale / 2, settings.cellSize);
        for(int i = minMapSize; i < maxMapSize; i += settings.cellSize){
            for(int x = -i; x < i; x+= settings.cellSize){
                resourceSpawnLocations.Add(new(x, 0, i));
                resourceSpawnLocations.Add(new(-x, 0, -i));
            }
            for(int z = -i; z < i; z+= settings.cellSize){
                resourceSpawnLocations.Add(new(i, 0, z));
                resourceSpawnLocations.Add(new(-i, 0, z));
            }
        }
        System.Random random = new();
        return resourceSpawnLocations.OrderBy(e => random.NextDouble()).ToList(); // Shuffles list
    }

    public void SpawnEntities(ref List<Vector3> spawnPositions, params Entity[] entities){
        Transform parentFolder = GameObject.Find("_GAME_RESOURCES_").transform;
        spawnPositions ??= CalculateResourceSpawnLocations();

        foreach(Entity e in entities){
            if(spawnPositions.Count <= 0){ continue; }

            Vector3 position = spawnPositions[0];
            spawnPositions.RemoveAt(0);

            Entity newE = Instantiate(e, position, Quaternion.identity, parentFolder);
            PlayerControls.instance.entities.SubscribeToEntities(newE);
            Collider col = newE.GetComponent<Collider>();

            Vector3 newPos = position;
            newPos.y = (map.localScale.y /2) + col.bounds.extents.y;
            newE.transform.position = newPos;

            Vector3 euler = newE.transform.eulerAngles;
            euler.y = RESOURCE_NODE_DIRECTIONS[UnityEngine.Random.Range(0, RESOURCE_NODE_DIRECTIONS.Length-1)];
            newE.transform.eulerAngles = euler;
        }
    }

    public void SpawnEntities(ref List<Vector3> spawnPositions, params ResourceSettings[] settings){
        spawnPositions ??= CalculateResourceSpawnLocations();

        for(int i = 0; i < settings.Length; i++){
            ResourceSettings r = settings[i];
            int fillAmount = Mathf.FloorToInt(spawnPositions.Count * r.resourcePopulation);

            for(int x = 0; x < fillAmount && x < spawnPositions.Count; x++){
                SpawnEntities(ref spawnPositions, r.resourceNode);
            }
        }
    }

    public void NewGame(){     
        Wave++;

        MapScale = Utilities.Remap(Wave + 1, 0, waves.Length, settings.mapSize.min, settings.mapSize.max);
        CurrentGame = new Game(waves[Wave], map.position, MapScale, (Wave + 1).ToString());

        // Spawn resources
        Transform parentFolder = GameObject.Find("_GAME_RESOURCES_").transform;
        List<Vector3> rLocations = CalculateResourceSpawnLocations();
        SpawnEntities(ref rLocations, settings.resources);

        // Spawn starting humans
        if(Wave == 0){
            for(int i = 0; i < humansToStartWith; i++){
                SpawnEntities(ref rLocations, humanEntity);
            }
        }
    }

    // RUNS ALL ENTITIES
    private void FixedUpdate() {
        if(state != WinLose.In_Game || PlayerControls.instance.Paused){ return; }
        for(int i = allEntities.Count-1; i > -1; i--){ allEntities[i].Run(state); }
    }

    void Awake() => instance = this;

    /// <summary>
    /// Initiates this script
    /// </summary>
    void Start(){        
        MapScale = Utilities.Remap(Wave + 1, 0, waves.Length, settings.mapSize.min, settings.mapSize.max);
        previousMapScale = Grid.RoundToGrid((float)settings.mapSize.min * settings.resourceSpawnSubtractMin, settings.cellSize);
        if(startOnPlay){ NewGame(); }
        pauseState.Initiate();
    }
    private void Update() {
        PlayMapAnimation();
        pauseState.OnPause(PlayerControls.instance?.Paused == true);
        if(PlayerControls.instance?.Paused == true){ 
            return;
        }
        if((state == WinLose.Win || state == WinLose.Lose) && (Tutorial.instance.CompletedWave || startOnPlay)){ return; }

        Keep keepInstance = Keep.instance;
        bool keepExists = keepInstance;
        bool hasOccupants = false; // Should check all structures
        List<Entity> s = PlayerControls.instance?.entities.GetAllOfType(Keep.instance as EntityStructure);
        foreach(Entity structure in s){
            EntityStructure thisS = structure as EntityStructure;
            if(!thisS){ continue; } 

            if(thisS.GetOccupants() >0){
                hasOccupants = true;
                break;
            }
        }
        bool hasHumansOnField = PlayerControls.instance?.entities.GetAllOfType(humanEntity).Count > 0;
        
        state = (keepExists && (hasOccupants || hasHumansOnField))? WinLose.In_Game : WinLose.Lose; // If keep exists and we either have occupants OR occupants on field
        
        if(Wave >= waves.Length-1 && CurrentGame.CalculateProgress() >= 1){ 
            state = WinLose.Win; 
            Win?.Invoke();
            return;
        }
        else if(state == WinLose.Lose && (Tutorial.instance.CompletedWave || startOnPlay)){
            Lose?.Invoke();
            return;
        }


        

        // Ensures the game is always running
        if(CurrentGame != null && CurrentGame.Run() && Wave < waves.Length-1 && state == WinLose.In_Game){            
            previousMapScale = MapScale;
            NewGame();
            return;
        }
    }

    private void OnDrawGizmos() {
        #if UNITY_EDITOR
        if(!showDebug || !map){ return; }
        Handles.DrawWireCube(map.position, new Vector3(settings.mapSize.min, map.localScale.y, settings.mapSize.min));
        Handles.DrawWireCube(map.position, new Vector3(settings.mapSize.max, map.localScale.y, settings.mapSize.max));
        #endif
    }

    public enum GameState{
        Init,
        Break,
        Wave
    }

    public enum WinLose{
        In_Game,
        Win,
        Lose
    }

    /// <summary>
    /// Used to setup each wave
    /// </summary>
    public class Game{
        #region CLASS_VARIABLES
        GameState _state;
        public GameState State{
            get => _state;
            private set{
                if(value == _state){ return; }
                _state = value;
                ProgressBar.instance.UpdateState(State.ToString());
                ProgressBar.instance.UpdateColour((value == GameState.Break)? wave.intermissionColour : wave.waveColour);
                if(value == GameState.Wave){ 
                    ProgressBar.instance.UpdateValue(waveName);
                    ProgressBar.instance.UpdateTarget(0); 
                }
            }
        }

        Vector3 position;
        float scale;
        string waveName;

        float intermission;

        /// <summary>
        /// Shows how long the intermission is
        /// </summary>
        
        WaveProfile wave;

        #region ENTITY COUNTERS
        /// <summary>
        /// IDs of the characters
        /// </summary>
        public HashSet<int> aliveEntities{
            get;
            private set;
        }

        /// <summary>
        /// The entities remaining for this wave
        /// </summary>
        int totalEntities;
        
        int _entitiesCleared;
        /// <summary>
        /// The number of entities cleared in this wave. | UPDATES THE PROGRESS BAR WHEN SET
        /// </summary>
        int EntitiesCleared{
            get => _entitiesCleared;
            set{
                _entitiesCleared = value;
                ProgressBar.instance.UpdateTarget(CalculateProgress());
            }
        }
        #endregion
        
        int spawnGroupIndex;
        float elapsedSpawnDelay;
        #endregion

        /// <summary>
        /// Constructor to create a new game
        /// </summary>
        /// <param name="waveData">Information about this wave</param>
        /// <param name="position">The base position of the map</param>
        /// <param name="scale">The map scale so we can spawn the entities on the edges</param>
        public Game(WaveProfile waveData, Vector3 position, float scale, string waveName = null){
            this.wave = waveData;

            aliveEntities = new();
            this.intermission = waveData.intermission;
            this.totalEntities = waveData.CalculateEntities();
            this.spawnGroupIndex = 0;
            this.EntitiesCleared = 0;

            this.position = position;
            this.scale = scale;            
            
            waveName ??= waveData.name;
            this.waveName = waveName;
        }

        /// <summary>
        /// Returns the group data of the given index for this wave
        /// </summary>
        WaveProfile.SpawnGroupData GetWaveGroup() => wave.waveData[Mathf.Clamp(spawnGroupIndex, 0, wave.waveData.Length-1)];

        /// <summary>
        /// Calculates a position on the edge of the map to spawn an entity
        /// </summary>
        /// <returns>The position we should spawn an entity at</returns>
        Vector3 CalculatePosition(){
            int spawnDirection = (int)GetWaveGroup().spawnDirection;
            bool isHorizontal = spawnDirection == 1 || spawnDirection == 3;
            float angle = Utilities.CalculateRadianAngle(spawnDirection, NUMBER_OF_SPAWN_DIRECTIONS);

            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (scale/2);

            float rndValue = UnityEngine.Random.Range(-1f, 1f);
            float offsetf = rndValue * scale/3;
            offset.x = (isHorizontal)? offsetf: offset.x; // Left/Right
            offset.y = 1.25f;
            offset.z = (!isHorizontal)? offsetf: offset.z; // Up/Down
            return position + offset;
        }
        
        /// <summary>
        /// Spawns an entity on the edge of the map on a given direction
        /// </summary>
        /// <param name="character">The entity we wish to spawn</param>
        void SpawnEntity(EntityCharacter character){
            // Spawns the character and adds it to alive entities list
            aliveEntities.Add(
                Instantiate(
                    character, 
                    CalculatePosition(), 
                    Quaternion.identity, 
                    GameObject.Find("_GAME_RESOURCES_").transform
                ).GetInstanceID()
            );
        } 

        /// <summary>
        /// Attempts to remove the entity from the 'aliveEntities' hashSet so we can see how complete the wave is
        /// </summary>
        /// <param name="id">The transform ID of the entity</param>
        public void RemoveEntity(int id){
            if(!aliveEntities.Contains(id)){ return; }
            aliveEntities.Remove(id);
            EntitiesCleared++;
        }
    
        /// <summary>
        /// Calculates wave progression 
        /// </summary>
        /// <return>Value between 0-1</return>
        public float CalculateProgress() => Mathf.Clamp01((float)EntitiesCleared / totalEntities);

        /// <summary>
        /// Runs this wave; Spawning entities
        /// </summary>
        public bool Run(){
            if(CalculateProgress() >= 1){ return true; } // Completed wave!

            intermission -= Time.deltaTime;
            State = (intermission >0)? GameState.Break : GameState.Wave;
            
            switch(State){
                case GameState.Break:                    
                    ProgressBar.instance.UpdateValue(Mathf.Ceil(intermission).ToString());
                    ProgressBar.instance.UpdateTarget(intermission / wave.intermission);
                    break;
                case GameState.Wave:
                    // Spawning
                    WaveProfile.SpawnGroupData groupData = GetWaveGroup();
                    elapsedSpawnDelay -= (elapsedSpawnDelay >0)? Time.deltaTime : 0;
                    if(elapsedSpawnDelay <= 0 && spawnGroupIndex <= wave.waveData.Length-1){
                        for(int i = 0; i < groupData.quantity; i++){
                            foreach(IWave _char in groupData.group.entities.Cast<IWave>()){
                                if(_char == null){ continue; } // Stops any non-'IWave' entites from being spawned
                                SpawnEntity(_char as EntityCharacter);
                            }
                        }
                        elapsedSpawnDelay = groupData.spawnDelay;
                        spawnGroupIndex++;
                    }
                    break;
            }            
            return false;
        }
    }

    /// <summary>
    /// The settings which controls this game loop
    /// </summary>
    [Serializable] public class Settings{
        public AnimationInterpolation scaleAnimation;
        public Utilities.MinMax mapSize = new(15, 100);
        [Tooltip("The distance up to the min map scale where we will start to spawn resources"), Range(0, 1)]public float resourceSpawnSubtractMin;
        public ResourceSettings[] resources;
        public int cellSize = 1;
    }

    [Serializable] public class ResourceSettings{
        [Tooltip("The resource we want to spawn")] public EntityResourceNode resourceNode;
        [Tooltip("The % quantity of this resource which will spawn"), Range(0, 1)] public float resourcePopulation = 0.05f;
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
    [Serializable] public class Pause{
        [Header("SFX Settings")]
        public AudioSource source;
        [Range(0, 1)] public float onPauseVolume = 0.05f;
        [Range(0, 1)] public float onPausePitch = 0.65f;

        public AnimationInterpolation transitionSettings;
        [HideInInspector] public float defaultVolume;
        [HideInInspector] public float defaultPitch;

        public void Initiate(){
            defaultVolume = source.volume;
            defaultPitch = source.pitch;
        }        
        public void OnPause(bool isPaused){
            float value = transitionSettings.Play(!isPaused);

            source.volume = Mathf.LerpUnclamped(defaultVolume, onPauseVolume, value);
            source.pitch = Mathf.LerpUnclamped(defaultPitch, onPausePitch, value);
        }
    }
    [Serializable] public class AudioSettings{
            public AudioClip[] type;
            public Utilities.MinMaxF cooldown;
            public Utilities.MinMaxF pitch = new Utilities.MinMaxF(1, 1);
            public float ActualCooldown{ get; set; }

            public void PlaySound(AudioSource s){
                if(ActualCooldown > 0 || type.Length <= 0 || !s) { return; }
                ActualCooldown = UnityEngine.Random.Range(cooldown.min, cooldown.max);
                s.clip = type[UnityEngine.Random.Range(0, type.Length)];
                s.pitch = UnityEngine.Random.Range(pitch.min, pitch.max);
                s.Play();
            }

            
            public void PlaySoundWithIncrease(AudioSource s, float value, float step = -1){
                if(ActualCooldown > 0 || type.Length <= 0 || !s) { return; }
                ActualCooldown = UnityEngine.Random.Range(cooldown.min, cooldown.max);
                s.clip = type[UnityEngine.Random.Range(0, type.Length)];
                
                float val = Utilities.Remap(Mathf.Clamp01(value), 0, 1, pitch.min, pitch.max);
                s.pitch = (step > 0)? (float)Math.Round(val / step, 1) * step : val;
                s.Play();
            }
        }
}
