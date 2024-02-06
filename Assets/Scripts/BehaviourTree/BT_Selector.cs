/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence.AI{
    public class BT_Selector : BT_Node{
        BT_Node[] nodes;

        /// <summary>
        /// Useful for root nodes (Iterates through each node in a selector. If the node fails, the next node will be queried. Otherwise if running or successful. The selector is successful)
        /// </summary>
        /// <param name="nodes">An array of nodes to be queried</param>
        public BT_Selector(BT_Node[] nodes) => this.nodes = nodes;

        /// <summary>
        /// Iterates through each node in a selector. If the node fails, the next node will be queried. Otherwise if running or successful. The selector is successful
        /// </summary>
        /// <returns>The node state of this selector</returns>
        public override NodeState Evaluate(){
            foreach(BT_Node node in nodes){
                switch(node.Evaluate()){
                    case NodeState.Running:
                        return NodeState.Running;
                    case NodeState.Success:
                        return NodeState.Success;
                    case NodeState.Fail:
                        break;
                }
            }
            return NodeState.Fail;
        }
    }
}