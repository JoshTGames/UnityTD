using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    /// <summary>
    /// The base entity class which provides basic functionality; allowing for 'OnHover' & 'OnSelect' interactivity
    /// </summary>
    public abstract class BaseEntity : MonoBehaviour, ISelectable{
        [SerializeField, Tooltip("Identifies who owns this entity. -1 means it is not owned. By default '0' is the player")] int _ownerId = -1;
        /// <summary>
        /// Identifies who owns this entity. -1 means it is not owned. By default '0' is the player
        /// </summary>
        public int owner{ get => _ownerId; }

        /// <summary>
        /// The behaviour that should occur when mouse is hovering over this entity
        /// </summary>
        /// <param name="hovered">Are we hovering over this entity?</param>
        public abstract void OnHover(bool hovered);
        /// <summary>
        /// The behaviour that should occur when this entity is selected
        /// </summary>
        /// <param name="selected">Is this entity selected?</param>
        public abstract void OnSelect(bool selected);
        
        /// <summary>
        /// Adds this entity into the entities data set
        /// </summary>
        protected virtual void Start() => PlayerControls.instance.entities.SubscribeToEntities(this);
        /// <summary>
        /// Removes this entity from the entities data set
        /// </summary>
        protected virtual void OnDestroy() => PlayerControls.instance.entities.SubscribeToEntities(this, true);

    }
}