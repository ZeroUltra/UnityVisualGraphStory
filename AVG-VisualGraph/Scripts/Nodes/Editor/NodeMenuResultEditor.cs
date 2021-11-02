using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VisualGraphRuntime;
using LJ.VisualAVG;
using System.Linq;
using VisualGraphInEditor;
using UnityEditor.Experimental.GraphView;

[CustomEditor(typeof(Node_MenuResult))]
public class NodeMenuResultEditor : Editor
{
    private SerializedProperty targetMenuID;
    private Node_MenuResult menuResult;

    private void OnEnable()
    {
        try
        {
            menuResult = target as Node_MenuResult;
            targetMenuID = serializedObject.FindProperty(nameof(menuResult.targetMenuID));
        }
        catch (System.Exception)
        {
        }
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUIUtility.labelWidth = 120;
        EditorGUILayout.PropertyField(targetMenuID, new GUIContent("目标ID:"));


        using (var horScopr = new GUILayout.HorizontalScope())
        {
            GUILayout.Space(100);
            if (GUILayout.Button("Refresh"))
            {
                var nodeMenu = (Node_Menu)menuResult.graph.Nodes.Find(item =>
                {
                    return item is Node_Menu && (item as Node_Menu).menuID == this.targetMenuID.intValue;
                });
                if (nodeMenu != null)
                {
                    var options = nodeMenu.options.ToArray();

                    //删除全部
                    for (int i = menuResult.Outputs.Count() - 1; i >= 0; i--)
                    {
                        VisualGraphEditor.visualGraphView.RemovePort((menuResult.graphElement as Node), menuResult.Outputs.Last().editor_port as Port);
                    }
                    //重新创建
                    for (int i = 0; i < nodeMenu.options.Count; i++)
                    {
                        VisualGraphEditor.visualGraphView.CreatePort((menuResult.graphElement as Node), $"{options[i]}", VisualGraphPort.PortDirection.Output, false);
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();

    }
}
