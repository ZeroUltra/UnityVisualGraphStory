///-------------------------------------------------------------------------------------------------
// author: William Barry
// date: 2020
// Copyright (c) Bus Stop Studios.
///-------------------------------------------------------------------------------------------------
using LJ.VisualAVG;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualGraphRuntime;

namespace VisualGraphInEditor
{
    public sealed class VisualGraphEditor : EditorWindow
    {
        private AVGProcesser avgProcesser;
        private Component visualGraphComponent;
        private VisualGraphView graphView;
        private VisualGraph visualGraph;
        public UnityEngine.Object objectSelection; // Used for enter/exit playmode

        private float scale;
        private Vector3 pos;

        public static VisualGraphView visualGraphView;
     
        /// <summary>
        /// Create a Visual Graph Window to support a VisualGraph object
        /// </summary>
        /// <param name="_visualGraph"></param>
        /// <returns></returns>
        public static VisualGraphEditor CreateGraphViewWindow(VisualGraph _visualGraph, bool forceSet = false)
        {
            var window = GetWindow<VisualGraphEditor>();
            window.SetVisualGraph(_visualGraph, forceSet);
            return window;
        }

        /// <summary>
        /// Create visual elements using Unity GraphView (Experiemental)
        /// </summary>
        private void OnEnable()
        {
            rootVisualElement.Clear();

            // Create the GraphView
            graphView = new VisualGraphView(this)
            {
                name = "Visual Graph View",
            };
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);

            // Add Toolbar to Window
           
            graphView.CreateMinimap(this.position.width);
            WaitUpdateView();
            //黑板
            //graphView.CreateBlackboard();

            EditorApplication.playModeStateChanged += LogPlayModeState;
            AVGProcesser.OnChangeNodeEvent += AvgProcesser_OnChangeNodeEvent;
            visualGraphView = graphView;
            visualGraphView.OnEnable();
        }
        //生成Toolbar
        async void WaitUpdateView()
        {
            await System.Threading.Tasks.Task.Delay(50);
            GenerateToolbar();
        }

        //运行时显示
        private void AvgProcesser_OnChangeNodeEvent(Node_Base node_Base)
        {
            graphView.ClearSelection();
            if (node_Base != null)
            {
                graphView.AddToSelection(node_Base.graphElement as VisualGraphNodeView);
            }
        }

