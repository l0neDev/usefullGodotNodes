//the main class that actualy makes plan (:

using System.Collections.Generic;
using System.Linq;

using AIcode.classes;

namespace AIcode.funcs
{
    static class Planner
    {
        #region public funcs
        /// <summary>
        /// main func to get an actions chain to reach [goal] (note: if you have other goals just change ur goals 'on the fly' according to changed world/actor/etc. conditions and replan your actions)
        /// </summary>
        /// <param name="goal">goal to reach from [startConditions]</param>
        /// <param name="possibleActions">all posible actions for actor you making plan for</param>
        /// <param name="startConditions">current world/actor/etc. conditions to start plan with</param>
        /// <param name="prefferCheapestPath">what path type preffered cheapest or shortest</param>
        /// <param name="debug">uses debug func if true (perfomance cost!)</param>
        /// <returns>list of actions for actor to do to reach this [goal] (from first to last {first element of list == first action to take}), or Null if can't reach/wrong func args/etc.</returns>
        public static List<ActorAction> GetPlan(Goal goal, List<ActorAction> possibleActions, List<Condition> startConditions, bool prefferCheapestPath = true, bool debug = false)
        {
            if (debug) Helper.DebugLog("starting to make plan.", true);
            
            if (goal is null || possibleActions is null || possibleActions.Count == 0 || startConditions is null || startConditions.Count == 0)
            {
                if (debug) Helper.DebugLog("one or more args are invalid, returning null.");
                return null;
            }

            if (goal.isReached(startConditions))
            {
                if (debug) Helper.DebugLog("provided goal already reached, returning null.");
                return null;
            }

            var tree = TreeBuilder.GetTree(goal, possibleActions, startConditions, debug);

            if (!tree.Any(n => n.nodeReachedGoal))
            {
                if (debug) Helper.DebugLog("provided goal can't be reached with those [startConditions] and [possibleActions], returning null.");
                return null; //note: if you have some subgoals just switch to them if plan for this goal returns null
            }

            var listOfChains = GetActionChainFromTree(tree, debug);

            if (prefferCheapestPath) return GetLesserCostChain(listOfChains, debug);
            
            return GetShortestChain(listOfChains, debug);
        }
        #endregion

        #region inner funcs
        /// <summary>
        /// convert premade tree to list of action chains
        /// </summary>
        /// <param name="tree">premade tree</param>
        /// <param name="debug">uses debug func if true (perfomance cost!)</param>
        /// <returns>list of action chains, each chain ordered from first to last (firls element == first action to do, last element == action to reach goal) or Null if tree is empty</returns>
        private static List<List<ActorAction>> GetActionChainFromTree(List<TreeNode> tree, bool debug = false)
        {
            if (debug) Helper.DebugLog("starting to convert tree to actions chains:");

            if (tree is null || tree.Count == 0)
            {
                if (debug) Helper.DebugLog("[tree] is invalid, returning null.");
                return null;
            }

            if (debug) Helper.DebugLog("getting all nodes that reached the goal.");
            var goalNodes = tree.Where(n => n.nodeReachedGoal);
            if (debug) Helper.DebugLog("got " + goalNodes.Count() + " node(s).");

            var listOfChains = new List<List<ActorAction>>();

            foreach(var goalNode in goalNodes)
            {
                if (debug) Helper.DebugLog("building chain for node with action [" + goalNode.nodeAction.ActionName + "], pathLength = " + goalNode.pathLength + ":");
                var currentNode = goalNode;

                var actionChain = new List<ActorAction>();

                do
                {
                    var actionToAdd = currentNode.nodeAction;
                    if (actionToAdd != null) //do not add mothernode (mothernode have no action)
                    {
                        if (debug) Helper.DebugLog("adding node with [" + actionToAdd.ActionName + "] action");
                        actionChain.Add(actionToAdd);
                    }
                    else
                    {
                        if (debug) Helper.DebugLog("node have no action, skipping.");
                    }

                    if (debug) Helper.DebugLog("getting parrent of this node.");
                    currentNode = currentNode.parrent;
                    if (debug && currentNode is null) Helper.DebugLog("node have no parrent, looks like its mothernode, stopping process.");
                }
                while (currentNode != null); //while meet mothernode (mothernode have no parrent)

                if (actionChain.Count > 0)
                {
                    if (debug) Helper.DebugLog("Done: got action chain with " + actionChain.Count + " action(s), reversing order, and adding it to list.");
                    actionChain.Reverse(); //more comfortable to use first to last (first element of list == first action to do)

                    listOfChains.Add(actionChain);
                }
                else
                {
                    if (debug) Helper.DebugLog("failed, no nodes with actions.");
                }
            }

            if (listOfChains.Count > 0)
            {
                if (debug) Helper.DebugLog("Done: " + listOfChains.Count + " chains made.");

                return listOfChains;
            }

            if (debug) Helper.DebugLog("failed to make list of chains.");
            return null;
        }

        /// <summary>
        /// func to get lowest total actions cost chain
        /// </summary>
        /// <param name="listOfChains">list of chains to check</param>
        /// <param name="debug">uses debug func if true (perfomance cost!)</param>
        /// <returns>one of the chains that have lowest total actions cost or null if [listOfChains] is empty</returns>
        private static List<ActorAction> GetLesserCostChain(List<List<ActorAction>> listOfChains, bool debug = false)
        {
            if (debug) Helper.DebugLog("getting cheapest actions chain:");

            if (listOfChains is null || listOfChains.Count == 0)
            {
                if (debug) Helper.DebugLog("list of chains is invalid, returning null.");
                return null;
            }

            int minSum = int.MaxValue;
            List<ActorAction> lessCostChain = null;

            foreach(var chain in listOfChains)
            {
                int sum = 0;
                chain.ForEach(a => sum += a.ActionCost);

                if (debug) Helper.DebugLog("chain with " + chain.Count + " actions have " + sum + " total cost.");

                if (sum < minSum)
                {
                    if (debug) Helper.DebugLog("it's a cheapest chain for now.");
                    minSum = sum;
                    lessCostChain = chain;
                }
            }

            if (debug) Helper.DebugLog("Done: returning " + lessCostChain.Count + " actions chain with " + minSum + " total actions cost.");
            return lessCostChain;
        }

        /// <summary>
        /// func to get shortest chain (containing less actions than others)
        /// </summary>
        /// <param name="listOfChains">list of chains to check</param>
        /// <param name="debug">uses debug func if true (perfomance cost!)</param>
        /// <returns>one of the shortest chains or null if there is no chains in list</returns>
        private static List<ActorAction> GetShortestChain(List<List<ActorAction>> listOfChains, bool debug = false)
        {
            if (debug) Helper.DebugLog("getting shortest actions chain.");

            if (listOfChains is null || listOfChains.Count == 0)
            {
                if (debug) Helper.DebugLog("list of chains is invalid, returning null.");
                return null;
            }

            return listOfChains.OrderBy(c => c.Count).First(); //just sort by chain length and return first chain
        }
        #endregion
    }
}
