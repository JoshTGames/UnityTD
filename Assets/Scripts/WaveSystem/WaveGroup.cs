using AstralCandle.Entity;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "new WaveGroup", menuName = "ScriptableObjects/WaveSystem/New WaveGroup")]
public class WaveGroup : ScriptableObject{
    public EntityCharacter[] entities;    
}
