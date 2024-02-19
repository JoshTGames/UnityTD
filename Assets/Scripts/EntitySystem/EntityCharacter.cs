using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    public abstract class EntityCharacter : EntityHealth, IMove{
        [Header("Character Settings")]
        [SerializeField, Tooltip("The speed this entity will move at")] protected float moveSpeed = 5f;
        [SerializeField, Tooltip("The distance between entities till the entity can interact")] float interactRadius = 2f;
        [SerializeField, Tooltip("How far will this entity lean when moving")] float moveLeanAmount = 2f;
        [SerializeField, Tooltip("How responsive this entity is to leaning")] float rotationSmoothing = 0.5f;

        public EntityCharacter(int ownerId) : base(ownerId){}

        public void Move(Vector3 position, Func<EntityERR> action = null)
        {
            transform.position = position;
        }
        
        protected override void Run(){}

        protected override bool OnDrawGizmos(){
            #if UNITY_EDITOR
            if(!base.OnDrawGizmos()){ return false; }

            Handles.DrawWireDisc(transform.position, Vector3.up, interactRadius);

            return true;
            #endif
        }
    }
}