        private void OnDisable()
        {
            scale = graphView.scale;
            pos = graphView.contentViewContainer.transform.position;
            EditorApplication.playModeStateChanged -= LogPlayModeState;
            AVGProcesser.OnChangeNodeEvent -= AvgProcesser_OnChangeNodeEvent;
            visualGraphView.OnDisable();
            visualGraphView = null;
        }

        
        private void LogPlayModeState(PlayModeStateChange state)
        {

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    objectSelection = Selection.activeObject;
                    Selection.activeObject = null;
                    break;

                case PlayModeStateChange.EnteredPlayMode:
                    Selection.activeObject = objectSelection;
                    graphView.SetGraph(visualGraph);
                    graphView.UpdateViewTransform(pos, Vector3.one * scale);
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    objectSelection = Selection.activeObject;
                    Selection.activeObject = null;
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    Selection.activeObject = objectSelection;
                    break;
            }
        }

        /// <summary>
        /// Change the Visual Graph
        /// </summary>
        /// <param name="_visualGraph"></param>
        private void SetVisualGraph(VisualGraph _visualGraph, bool forceSet = false)
        {
            visualGraph = _visualGraph;
            if (visualGraph == null)
            {
                titleContent = new GUIContent("Empty");
            }
            else
            {
                titleContent = new GUIContent(visualGraph.name);
            }
            graphView.SetGraph(visualGraph);
            //GenerateToolbar();
        }

        /// <summary>
        /// Window toolbar
        /// </summary>
        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            ToolbarToggle minimap_toggle = new ToolbarToggle();
            minimap_toggle.text = "Show MiniMap";
            minimap_toggle.SetValueWithoutNotify(true);
            minimap_toggle.RegisterCallback<ChangeEvent<bool>>(
                (evt) =>
                {
                    graphView.Minimap.visible = evt.newValue;
                }
            );
            toolbar.Add(minimap_toggle);

            //黑板
            //ToolbarToggle blackboard_toggle = new ToolbarToggle();
            //blackboard_toggle.text = "Show Blackboard";
            //blackboard_toggle.SetValueWithoutNotify(true);
            //blackboard_toggle.RegisterCallback<ChangeEvent<bool>>(
            //    (evt) =>
            //    {
            //        graphView.Blackboard.visible = evt.newValue;
            //    }
            //);
            //toolbar.Add(blackboard_toggle);

            //绘制一个按钮
            Button btnasset = new Button();
            btnasset.text = "Open Assets";
            btnasset.RegisterCallback<ClickEvent>((evt) =>
            {
                var obj = (visualGraph as AVGGraph).graphAssets;
                if (obj != null)
                    AssetDatabase.OpenAsset(obj);
            });
            toolbar.Add(btnasset);

            //绘制obj区域
            ObjectField assetobj = new ObjectField();
            assetobj.allowSceneObjects = false;
            assetobj.objectType = typeof(AVGGraphAssets);
            assetobj.style.width = 300;
            //if (visualGraph == null) Debug.Log("sdfasd");
            if ((visualGraph as AVGGraph).graphAssets != null)
                assetobj.value = (visualGraph as AVGGraph).graphAssets;
            //改变事件
            assetobj.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) =>
            {
                if (evt.newValue != null)
                    (visualGraph as AVGGraph).graphAssets = evt.newValue as AVGGraphAssets;
            });
            toolbar.Add(assetobj);
            rootVisualElement.Add(toolbar);
        }

        /// <summary>
        /// When the GUI changes update the view (this positions the blackboard and minimap)
        /// </summary>
        private void OnGUI()
        {
            if (graphView != null)
            {
                graphView.OnGUI();
            }

        }

        /// <summary>
        /// TODO: When this works we can highlight the node that is active great for
        ///		  runtime cases and viewing things like FSM
        /// </summary>
        //private void Update()
        //{
        //    if (Application.isPlaying)
        //    {
        //        if (graphView != null)
        //        {
        //            graphView.Update();
        //        }
        //    }
        //}

        /// <summary>
        /// Handle selection change. This will check the active object to see if it is a
        /// VisualGraph Scriptable object. If it is not then it will see if the selected
        /// object is a GameObject. If the selection is a GameObject then we iterate over
        /// all MonoBehaviour (scripts) to see if one is a has a VisualGraphMonoBehaviour<>.
        /// If we find a Component that is a VisualGraphMonoBehaviour<> then we first check
        /// if there is an InternalGraph (which is used during runtime) otherwise we will
        /// use the Graph itself (needs to change when runtime is invoked in the editor)
        /// </summary>
        //void OnSelectionChange()
        //{
        //    return;
        //    visualGraphComponent = null;

        //    VisualGraph graph = Selection.activeObject as VisualGraph;
        //    if (graph == null && graphView.activeVisualGraph)
        //    {
        //        //SetVisualGraph(null);
        //    }
        //    else if (graph != null && graph != visualGraph)
        //    {
        //        if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(graph)))
        //        {
        //            SetVisualGraph(graph);
        //        }
        //    }
        //    else
        //    {
        //        GameObject go = Selection.activeObject as GameObject;
        //        if (go != null)
        //        {
        //            Component[] components = go.GetComponents(typeof(MonoBehaviour));
        //            foreach (var comp in components)
        //            {
        //                // Because everything in Components is a MonoBehaviour we can get the base type
        //                // If they base type is a generic of type VisualGraphMonoBehaviour<> then we can try and
        //                // get the internal graph (this is for editor runtime). If that doesn't exist use the set graph.
        //                if (comp == null) continue;

        //                Type t = comp.GetType().BaseType;
        //                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(VisualGraphMonoBehaviour<>))
        //                {
        //                    graph = (VisualGraph)t.GetField("internalGraph", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(comp);
        //                    if (graph != null)
        //                    {
        //                        visualGraphComponent = comp;
        //                        SetVisualGraph(graph);
        //                        return;
        //                    }

        //                    //graph = (VisualGraph)t.GetField("graph", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(comp);
        //                    graph = (VisualGraph)t.GetField("graph").GetValue(comp);
        //                    if (graph != null)
        //                    {
        //                        visualGraphComponent = comp;
        //                        SetVisualGraph(graph);
        //                        return;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}


    }
}