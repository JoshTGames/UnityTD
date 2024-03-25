using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    public class EntityTooltip : MonoBehaviour{
        public static EntityTooltip instance;
        [SerializeField] RectTransform tooltipObject;
        public RectTransform TooltipObject{ get => tooltipObject; }
        [SerializeField] TextMeshProUGUI headerUI, descriptionUI;
        [SerializeField] Transform attributeUI;
        [SerializeField] Image attributeIconUIPrefab;
        [SerializeField] GameObject contentsUIPrefab;
        public GameObject ContentsUIPrefab{ get => contentsUIPrefab; }
        [SerializeField] LayoutElement layout;
        [SerializeField, Tooltip("The limit until we start a new line")] int charLimit = 100;

        [Header("Game Settings")]
        public TooltipData healthData;
        public TooltipData occupantData;
        Camera cam;


        public Dictionary<string, Contents> contents;

        Tooltip _tooltip;

        /// <summary>
        /// Creates a new tooltip and applies the text onto the UI
        /// </summary>
        public Tooltip tooltip{
            get => _tooltip;
            set{
                if(_tooltip?.GetHashCode() == value?.GetHashCode()){ return; } // Stops applying itself

                // Attribute manager
                foreach(Transform attribute in attributeUI){ Destroy(attribute.gameObject); }
                //

                // Contents manager
                if(contents == null){ contents = new(); }
                else{
                    foreach(KeyValuePair<string, Contents> content in contents){
                        Destroy(content.Value.spawnedObject.gameObject);
                    }
                    contents.Clear();
                }
                //

                _tooltip = value;

                // Guard clause - Stops further processing if tool tip doesnt exist
                if(value == null){
                    tooltipObject.gameObject.SetActive(false);
                    return;
                }

                int headerLength = value.header.Length;
                int descriptionLength = value.description.Length;
                bool exceedsCharLength = (headerLength > charLimit || descriptionLength > charLimit);
                // Applies text
                headerUI.text = value.header;
                descriptionUI.text = value.description;

                // Enables multiple lines - If either text exceeds the limit
                layout.enabled = exceedsCharLength;

                value.AddContents(contentsUIPrefab, tooltipObject, ref contents, value.contents);
                foreach(Sprite sprite in value.attributes){
                    Image image = Instantiate(attributeIconUIPrefab, attributeUI);
                    image.sprite = sprite;
                }
                
                // Enables the UI object
                tooltipObject.gameObject.SetActive(headerLength > 0);
            }
        }

        void Start() => instance = this;
        void Awake() => cam = Camera.main;

        /// <summary>
        /// Clamps the screen position within the bounds of the visible screen
        /// </summary>
        /// <param name="screenPos">The screen position</param>
        /// <returns>A screen position within the bounds of the visible screen</returns>
        Vector2 ClampToScreen(Vector2 screenPos){
            screenPos = cam.ScreenToViewportPoint(screenPos);
            screenPos = new Vector2(Mathf.Clamp01(screenPos.x), Mathf.Clamp01(screenPos.y));
            return cam.ViewportToScreenPoint(screenPos);
        }

        /// <summary>
        /// Positioning
        /// </summary>
        private void LateUpdate() {
            if(PlayerControls.instance.IsPivoting){                
                tooltip = null;
                return;
            }
            
            Cursor.visible = tooltip == null;
            if(tooltip == null){ return; }

            Vector2 mousePos = ClampToScreen(PlayerControls.instance.cursorPosition);
            

            float pivotX = Mathf.Clamp01(mousePos.x / Screen.width);
            float pivotY = Mathf.Clamp01(mousePos.y / Screen.height);
            tooltipObject.pivot = new Vector2(pivotX, pivotY);
            tooltipObject.transform.position = mousePos;
        }
        
        public class Contents{
            public string id{
                get;
                private set;
            }

            public Transform spawnedObject{
                get;
                private set;
            }

            Image slider;
            TMP_Text label;

            Color sliderColour;
            Sprite icon;
            string defaultText;
            
            public Contents(string id, Color sliderColour, Sprite icon, string text = ""){
                this.id = id;
                this.sliderColour = sliderColour;
                this.icon = icon;
                this.defaultText = text;
            }

            public void CreateContents(GameObject contentsUI, Transform parent){
                spawnedObject = Instantiate(contentsUI, parent).transform;
                Transform transform = spawnedObject.transform;
                slider = transform.GetChild(1).GetComponent<Image>();  
                slider.color = sliderColour;              

                Transform info = transform.GetChild(2);
                info.GetChild(0).GetComponent<Image>().sprite = icon;
                label = info.GetChild(1).GetComponent<TMP_Text>();

                label.text = defaultText;
            }

            /// <summary>
            /// Sets the UI to display the correct 
            /// </summary>
            public void SetPercent(float value) => slider.fillAmount = Mathf.Clamp01(value);
            /// <summary>
            /// Sets the UI text
            /// </summary>
            public void SetText(string text) => label.text = text;
        }

        public class Tooltip{
            public string header{
                get;
                private set;
            }

            public List<Sprite> attributes{
                get;
                private set;
            }

            public string description{
                get;
                private set;
            }

            public Contents[] contents{
                get;
                private set;
            }

            public Tooltip(string header, string description, Sprite[] attributes = null, params Contents[] contents){
                this.header = header;
                this.attributes = (attributes != null)? attributes.ToList() : new();
                this.description = description;
                this.contents = contents;
            }

            public void AddContents(GameObject contentsUIPrefab, Transform tooltipObject, ref Dictionary<string, Contents> contents, params Contents[] contentData){
                foreach(Contents content in contentData){
                    content.CreateContents(contentsUIPrefab, tooltipObject);
                    contents.Add(content.id, content); // Add object to contents dictionary
                }
            }

            public void AddAttribute(params Sprite[] sprites){
                foreach(Sprite sprite in sprites){ attributes.Add(sprite); }
            }
        }
        [System.Serializable] public class TooltipData{
            [ColorUsage(showAlpha: false)] public Color colour = Color.red;
            public Sprite icon;
        }
    }
}