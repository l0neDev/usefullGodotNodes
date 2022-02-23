//class to generate actions tree from given start conditions to goal

using System.Collections.Generic;
using System.Linq;

using AIcode.classes;

namespace AIcode.funcs
{
    static class TreeBuilder
    {
        #region public funcs
        /// <summary>
        /// build a tree from [startConditions] to [goal]
        /// </summary>
        /// <param name="goal">goal for this tree</param>
        /// <param name="posibleActions">all posible actions</param>
        /// <param name="startConditions">conditions to start with</param>
        /// <param name="goalReached">is goal was reached</param>
        /// <returns>list of explored nodes if there was some or null if not, and [goalReached] = true if path found or false if not</returns>
        public static List<TreeNode> GetTree(Goal goal, List<ActorAction> posibleActions, List<Condition> startConditions, bool debug = false)
        {
            if (debug) Helper.DebugLog("starting to build tree of actions:");

            if (goal == null || posibleActions == null || posibleActions.Count == 0)
            {
                if (debug) Helper.DebugLog("one or more args are invalid, returning null.");
                return null;
            }

            var openList = new List<TreeNode>(); //list of opened(unexplored) nodes
            var closedList = new List<TreeNode>(); //list of closed(explored) nodes

            if (debug) Helper.DebugLog("making mothernode and adding it to open(unexplored) list.");
            TreeNode firstNode = new TreeNode(null, null, startConditions, goal);
            openList.Add(firstNode);

            while(openList.Count > 0) //while we have some unexplored nodes
            {
                if (debug) Helper.DebugLog("getting most valuable node from open list.");
                TreeNode currentNode = openList.OrderByDescending(n => n.nodeValue).First();
                if (debug) Helper.DebugLog("got node with [nodeValue] = " + currentNode.nodeValue + " and action [" +
                    currentNode.nodeAction?.ActionName + "], removing it from open list and adding to closed(explored) list.");

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                var childs = GenerateChilds(currentNode, posibleActions, goal, debug);

                if (childs is null)
                {
                    if (debug) Helper.DebugLog("no childs generated for this node, proceed to next node in open list.");
                    continue;
                }

                if (debug) Helper.DebugLog("start processing generated childs:");
                foreach (var child in childs)
                {
                    if (debug) Helper.DebugLog("processing child with pathLength = " + child.pathLength + ", action [" + child.nodeAction.ActionName +
                        "].actionCost = " + child.nodeAction.ActionCost + ".");

                    if (openList.Any(n => Helper.isNodeEquals(n, child) && n.pathLength <= child.pathLength))
                    {
                        if (debug) Helper.DebugLog("node with same conditions and lower path found in open list, skipping this child.");
                        continue;
                    }

                    if (closedList.Any(n => Helper.isNodeEquals(n, child) && n.pathLength <= child.pathLength))
                    {
                        if (debug) Helper.DebugLog("node with same conditions and lower path found in closed list, skipping this child.");
                        continue;
                    }

                    if (child.nodeReachedGoal)
                    {
                        if (debug) Helper.DebugLog("this child reached goal, adding it in closed list.");
                        closedList.Add(child);
                    }
                    else
                    {
                        if (debug) Helper.DebugLog("adding this child to open list and proceed with it.");
                        openList.Add(child);
                    }
                }
            }

            if (debug) Helper.DebugLog("Done: got " + closedList.Count + " explored nodes.");
            return closedList;
        }
        #endregion

        #region inner funcs
        /// <summary>
        /// generating childs for provided node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="posibleActions">all posible actions</param>
        /// <param name="goal">current goal</param>
        /// <returns>list of childs if can be created, or null if not</returns>
        private static List<TreeNode> GenerateChilds(TreeNode node, List<ActorAction> posibleActions, Goal goal, bool debug = false)
        {
            if (debug) Helper.DebugLog("starting to generate node's childs:");
            var nodeActions = posibleActions.Where(a => a.isValid(node.nodeConditions)); //all valid actions with those conditions

            if (nodeActions.FirstOrDefault() is null)
            {
                if (debug) Helper.DebugLog("no valid actions for this node found, returning null.");
                return null;
            }

            if (debug) Helper.DebugLog("found " + nodeActions.Count() + " valid action(s) for this node, making childs with all this actions:");

            var childsList = new List<TreeNode>();

            foreach(var action in nodeActions)
            {
                if (debug) Helper.DebugLog("making new conditions for child node (parrent node conditions merged with child's node action post conditions).");
                var newConditions = Helper.MergeConditions(node.nodeConditions, action.PostConditions);

                if (debug) Helper.DebugLog("making and adding new node with action [" + action.ActionName + "] to child list.");
                var childNode = new TreeNode(action, node, newConditions, goal);

                childsList.Add(childNode);
            }

            if (debug) Helper.DebugLog("Done: returning child list with " + childsList.Count + " entry(s).");
            return childsList;
        }
        #endregion
    }
}
