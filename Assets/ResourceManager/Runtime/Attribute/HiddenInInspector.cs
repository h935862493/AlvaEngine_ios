using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Alva.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class HiddenInInspector : PropertyAttribute
    {
        public readonly string conditionFieldName;
        public readonly object comparationValue;
        public HiddenInInspector(string conditionFieldName, object comparationValue = null)
        {
            this.conditionFieldName = conditionFieldName;
            this.comparationValue = comparationValue;
        }
    }
}
