using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LJ.VisualAVG;
[CustomEditor(typeof(Node_BG))]
public class NodeBGEditor : Editor
{
    AVGGraph avgGraph;
    Node_BG node_BG;

    private string[] bgNames;
    private void OnEnable()
    {
        try
        {
            node_BG = target as Node_BG;
            avgGraph = node_BG.graph as AVGGraph;
        }
        catch (System.Exception)
        {

        }
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        bgNames = avgGraph.graphAssets.BgNames;
        //显示选项
        int currIndex = System.Array.IndexOf<string>(bgNames, node_BG.spBgName);
        if (currIndex == -1) currIndex = 0; //默认显示第一个
        node_BG.spBgName = bgNames[EditorGUILayout.Popup(currIndex, bgNames)];
        //显示图片
        using (var scope = new GUILayout.HorizontalScope())
        {
            GUILayout.Space(150);
            Texture2D previewTexture = AssetPreview.GetAssetPreview(avgGraph.graphAssets.GetBgSprite(node_BG.spBgName));
            GUILayout.Box(previewTexture, GUILayout.Width(46), GUILayout.Height(100));
        }
        node_BG.fXType=(AVGHelper.FXType2)EditorGUILayout.EnumPopup("效果", node_BG.fXType);
        //EditorGUILayout.ObjectField(avgGraph.graphAssets.GetBgSprite(node_BG.spBgName), typeof(Sprite), allowSceneObjects: false,GUILayout.Width(80), GUILayout.Height(180));
        EditorGUIUtility.labelWidth = 120;
    }
}
