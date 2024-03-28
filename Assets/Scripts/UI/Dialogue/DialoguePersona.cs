using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "New persona", menuName = "ScriptableObjects/DialogueSystem/New persona")]
public class DialoguePersona : ScriptableObject{
    [SerializeField] Sprite art;
    [SerializeField, ColorUsage(showAlpha: false)] Color colour;
    [SerializeField, Tooltip("PITCH, higher value = higher pitch")] float voice = 1;
    [SerializeField, Tooltip("Higher the value, faster the text will appear on the screen")] float talkingSpeed = 3;

    public Sprite Art{ get => art; }
    public Color Colour{ get => colour; }
    public float Voice{ get => voice; }
    public float TalkingSpeed{ get => talkingSpeed; }
}
