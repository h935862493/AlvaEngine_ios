using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Alva.Attributes
{
   
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class InspectorContextMenuItemAttribute : Attribute
    {
        public string menuItem;
        public int priority;

        public InspectorContextMenuItemAttribute(string menuItem, int priority)
        {
            this.menuItem = menuItem;
            this.priority = priority;
        }

        public InspectorContextMenuItemAttribute(string menuItem) : this(menuItem, 0)
        {

        }
    }
}
