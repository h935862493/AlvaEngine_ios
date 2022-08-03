
using System.IO;
using UnityEditor;

namespace Alva.Common
{

    public class FunctionExtantion
    {
#if UNITY_EDITOR
        public class FunctionExtantionEditor: UnityEditor.Editor
        {
            public virtual void ReplaceRecognitionImageFile(UnityEngine.GameObject obj)
            {
               
            }

            public virtual void ReplaceRecognitionModelFile(UnityEngine.GameObject obj)
            {

            }
        }
#endif
    }
}
