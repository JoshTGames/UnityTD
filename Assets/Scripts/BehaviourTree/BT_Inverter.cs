/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence.AI{
    public class BT_Inverter : BT_Node{
        BT_Node node;

        /// <summary>
        /// Constructs a node which inverts the result of the parsed node
        /// </summary>
        /// <param name="node">The node we want to invert</param>
        BT_Inverter(BT_Node node) => this.node = node;

        /// <summary>
        /// Inverts the result of the associated node
        /// </summary>
        /// <returns>The inverted result of the node should it fail/succeed</returns>
        public override NodeState Evaluate(){
            switch(node.Evaluate()){
                case NodeState.Running:
                    return NodeState.Running;
                case NodeState.Success:
                    return NodeState.Fail;
                case NodeState.Fail:
                    return NodeState.Success;
            }
            return NodeState.Fail;
        }
    }
}