using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class PlayParticleFXOnEvent : MonoBehaviour{
   [SerializeField] ParticleSystem pS;
   public void Play() => pS.Play();
}
