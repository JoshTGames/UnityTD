using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence{
    public class Item{
        // The physical object this class is associated to 
        public Transform obj{
            get;
            private set;
        }

        /// <summary>
        /// The max amount of items that can fit in this object
        /// </summary>
        protected int maxDespositQuantity{
            get;
            private set;
        }

        /// <summary>
        /// The concurrent amount of items associated to this object 
        /// </summary>
        public int depositQuantity{
            get;
            private set;
        }

        /// <summary>
        /// Constructs this item
        /// </summary>
        /// <param name="obj">The transform associated to this item</param>
        /// <param name="depositQuantity">The deposit total of this item</param>
        public Item(Transform obj, int maxDespositQuantity, int depositQuantity = 1){
            this.obj = obj;
            this.maxDespositQuantity = maxDespositQuantity;
            this.depositQuantity = depositQuantity;
        }
        
        /// <summary>
        /// Gets the remaining available space in this item
        /// </summary>
        public int REMAINING_SPACE{
            get => maxDespositQuantity - depositQuantity;
        }

        /// <summary>
        /// Adds a parsed amount onto the deposit bank
        /// </summary>
        /// <param name="amount">The amount we want to parse</param>
        /// <returns>True/False based on success & modifies the 'amount' with any excess quantity</returns>
        public bool Deposit(ref int amount){
            if(amount != Mathf.Abs(amount)){ 
                Debug.LogWarning($"Negative values parsed on {obj.transform} when trying to Desposit");
                return false; 
            } 
            int newDeposit = Mathf.Clamp(depositQuantity + amount, 0, maxDespositQuantity); // Clamps deposit between values - Precalculates it
            amount = Mathf.Clamp(amount - REMAINING_SPACE, 0, amount); // Positive value means there is excess

            depositQuantity = newDeposit;
            return true;
        }

        /// <summary>
        /// Removes the amount parsed from this item
        /// </summary>
        /// <param name="amount">The quantity desired to be withdrawn from this item</param>
        /// <returns>True/False based on success & updates the 'depositQuantity' accordingly</returns>
        public bool Withdraw(ref int amount){
            if(amount != Mathf.Abs(amount)){ 
                Debug.LogWarning($"Negative values parsed on {obj.transform} when trying to Withdraw");
                return false; 
            } 
            int toBeWithdrawn = Mathf.Clamp(amount, 0, depositQuantity);
            depositQuantity -= toBeWithdrawn;
            amount = toBeWithdrawn;
            return true;
        }
    
        public enum ItemType{
            WOOD,
            STONE,
            METAL,
            GOLD
        }
    }
}