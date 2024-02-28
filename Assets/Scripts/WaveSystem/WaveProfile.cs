using UnityEngine;
using System;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "new WaveProfile", menuName = "ScriptableObjects/WaveSystem/New Wave")]
public class WaveProfile : ScriptableObject{
    [Tooltip("Time before wave starts")] public float intermission = 10;
    [Tooltip("Bar colour on intermission")] public Color intermissionColour = Color.blue;
    [Tooltip("Bar colour on wave")] public Color waveColour = Color.green;
    [Tooltip("Group data for spawning")] public SpawnGroupData[] waveData;

    /// <summary>
    /// Calculates the number of entities inside this wave
    /// </summary>
    public int CalculateEntities(){
        int entities = 0;
        foreach(SpawnGroupData groupData in waveData){ entities += groupData.quantityOfEntities(); }
        return entities;
    }


    [Serializable] public class SpawnGroupData{
        [Tooltip("The entity we wish to spawn")] public WaveGroup group;
        [Tooltip("The quantity of entities to spawn")] public int quantity;
        [Tooltip("The delay until we spawn more entities")] public float spawnDelay;
        [Tooltip("The direction to spawn this WaveGroup")] public SpawnDirection spawnDirection;   

        /// <summary>
        /// Returns the number of entities in this group
        /// </summary>        
        public int quantityOfEntities() => group.entities.Length * quantity; 
    }

    [Serializable] public enum SpawnDirection{
        RIGHT,
        UP,
        LEFT,
        DOWN
    }
}
