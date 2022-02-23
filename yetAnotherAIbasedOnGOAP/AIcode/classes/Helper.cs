//helper class with some calcs for entire system

using System.Collections.Generic;
using System.Linq;

namespace AIcode.classes
{
    static class Helper
    {
        #region constants
        public const string UNDEFINED_CONDITION = "undefined_condition";
        public const string UNDEFINED_GOAL = "undefined_goal";
        public const string UNDEFINED_ACTION = "undefined_action";
        #endregion

        #region funcs for constructors
        /// <summary>
        /// it will replace empty names with "undefined"
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rename"></param>
        /// <returns></returns>
        public static string checkName(string name, string rename)
        {
            string result = name;

            if (name is null || name == string.Empty) result = rename;

            return result;
        }

        /// <summary>
        /// it will remove all "undefined" enties, and duplicates
        /// </summary>
        /// <param name="listToClean"></param>
        /// <returns>list, or null - if there was no elements</returns>
        public static List<Condition> cleanConditionList(List<Condition> listToClean)
        {
            if (listToClean is null || listToClean.Count == 0) return null;

            var result = listToClean.Where(c => c.ConditionName != UNDEFINED_CONDITION); //clean from undefined ones

            if (result.FirstOrDefault() is null) return null;

            var noDupes = result.GroupBy(c => c.ConditionName).Select(c => c.FirstOrDefault()); //clean from duplicates

            if (!result.SequenceEqual(noDupes)) result = noDupes; //if something removed

            return result.ToList();
        }

        /// <summary>
        /// convert Dictionary|name, state| to list|Condition|
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>list of conditions converted from dict or null if there was none</returns>
        public static List<Condition> GetConditionListFromDict(Dictionary<string, bool> dict)
        {
            if (dict is null || dict.Count == 0) return null;

            Condition condition;
            var tempList = new List<Condition>();

            foreach (var c in dict)
            {
                condition = new Condition(c.Key, c.Value);
                tempList.Add(condition);
            }

            return tempList;
        }

        /// <summary>
        /// convert arrays of string(names) and bool(statuses) to condition list
        /// </summary>
        /// <param name="names"></param>
        /// <param name="statuses"></param>
        /// <returns></returns>
        public static List<Condition> GetConditionListFromArrays(string[] names, bool[] statuses)
        {
            if (names is null || names.Length == 0) return null;

            if (statuses is null || statuses.Length == 0) statuses = new bool[names.Length];

            if (statuses.Length < names.Length)
            {
                var tmp = new bool[names.Length];
                for (int i = 0; i < statuses.Length; i++) tmp[i] = statuses[i];

                statuses = tmp;
            }

            Condition condition;
            var tmpList = new List<Condition>();

            for (int i = 0; i < names.Length; i++)
            {
                condition = new Condition(names[i], statuses[i]);
                tmpList.Add(condition);
            }

            return tmpList;
        }
        #endregion

        #region other usefull funcs
        /// <summary>
        /// simple func to merge two condition lists
        /// </summary>
        /// <param name="mergeWith">list that need to be changed</param>
        /// <param name="whatToMerge">list with changes</param>
        /// <returns>new conditions list representing union of mergeWith with whatToMerge</returns>
        public static List<Condition> MergeConditions(List<Condition> mergeWith, List<Condition> whatToMerge)
        {
            if (mergeWith is null || whatToMerge is null) return null;

            var mergedList = new List<Condition>(mergeWith);

            foreach(var condition in whatToMerge)
            {
                int indx = mergedList.FindIndex(c => c.ConditionName == condition.ConditionName); //gettin index of same name condition from mergeWith list

                if (indx != -1) mergedList[indx] = condition; //if same name condition exists in mergeWith, replacing it with value from whatToMerge
            }

            return mergedList;
        }        

        /// <summary>
        /// check if all conditions to check met in provided list of conditions
        /// </summary>
        /// <param name="providedConditions">provided list of conditions (for example all currend world conditions)</param>
        /// <param name="conditionsToCheck">conditions we look for in provided</param>
        /// <returns>true if all conditions from conditionsToCheck met in provided and false if not</returns>
        public static bool isAllConditionsMet(List<Condition> providedConditions, List<Condition> conditionsToCheck)
        {
            if (providedConditions is null || conditionsToCheck is null) return false;

            if (providedConditions.Count < conditionsToCheck.Count) return false; //can't check all conditions to check if there is less conditions in checkable ¯\_(ツ)_/¯

            var intersects = providedConditions.Intersect(conditionsToCheck); //findin intersects (only exact names and statuses will count)

            if (intersects.FirstOrDefault() is null) return false; //no intersects

            if (conditionsToCheck.Count != intersects.Count()) return false; //not all conditions met

            return true;
        }

        /// <summary>
        /// check 2 nodes for their conditions list equality
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>true if nodes have same conditions and false if not</returns>
        public static bool isNodeEquals(TreeNode first, TreeNode second)
        {
            return isAllConditionsMet(first.nodeConditions, second.nodeConditions);
        }

        /// <summary>
        /// calculates node's value
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="goal"></param>
        /// <returns>number of goal's conditions met in this node's condition list</returns>
        public static int nodeValueCalc(List<Condition> conditions, Goal goal)
        {
            if (conditions is null || goal is null) return 0;

            return conditions.Intersect(goal.GoalConditions).Count();
        }
        #endregion

        #region debug logging func
        /// <summary>
        /// func for logging debug info, just prints messages to Godot console (change it with your needs(like save log to file or something)/engine(replace Godot.GD.PrintRaw with System.Console.Write for example)/etc.)
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="debugOn">is debug mode on</param>
        /// <param name="mute">show this message or not</param>
        public static void DebugLog(string message, bool showDateTime = false, bool mute = false)
        {
            if (!mute)
            {
                if (showDateTime) message = "[" + System.DateTime.Now.ToString() + "]\n" + message;
                Godot.GD.PrintRaw(message, "\n");
            }
        }
        #endregion
    }
}
