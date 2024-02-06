/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.TowerDefence.AI{
    public class BT_Sequence : BT_Node{
        BT_Node[] nodes;

        /// <summary>
        /// Iterates through each node in a sequence. If the node fails, the whole sequence fails. If successful, move onto next node
        /// </summary>
        /// <param name="nodes">An array of nodes to be queried</param>
        public BT_Sequence(BT_Node[] nodes) => this.nodes = nodes;

        /// <summary>
        /// Iterates through each node in a sequence. If the node fails, the whole sequence fails. If successful, move onto next node
        /// </summary>
        /// <returns>The node state of this sequence</returns>
        public override NodeState Evaluate(){
            bool isAnyNodeRunning = false;
            foreach(BT_Node node in nodes){
                switch(node.Evaluate()){
                    case NodeState.Running:
                        isAnyNodeRunning = true;
                        break;
                    case NodeState.Success:
                        break;
                    case NodeState.Fail:
                        return NodeState.Fail;
                }
            }

            return (isAnyNodeRunning)? NodeState.Running : NodeState.Success;
        }
    }
}