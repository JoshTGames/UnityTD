using System.Collections;
using UnityEngine;
using System;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "new WaveProfile", menuName = "ScriptableObjects/WaveSystem/New Wave")]
public class WaveProfile : ScriptableObject{
    [Tooltip("Time before wave starts")] public float intermission = 10;
    [Tooltip("Group data for spawning")] public SpawnGroupData[] waveData;


    [Serializable] public class SpawnGroupData{
        [Tooltip("The entity we wish to spawn")] public WaveGroup group;
        [Tooltip("The quantity of entities to spawn")] public int quantity;
        [Tooltip("The delay until we spawn more entities")] public float spawnDelay;
        [Tooltip("The direction to spawn this WaveGroup")] public SpawnDirection spawnDirection;    
    }

    [Serializable] public enum SpawnDirection{
        UP,
        RIGHT,
        DOWN,
        LEFT
    }
}
