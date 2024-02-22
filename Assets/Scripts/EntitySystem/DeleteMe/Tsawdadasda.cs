using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AstralCandle.Entity;
using System;
public class Tsawdadasda : EntityCharacter{
    [SerializeField, Tooltip("Dictates how far this character leans when moving")] float leanFactor = 10f;
    [SerializeField] float smoothing = 0.1f;
    Animator animator;

    Quaternion prvLookDirection;
    Vector3 lookDirection, lookVelocity;


    public Tsawdadasda(int ownerId) : base(ownerId){}

    #region HIDE
    protected override void OnDamage(){}
    protected override void OnDeath(){}
    protected override void OnHeal(){}
    protected override void OnImmortalHit(){}
    #endregion

    void ManageMovement() => animator.Play((entityTask != null)? "Walk" : "Idle");

    void ManageLean(){
        lookDirection = Vector3.SmoothDamp(lookDirection, (entityTask != null)? entityTask.moveDirection : Vector3.zero, ref lookVelocity, smoothing);

        Vector3 localMove = transform.InverseTransformDirection(lookDirection);
        Vector3 lean = new Vector3(localMove.z * leanFactor, 0, -localMove.x * leanFactor);

        Quaternion lookRot = (lookDirection != Vector3.zero)? Quaternion.LookRotation(lookDirection) : prvLookDirection;
        prvLookDirection = lookRot;
        transform.rotation = lookRot * Quaternion.Euler(lean);
    }

    private void LateUpdate() {
        ManageMovement();        
        ManageLean();
    }

    protected override void Start(){
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }
}
