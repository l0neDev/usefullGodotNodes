//class for the condition. the state of world/actor/etc.
//Example: name = "have ammo", state = false - represents that Actor have no ammo ¯\_(ツ)_/¯

using System;

namespace AIcode.classes
{
    class Condition : IEquatable<Condition> //custom comparer here, do not change that! need for other funcs to work properly with conditions
    {
        #region constructors
        /// <summary>
        /// stright constructor, makes condition with provided name and false status by default if status not provided
        /// </summary>
        /// <param name="name"></param>
        /// <param name="status"></param>
        public Condition(string name, bool status = false)
        {
            _ConditionName = Helper.checkName(name, Helper.UNDEFINED_CONDITION);
            ConditionStatus = status;
        }
        #endregion

        #region variables
        private string _ConditionName;
        public string ConditionName => _ConditionName;

        public bool ConditionStatus { get; set; }
        #endregion

        #region overrides
        public bool Equals(Condition other)
        {
            if (other is null) return false;

            return ConditionName == other.ConditionName && ConditionStatus == other.ConditionStatus;
        }

        public override bool Equals(object obj) => Equals(obj as Condition);
        public override int GetHashCode() => (ConditionName, ConditionStatus).GetHashCode();
        #endregion
    }
}
