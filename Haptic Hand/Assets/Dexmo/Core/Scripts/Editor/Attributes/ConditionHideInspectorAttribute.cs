using System;
using UnityEngine;
using System.Collections;

namespace Dexmo.Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionalVisibleAttribute : PropertyAttribute
    {
        //The name of the bool field that will be in control
        public string ConditionalSourceField = "";
        public string ConditionValue = "";

        public ConditionalVisibleAttribute(string conditionalSourceField, string conditianValue)
        {
            this.ConditionalSourceField = conditionalSourceField;
            ConditionValue = conditianValue;
        }

        public ConditionalVisibleAttribute(string conditionalSourceField, bool conditianValue)
        {
            this.ConditionalSourceField = conditionalSourceField;
            ConditionValue = conditianValue.ToString();
        }
    }
}
