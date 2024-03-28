using UnityEngine;
using System;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

[Serializable] public class DialogueMain{
    [SerializeField] protected Dialogue dialogue;
    [SerializeField] protected Direction position;
    
    public Dialogue Dialogue{ get => dialogue; }
    public Direction Position{ get => position; }

    bool complete = false;
    bool fullyComplete = false;
    float elapsedTime; // Despite being a timer, it is also used to index the text
    
    /// <summary>
    /// Queries this dialogue to see if it is complete
    /// </summary>
    public bool IsComplete(){
        if(fullyComplete){ return true; }
        DialogueFunctionCheck.FunctionState state = (dialogue.Check)? dialogue.Check.DependencyCheck() : DialogueFunctionCheck.FunctionState.Not_Implemented;
        bool isComplete = (complete && state == DialogueFunctionCheck.FunctionState.Not_Implemented) || state == DialogueFunctionCheck.FunctionState.Success;
        if(isComplete){ 
            Dialogue.PostFunction?.Run(); 
            fullyComplete = true;
        }
        return isComplete;
    }

    /// <summary>
    /// Used to skip the text transition and move onto next dialogue
    /// </summary>
    public void CompleteDialogue(){
        complete = elapsedTime >= dialogue.Text.Length;
        elapsedTime = dialogue.Text.Length;
    }

    /// <summary>
    /// Plays this dialogue
    /// </summary>
    /// <param name="text">text to be applied onto a label</param>
    /// <returns>true if dialogue has successfully completed</returns>
    public bool Play(out string text){
        text = dialogue.Text[..Mathf.Clamp(Mathf.FloorToInt(elapsedTime), 0, dialogue.Text.Length)]; // Substrings the text to match the time
        if(IsComplete()){ return true; }

        elapsedTime += Time.deltaTime * dialogue.Persona.TalkingSpeed;
        return false;
    }

    
    [Serializable] public enum Direction{
        Right,
        Left
    }
}
