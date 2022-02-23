//class for some abstract action for your actor to do.
//Its not actual actions, just an "avatars" for em, u need to code actual actions urself, cuz there is no universal ones ¯\_(ツ)_/¯

using System.Collections.Generic;

namespace AIcode.classes
{
    class ActorAction
    {
        #region constructors
        /// <summary>
        /// stright constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cost"></param>
        /// <param name="preConditions"></param>
        /// <param name="postConditions"></param>
        public ActorAction(string name, int cost, List<Condition> preConditions, List<Condition> postConditions)
        {
            _ActionName = Helper.checkName(name, Helper.UNDEFINED_ACTION);
            ActionCost = cost;
            _PreConditions = Helper.cleanConditionList(preConditions);
            _PostConditions = Helper.cleanConditionList(postConditions);
        }

        /// <summary>
        /// generic constructor from Dicts
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cost"></param>
        /// <param name="preConditions"></param>
        /// <param name="postConditions"></param>
        public ActorAction(string name, int cost, Dictionary<string, bool> preConditions, Dictionary<string, bool> postConditions)
        {
            _ActionName = Helper.checkName(name, Helper.UNDEFINED_ACTION);
            ActionCost = cost;

            var tmpList = Helper.GetConditionListFromDict(preConditions);
            _PreConditions = Helper.cleanConditionList(tmpList);

            tmpList = Helper.GetConditionListFromDict(postConditions);
            _PostConditions = Helper.cleanConditionList(tmpList);
        }

        /// <summary>
        /// generic constructor from arrays
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cost"></param>
        /// <param name="preConditionsNames"></param>
        /// <param name="preConditionsStatuses"></param>
        /// <param name="postConditionsNames"></param>
        /// <param name="postConditionsStatuses"></param>
        public ActorAction(string name, int cost, string[] preConditionsNames, bool[] preConditionsStatuses, string[] postConditionsNames, bool[] postConditionsStatuses)
        {
            _ActionName = Helper.checkName(name, Helper.UNDEFINED_ACTION);
            ActionCost = cost;

            var tmpList = Helper.GetConditionListFromArrays(preConditionsNames, preConditionsStatuses);
            _PreConditions = Helper.cleanConditionList(tmpList);

            tmpList = Helper.GetConditionListFromArrays(postConditionsNames, postConditionsStatuses);
            _PostConditions = Helper.cleanConditionList(tmpList);
        }
        #endregion

        #region variables
        private string _ActionName;
        public string ActionName => _ActionName;

        /// <summary>
        /// higher cost -> lower priority
        /// </summary>
        public int ActionCost { get; set; } //?

        private List<Condition> _PreConditions;
        /// <summary>
        /// conditions to run this action
        /// </summary>
        public List<Condition> PreConditions => _PreConditions;

        private List<Condition> _PostConditions;
        /// <summary>
        /// conditions on action complete. Action's result.
        /// </summary>
        public List<Condition> PostConditions => _PostConditions;
        #endregion

        #region funcs
        /// <summary>
        /// check if this action can run with provided conditions
        /// </summary>
        /// <param name="conditions">provided conditions</param>
        /// <returns>true if can and false if not</returns>
        public bool isValid(List<Condition> conditions)
        {
            return Helper.isAllConditionsMet(conditions, PreConditions);
        }
        #endregion
    }
}
