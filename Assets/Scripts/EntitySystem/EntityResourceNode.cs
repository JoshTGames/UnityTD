using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    public class EntityResourceNode : EntityHealth{
        protected EntityResourceNode(int ownerId) : base(ownerId){}

        protected override void OnImmortalHit(){}
    }
}