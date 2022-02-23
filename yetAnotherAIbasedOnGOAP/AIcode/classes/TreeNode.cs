//class for tree node used by tree builder

using System.Collections.Generic;

namespace AIcode.classes
{
    class TreeNode
    {
        #region cunstructors
        public TreeNode(ActorAction nodeAction, TreeNode parrent, List<Condition> currentConditions, Goal goal)
        {
            this.nodeAction = nodeAction;

            this.parrent = parrent;

            if (parrent != null) pathLength = parrent.pathLength + 1;
            else pathLength = 0;

            if (nodeAction != null) nodeConditions = Helper.MergeConditions(currentConditions, nodeAction.PostConditions);
            else nodeConditions = currentConditions;

            nodeValue = Helper.nodeValueCalc(nodeConditions, goal);

            nodeReachedGoal = goal.isReached(nodeConditions);
        }
        #endregion

        #region variables
        /// <summary>
        /// action that leads to this node
        /// </summary>
        public ActorAction nodeAction { get; }
        /// <summary>
        /// link to node's parrent node
        /// </summary>
        public TreeNode parrent { get; }
        /// <summary>
        /// conditions of this node (start conditions merged with node's action's postconditions)
        /// </summary>
        public List<Condition> nodeConditions { get; }
        /// <summary>
        /// how far this node from first node in tree
        /// </summary>
        public int pathLength { get; }
        /// <summary>
        /// more conditions of goal this node met -> bigger value this node have
        /// </summary>
        public int nodeValue { get; }
        /// <summary>
        /// is this node reaches goal
        /// </summary>
        public bool nodeReachedGoal { get; }
        #endregion
    }
}
