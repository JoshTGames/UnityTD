using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/
namespace AstralCandle.TowerDefence{
    public class EntityTooltip : MonoBehaviour{
        public static EntityTooltip instance;
        [SerializeField] RectTransform tooltipObject;
        [SerializeField] TextMeshProUGUI headerUI, contentsUI;
        [SerializeField] LayoutElement layout;
        [SerializeField, Tooltip("The limit until we start a new line")] int charLimit = 100;
        Camera cam;

        Tooltip _tooltip;

        /// <summary>
        /// Creates a new tooltip and applies the text onto the UI
        /// </summary>
        public Tooltip tooltip{
            get => _tooltip;
            set{
                _tooltip = value;
                // Guard clause - Stops further processing if tool tip doesnt exist
                if(value == null){
                    tooltipObject.gameObject.SetActive(false);
                    return;
                }

                int headerLength = value.header.Length;
                int contentsLength = value.contents.Length;
                bool exceedsCharLength = (headerLength > charLimit || contentsLength > charLimit);

                // Applies text
                headerUI.text = value.header;
                contentsUI.text = value.contents;


                // Enables multiple lines - If either text exceeds the limit
                layout.enabled = exceedsCharLength;
                // Enables the UI object
                tooltipObject.gameObject.SetActive(headerLength > 0);
            }
        }

        void Start() => instance = this;
        void Awake() {
            cam = Camera.main;
        }

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
            Cursor.visible = tooltip == null;
            if(tooltip == null){ return; }

            Vector2 mousePos = ClampToScreen(PlayerControls.instance.cursorPosition);
            

            float pivotX = Mathf.Clamp01(mousePos.x / Screen.width);
            float pivotY = Mathf.Clamp01(mousePos.y / Screen.height);
            tooltipObject.pivot = new Vector2(pivotX, pivotY);
            tooltipObject.transform.position = mousePos;
        }

        [System.Serializable] public class Tooltip{
            public string header{
                get;
                private set;
            }

            public string contents{
                get;
                private set;
            }

            public Tooltip(string header, string contents){
                this.header = header;
                this.contents = contents;
            }
        }
    }
}