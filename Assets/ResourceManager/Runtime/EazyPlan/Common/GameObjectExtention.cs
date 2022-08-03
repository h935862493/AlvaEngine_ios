using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Alva.EazyPlan
{
    public static class GameObjectExtention
    {
        public static T GetOrAddComponent<T>( this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
    }
}
