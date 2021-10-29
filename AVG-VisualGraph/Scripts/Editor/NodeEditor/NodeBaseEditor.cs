using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace LJ.VisualAVG
{
    [CustomEditor(typeof(Node_Base), editorForChildClasses: true)]
    public class NodeBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUIUtility.labelWidth = 120;
            base.OnInspectorGUI();
        }
    }
}