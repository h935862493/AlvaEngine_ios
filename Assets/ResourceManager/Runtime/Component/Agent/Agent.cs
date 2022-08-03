using Alva.Common;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Alva.Runtime.Components
{
    public class Agent : MonoBehaviour
    {
        /// <summary>
        /// 组件唯一标志
        /// </summary>
        [SerializeField, HideInInspector]
        public string id;
        /// <summary>
        /// 是否公开显示样式
        /// </summary>
        private bool showStyle;
        /// <summary>
        /// 显示样式的名称列表
        /// </summary>
        private string[] style;
        string prefabType;
        [HideInInspector]
        public ComponentPrefabType componentPrefabType = ComponentPrefabType.None;
        /// <summary>
        /// 默认显示样式名
        /// </summary>
        [SerializeField, HideInInspector]
        public string defaultElement;
        public void Init(string prefabType)
        {
            this.prefabType = prefabType;
            componentPrefabType = (ComponentPrefabType)System.Enum.Parse(typeof(ComponentPrefabType), prefabType);
            Init();
        }
        #region 折叠组件
        public void ShrinkageAll()
        {
           // Shrinkage(true);
        }
        public void ShrinkageAllExcludeAgent()
        {
           // Shrinkage(false);
        }
        public void Shrinkage(bool hideAll = false)
        {
#if UNITY_EDITOR
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            var window = EditorWindow.GetWindow(type);
            FieldInfo info = type.GetField("m_Tracker", BindingFlags.NonPublic | BindingFlags.Instance);
            ActiveEditorTracker tracker = info.GetValue(window) as ActiveEditorTracker;

            for (int i = 0; i < tracker.activeEditors.Length; i++)
            {
                if (!hideAll && tracker.activeEditors[i].ToString() == " (Alva.Core.CustomEditors.ComponentStyleCustomEditor)")
                {
                    continue;
                }
                //这里1就是展开，0就是合起来
                tracker.SetVisible(i, 0);
                //UnityEditor.Undo.RecordObject(tracker.activeEditors[i], "Changed default element");
                //UnityEditor.EditorUtility.SetDirty(tracker.activeEditors[i]);
            }
         
#endif
        }
        #endregion
        public virtual void Init()
        {

        }
        public virtual void ExternalCall(string[] parameter, string method = default)
        {

        }
        public virtual void OnInspectorGUI()
        {
            OnBeforeInspectorGUI();
        }
#if UNITY_EDITOR
        public virtual void OnInspectorGUI(SerializedObject so)
        {

        }
        public virtual void OnInspectorGUI<T>(SerializedObject so, T action) where T : FunctionExtantion.FunctionExtantionEditor
        {

        }
        protected void OnDrawing(SerializedObject so, string name)
        {

            SerializedProperty sp = so.FindProperty(name);
            if (sp != null)
                EditorGUILayout.PropertyField(sp);


            //应用修改项
            so.ApplyModifiedProperties();
        }
#endif
        public virtual void OnBeforeInspectorGUI()
        {

        }
        public virtual void OnAfterInspectorGUI()
        {
            OnInspectorGUI();
        }
        public virtual string[] GetStyle()
        {
            return style;
        }
        public virtual bool GetShowStyle()
        {
            return showStyle;
        }
        public virtual string GetDefaultElement()
        {
            return defaultElement;
        }
        public virtual void OnStyleValueSelected(object value)
        {
            defaultElement = value.ToString();
        }


    }
    /// <summary>
    /// 组件预制体类别
    /// </summary>
    public enum ComponentPrefabType
    {
        AreaRecognition,
        ImageRecognition,
        ModelRecognition,
        SlamRecognition,
        Model,
        Label,
        Label2,
        Image,
        Image2,
        Button,
        Button2,
        CheckBox,
        CheckBox2,
        Select,
        Select2,
        Slider,
        Slider2,
        TextInput,
        TextInput2,
        Toggle,
        Toggle2,
        ToggleButton,
        ToggleButton2,
        Audio,
        Audio2,
        Video,
        Video2,
        File,
        File2,
        Hyperlink,
        Hyperlink2,
        ProgressBar,
        ProgressBar2,
        Gauge,
        Gauge2,
        DataGrid,
        DataGrid2,
        BarChart,
        BarChart2,
        TimeSeriesChart,
        TimeSeriesChart2,
        Hotspot,
        None
    }
}