using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using LJ.VisualAVG;
using VisualGraphInEditor;
using UnityEditor.Experimental.GraphView;

[CustomEditor(typeof(Node_Menu))]
public class NodeMenuEditor : Editor
{
    private SerializedProperty options;
    private SerializedProperty ID;

    private SerializedObject serializedTarget;
    private MonoScript monoScript;

    private Node_Menu node_Menu;
    private GUIStyle idStyle;
    private void OnEnable()
    {
        node_Menu = target as Node_Menu;
        try
        {
            this.monoScript = MonoScript.FromScriptableObject(this.target as ScriptableObject);
            options = serializedObject.FindProperty(nameof(node_Menu.options));
            ID = serializedObject.FindProperty(nameof(node_Menu.menuID));
        }
        catch (System.Exception)
        {
        }
        idStyle = new GUIStyle();
        idStyle.alignment = TextAnchor.MiddleCenter;
        idStyle.fontStyle = FontStyle.Bold;
        idStyle.fontSize = 20;
        idStyle.normal.textColor = Color.yellow;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUIUtility.labelWidth = 120;
        //绘制脚本
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", this.monoScript, typeof(MonoScript), false);
        EditorGUI.EndDisabledGroup();

        //绘制ID
        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("ID: " + ID.intValue.ToString(), idStyle);
        EditorGUILayout.Space(15);

        for (int i = 0; i < node_Menu.options.Count; i++)
        {
            EditorGUILayout.LabelField($"选项{i + 1}:");
            node_Menu.options[i] = EditorGUILayout.TextField(node_Menu.options[i]);
        }

        var node = (node_Menu.graphElement as Node);

        //添加 减少
        using (var horScopr = new GUILayout.HorizontalScope())
        {
            GUILayout.Space(170);
            if (GUILayout.Button("+"))
            {
                node_Menu.options.Add(string.Empty);
                VisualGraphEditor.visualGraphView.CreatePort(node, $"选项{node_Menu.options.Count}", VisualGraphRuntime.VisualGraphPort.PortDirection.Output,false);
            }
            if (GUILayout.Button("-"))
            {
                if (node_Menu.options.Count > 0)
                {
                    node_Menu.options.Remove(node_Menu.options.Last());
                    VisualGraphEditor.visualGraphView.RemovePort(node,node_Menu.Outputs.Last().editor_port as Port);
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
