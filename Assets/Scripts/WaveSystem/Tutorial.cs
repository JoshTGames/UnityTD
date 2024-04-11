using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class Tutorial : MonoBehaviour{
    public static Tutorial instance;
    [SerializeField] DialogueMain[] dialogue;

    [HideInInspector] public List<Vector3> spawnPositions;
    public WaveProfile tutorialWave;
    public GameLoop.Game TutorialGame{ get; private set; }

    bool playTutorialWave = false; 
    public void PlayWave() => playTutorialWave = true;
    public bool CompletedWave{ get; private set; }

    bool ran;
    private void Update() {
        if(PlayerControls.instance.Paused){ return; }
        if(playTutorialWave && TutorialGame.Run()){ CompletedWave = true; }

        if(!DialogueSystem.instance || !GameLoop.instance || GameLoop.instance.Wave >=0 || ran){ return; }
        ran = true;
        DialogueSystem.instance.LoadDialogue(dialogue);
        spawnPositions = GameLoop.instance.CalculateResourceSpawnLocations();
        TutorialGame = new GameLoop.Game(tutorialWave, PlayerControls.instance.Map.transform.position, GameLoop.instance.MapScale, tutorialWave.name);
    }

    private void Start(){
        instance = this;
        TutorialGame = null;
    }
}
