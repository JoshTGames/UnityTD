/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence.AI{
    /// <summary>
    /// Creates a new decision node
    /// </summary>
    public abstract class BT_Node{   
             
        /// <summary>
        /// The concurrent state of this decision node.
        /// </summary>
        public NodeState state{ 
            get; 
            protected set;
        }        
        
        /// <summary>
        ///  This is where the logic goes to dictate the result of this decision node
        /// </summary>
        /// <returns>The result of this decision node</returns>
        public abstract NodeState Evaluate();

        /// <summary>
        /// All the possible states a decision node could be
        /// </summary>
        public enum NodeState{
            Fail,
            Success,
            Running
        }
    }
}