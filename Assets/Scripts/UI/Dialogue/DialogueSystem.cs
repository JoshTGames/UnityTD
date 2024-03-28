using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using AstralCandle.Animation;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class DialogueSystem : MonoBehaviour{
    public static DialogueSystem instance;
    [SerializeField] Settings settings;
    [SerializeField] TTA textToAudio;
    [SerializeField] AnimationInterpolation shiftingSettings;
    [SerializeField] float shiftingOffset;
    float defaultShiftX;
    Transform ui;

    Queue<DialogueMain> dialogueQueue;
    DialogueMain currentDialogue;
    public DialogueMain CurrentDialogue{ 
        get => currentDialogue;
        private set{
            if(currentDialogue == value){ return; }
            currentDialogue = value;
            
            if(value == null){ 
                settings.leftActor.gameObject.SetActive(false);
                settings.rightActor.gameObject.SetActive(false);
                return; 
            }
            value.Dialogue.PreFunction?.Run();
            settings.header.color = value.Dialogue.Persona.Colour;
            settings.header.text = value.Dialogue.Persona.name;
            
            settings.leftActor.gameObject.SetActive(false);
            settings.rightActor.gameObject.SetActive(false);
            if(value.Dialogue.ShowArt){
                DialogueMain.Direction direction = value.Position;
                Image actor = settings.leftActor;
                if(direction == DialogueMain.Direction.Left){
                    settings.leftActor.gameObject.SetActive(true);
                }
                else{
                    settings.rightActor.gameObject.SetActive(true);
                    actor = settings.rightActor;
                }

                actor.sprite = value.Dialogue.Persona.Art;
            }
        }
    }

    /// <summary>
    /// Appends dialogue onto the queue
    /// </summary>
    /// <param name="dialogue">The dialogue we want to load</param>
    public void LoadDialogue(params DialogueMain[] dialogue){
        foreach(DialogueMain d in dialogue){ 
            dialogueQueue.Enqueue(d);
        }
    }

    DialogueMain GetNewDialogue(){
        bool hasComplete = CurrentDialogue != null && CurrentDialogue.IsComplete();
        
        return CurrentDialogue = (dialogueQueue.Count > 0 && (CurrentDialogue == null || hasComplete))? dialogueQueue.Dequeue() : CurrentDialogue;
    }
    void PlayDialogue(){
        if(GetNewDialogue() == null || CurrentDialogue.Play(out string text)){ 
            CurrentDialogue = null;
            return; 
        }

        if(!settings.label.text.Equals(text) && text.Length >0){
            // Play audio
            textToAudio.Play(text[^1], CurrentDialogue.Dialogue.Persona.Voice); // Refers to the last element in the array
        }

        // If the dialogue gets too big...
        settings.layoutElement.enabled = text.Length >= settings.maxCharactersPerLine;

        // Enable text to tell the user to move onto next dialogue
        settings.nextLineText.SetActive(CurrentDialogue.Dialogue.Text.Equals(text) && !currentDialogue.Dialogue.Check);

        Color skippableColour = settings.isSkippableBar.color;
        skippableColour.a = (!CurrentDialogue.Dialogue.Check)? 255 : 0;
        settings.isSkippableBar.color = skippableColour;

        settings.label.text = text;
    }

    private void Start() {
        instance = this;
        textToAudio.InitialiseTTA();
        settings.Initialise();
        dialogueQueue = new();
        ui = transform.GetChild(2);
        defaultShiftX = ui.localPosition.x;
        shiftingSettings.ResetTime();
    }
    private void LateUpdate(){
        settings.DoShow(ui, CurrentDialogue != null);
        PlayDialogue();

        float value = shiftingSettings.Play(!BuildUI.instance.isOpen);
        Vector3 defaultPos = new(defaultShiftX, ui.localPosition.y);
        Vector3 shiftPos = new(defaultShiftX + shiftingOffset, ui.localPosition.y);
        ui.localPosition = Vector3.LerpUnclamped(defaultPos, shiftPos, value);
    }


    // ---
    // CLASSES
    // ---
    [Serializable] public class TTA{
        [SerializeField] AudioSource source;
        [SerializeField] TTAKeyValue[] TTAKeyValues;

        Dictionary<char, AudioClip> characters;

        /// <summary>
        /// Will create a look-up dictionary to use and play audio from
        /// </summary>
        public void InitialiseTTA(){
            characters = new();
            foreach(TTAKeyValue _tta in TTAKeyValues){ 
                characters.Add(_tta.character, _tta.clip);
            }
        }

        /// <summary>
        /// Uses our lookup data to find the audio clip and play it
        /// </summary>
        /// <param name="character"></param>
        /// <param name="pitch"></param>
        public void Play(char character, float pitch = 1){
            if(!source || !characters.ContainsKey(character)){ return; }
            source.clip = characters[character];
            source.pitch = pitch;
            source.Play();
        }

        [Serializable] public class TTAKeyValue{
            [Tooltip("This letter, if found in a string will play the audio clip")] public char character;
            [Tooltip("The audio that will be played if the character is found")] public AudioClip clip;
        }
    }
    [Serializable] public class Settings{
        [Header("Text settings")]
        public TMP_Text header;
        public TMP_Text label;
        public GameObject nextLineText;
        public Image isSkippableBar;
        public Image leftActor, rightActor;
        public LayoutElement layoutElement;
        public int maxCharactersPerLine = 200;

        [SerializeField] AnimationInterpolation animationInterpolation;

        public void Initialise() => animationInterpolation.ResetTime();
        public void DoShow(Transform transform, bool doShow = true){
            float value = animationInterpolation.Play(!doShow);

            transform.gameObject.SetActive(animationInterpolation.percent != 0);
            transform.localScale = Vector3.one * value;
        }
    }
}
