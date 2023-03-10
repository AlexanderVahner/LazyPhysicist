using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.DB
{
    public enum ConditionOperator { Equlal, NotEqual, Less, More, LessOrEqual, MoreOrEqual };
    public class FilterCondition
    {
        private string ConditionOperatorToString(ConditionOperator o)
        {
            string result = " = ";
            switch (o)
            {
                case ConditionOperator.Equlal:
                    result = " = ";
                    break;
                case ConditionOperator.NotEqual:
                    result = " <> ";
                    break;
                case ConditionOperator.Less:
                    result = " < ";
                    break;
                case ConditionOperator.More:
                    result = " > ";
                    break;
                case ConditionOperator.LessOrEqual:
                    result = " <= ";
                    break;
                case ConditionOperator.MoreOrEqual:
                    result = " >= ";
                    break;

            }
            return result;
        }
        public FilterCondition(string key, object value, ConditionOperator operation = ConditionOperator.Equlal)
        {
            string stringValue = Equals(value.GetType(), typeof(string)) ? $@"""{value}""" : value.ToString();
            ConditionString = $"{key}{operation}{stringValue}";
        }
        public FilterCondition(string condition)
        {
            ConditionString = condition;
        }

        public string ConditionString { get; private set; }
    }
}
