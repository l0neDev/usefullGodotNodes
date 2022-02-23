//goal class, what Planner will try to reach

using System.Collections.Generic;

namespace AIcode.classes
{
    class Goal
    {
        #region constructors        
        /// <summary>
        /// stright constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conditionsToReachGoal"></param>
        public Goal(string name, List<Condition> conditionsToReachGoal)
        {
            _GoalName = Helper.checkName(name, Helper.UNDEFINED_GOAL);
            _GoalConditions = Helper.cleanConditionList(conditionsToReachGoal);
        }

        /// <summary>
        /// generic constructor from dictionary
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conditionsToReachGoal"></param>
        public Goal(string name, Dictionary<string, bool> conditionsToReachGoal)
        {
            _GoalName = Helper.checkName(name, Helper.UNDEFINED_GOAL);

            var tmpList = Helper.GetConditionListFromDict(conditionsToReachGoal);
            _GoalConditions = Helper.cleanConditionList(tmpList);
        }

        /// <summary>
        /// generic constructor from arrays
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conditionsToReachGoalNames"></param>
        /// <param name="conditionsToReachGoalStatuses"></param>
        public Goal(string name, string[] conditionsToReachGoalNames, bool[] conditionsToReachGoalStatuses)
        {
            _GoalName = Helper.checkName(name, Helper.UNDEFINED_GOAL);

            var tmpList = Helper.GetConditionListFromArrays(conditionsToReachGoalNames, conditionsToReachGoalStatuses);
            _GoalConditions = Helper.cleanConditionList(tmpList);
        }
        #endregion

        #region variables                
        private string _GoalName;
        public string GoalName => _GoalName;

        private List<Condition> _GoalConditions;
        /// <summary>
        /// state of the world to reach this goal
        /// </summary>
        public List<Condition> GoalConditions => _GoalConditions;
        #endregion

        #region funcs
        /// <summary>
        /// func to check if this goal will be reached with provided conditions
        /// </summary>
        /// <param name="conditions">provided conditions</param>
        /// <returns>true if reached and false if not</returns>
        public bool isReached(List<Condition> conditions)
        {
            if (GoalConditions is null) return true; //if this goal have no conditions (O_o) -> it's always reached ¯\_(ツ)_/¯

            if (conditions is null || conditions.Count == 0) return false; //if conditions to check not porvided -> cant check -> not reached

            if (!Helper.isAllConditionsMet(conditions, GoalConditions)) return false; //not all conditions of this goal met -> not reached

            return true; //if still here -> reached!
        }
        #endregion
    }
}
