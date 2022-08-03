using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Alva.Attributes
{
    public class LabelAttribute : PropertyAttribute
    {
        public string label;
        public LabelAttribute(string label)
        {
            this.label = label;
        }
    }
}