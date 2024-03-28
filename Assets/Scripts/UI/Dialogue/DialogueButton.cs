using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class DialogueButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler{

    List<Colours> colours;
    [SerializeField] Color selectionColour = Color.white, clickColour = Color.grey;

    public void OnPointerClick(PointerEventData eventData) => DialogueSystem.instance.CurrentDialogue?.CompleteDialogue();

    public void OnPointerDown(PointerEventData eventData) => SetColour(clickColour);

    public void OnPointerUp(PointerEventData eventData) => SetColour();

    public void OnPointerEnter(PointerEventData eventData){
        PlayerControls.instance.canZoom = false;
        SetColour(selectionColour);
    }

    public void OnPointerExit(PointerEventData eventData){
        PlayerControls.instance.canZoom = true;
        SetColour();
    }

    void SetColour(Color? colour = null){
        foreach(Colours c in colours){ 
            c.SetColour((colour != null)? (Color)colour : null); 
        }
    }

    private void Start() {
        Image[] images = GetComponentsInChildren<Image>();

        colours = new();
        foreach(Image image in images){ colours.Add(new Colours(image)); }
    }

    public class Colours{
        Image image;
        Color defaultColour;
        public Colours(Image image){ 
            this.image = image;
            this.defaultColour = image.color;
        }

        public void SetColour(Color? colour = null){
            Color c = defaultColour;
            if(colour != null){ c = defaultColour * (Color)colour; }

            if(image){ image.color = c; }
        }
    }
}
