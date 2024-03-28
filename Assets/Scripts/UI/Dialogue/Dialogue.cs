using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[CreateAssetMenu(fileName = "New dialogue", menuName = "ScriptableObjects/DialogueSystem/New dialogue")]
public class Dialogue : ScriptableObject{
    [SerializeField] DialoguePersona persona;
    [SerializeField] DialogueFunctionCheck check;
    [SerializeField] DialogueAction preFunction;
    [SerializeField] DialogueAction postFunction;
    [SerializeField] bool showArt;
    [SerializeField, TextArea] string text;
    public DialoguePersona Persona{ get => persona; }
    public DialogueFunctionCheck Check{ get => check; }
    public DialogueAction PreFunction{ get => preFunction; }
    public DialogueAction PostFunction{ get => postFunction; }
    public bool ShowArt{ get => showArt; }
    public string Text{ get => text; }
}
