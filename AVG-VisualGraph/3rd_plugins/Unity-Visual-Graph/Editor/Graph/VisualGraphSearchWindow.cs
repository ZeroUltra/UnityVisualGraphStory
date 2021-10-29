///-------------------------------------------------------------------------------------------------
// author: William Barry
// date: 2020
// Copyright (c) Bus Stop Studios.
///-------------------------------------------------------------------------------------------------
using VisualGraphRuntime;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using LJ.VisualAVG;

namespace VisualGraphInEditor
{
    public class VisualGraphSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow window;
        private VisualGraphView graphView;
        private List<Type> nodeTypes = new List<Type>();
        //private Texture2D indentationIcon;
        private Texture2D lineTexture2d;

        private Dictionary<Type, Texture2D> dictTexture2D;

        public void Configure(EditorWindow window, VisualGraphView graphView)
        {
            this.window = window;

            this.graphView = graphView;

            var result = new List<System.Type>();
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

            DefaultNodeTypeAttribute typeAttrib = graphView.VisualGraph.GetType().GetCustomAttribute<DefaultNodeTypeAttribute>();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                //将所有节点添加到 列表
                foreach (var type in types)
                {
                    if (typeAttrib != null && (type.IsAssignableFrom(typeAttrib.type) == true || type.IsSubclassOf(typeAttrib.type))
                        && type.IsSubclassOf(typeof(VisualGraphNode)) == true
                        && type.IsAbstract == false)
                    {
                        nodeTypes.Add(type);
                    }
                }
            }

            lineTexture2d = new Texture2D(16, 16);
            for (int i = 0; i < lineTexture2d.height; i++)
            {
                if (i == 6)
                {
                    for (int a = 0; a < lineTexture2d.width; a++)
                        lineTexture2d.SetPixel(a, i, Color.white);
                }
                else
                {
                    for (int a = 0; a < lineTexture2d.width; a++)
                        lineTexture2d.SetPixel(a, i, Color.clear);
                }
            }
            lineTexture2d.Apply();

        dictTexture2D = new Dictionary<Type, Texture2D>()
            {
                {typeof(Node_Wait),EditorGUIUtility.IconContent("UnityEditor.ProfilerWindow").image as Texture2D },
                {typeof(Node_BG),EditorGUIUtility.IconContent("RawImage Icon").image as Texture2D },
                {typeof(Node_SayAside),EditorGUIUtility.IconContent("EventSystem Icon").image as Texture2D },
                {typeof(Node_SayHide),EditorGUIUtility.IconContent("EventSystem Icon").image as Texture2D },
                {typeof(Node_SayMe),EditorGUIUtility.IconContent("EventSystem Icon").image as Texture2D },
                {typeof(Node_SayOther),EditorGUIUtility.IconContent("EventSystem Icon").image as Texture2D },
                {typeof(Node_SayRole),EditorGUIUtility.IconContent("EventSystem Icon").image as Texture2D },
                {typeof(Node_PotHide),EditorGUIUtility.IconContent("Avatar Icon").image as Texture2D },
                {typeof(Node_PotShow),EditorGUIUtility.IconContent("Avatar Icon").image as Texture2D },
                {typeof(Node_Menu),EditorGUIUtility.IconContent("BlendTree Icon").image as Texture2D },
                {typeof(Node_MenuResult),EditorGUIUtility.IconContent("BlendTree Icon").image as Texture2D },
                {typeof(Node_AnimaHide),EditorGUIUtility.IconContent("VideoPlayer Icon").image as Texture2D },
                {typeof(Node_AnimaShow),EditorGUIUtility.IconContent("VideoPlayer Icon").image as Texture2D },
                {typeof(Node_SoundPlay),EditorGUIUtility.IconContent("AudioClip Icon").image as Texture2D },
                {typeof(Node_SoudeStop),EditorGUIUtility.IconContent("AudioClip Icon").image as Texture2D },
                {typeof(Node_Interact),EditorGUIUtility.IconContent("StandaloneInputModule Icon").image as Texture2D },
            };
        }

        //创建搜索tree
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"), 0));
            //tree.Add(new SearchTreeGroupEntry(new GUIContent("Nodes"), 1));

            List<(int orderID, string disName, Type Node)> listMenu = new List<(int orderID, string disName, Type node)>();
            //遍历node 菜单
            foreach (var type in nodeTypes)
            {
                if (type.GetCustomAttribute<NodeNameAttribute>() != null)
                {
                    string display_name = type.GetCustomAttribute<NodeNameAttribute>().name;
                    int orderID = type.GetCustomAttribute<NodeNameAttribute>().orderID;
                    listMenu.Add((orderID, display_name, type));
                }
            }
            //排序
            listMenu.Sort((a, b) =>
            {
                var o = a.orderID - b.orderID;
                return o;
            });

            int lastOrderIndex = -10000;
            int index = 0;
            foreach (var item in listMenu)
            {
                if (lastOrderIndex != -10000 && item.orderID - lastOrderIndex >= 10)//超过10 加一个横线
                    tree.Add(new SearchTreeEntry(new GUIContent("——————————————————————————————————————————", lineTexture2d))
                    {
                        level = 1
                    });
                index++;
                //var treeEntry = new SearchTreeEntry(new GUIContent(item.disName,EditorGUIUtility.IconContent("console.infoicon").image as Texture2D));
                var treeEntry = new SearchTreeEntry(GUIContent.none);
                treeEntry.level = 1;
                treeEntry.userData = item.Node;
                treeEntry.content = new GUIContent(item.disName, dictTexture2D[item.Node]);
                tree.Add(treeEntry);
                lastOrderIndex = item.orderID;
            }

            #region 名字多级分组处理 例如:AAA/BB 现在不用了
            //foreach (var type in nodeTypes)
            //{
            //    string display_name = "";
            //    int orderID = 0;
            //    if (type.GetCustomAttribute<NodeNameAttribute>() != null)
            //    {
            //        display_name = type.GetCustomAttribute<NodeNameAttribute>().name;
            //        orderID = type.GetCustomAttribute<NodeNameAttribute>().orderID;
            //    }
            //    else
            //    {
            //        display_name = type.Name;
            //    }
            //    if (display_name.Contains("/"))
            //    {
            //        string[] names = display_name.Split('/');
            //        for (int i = 0; i < names.Length; i++)
            //        {
            //            //最后一个添加数据
            //            if (i == names.Length - 1)
            //            {
            //                tree.Add(new SearchTreeEntry(new GUIContent(names[i], indentationIcon))
            //                {
            //                    level = i + 1,
            //                    userData = type
            //                });
            //            }
            //            //添加group
            //            else
            //            {
            //                bool isExists = tree.Exists(item => item.CompareTo(new SearchTreeGroupEntry(new GUIContent(names[i]), i + 1)) == 0 /*0是相等*/);
            //                //不存在就添加
            //                if (isExists == false)
            //                    tree.Add(new SearchTreeGroupEntry(new GUIContent(names[i]), i + 1));
            //            }
            //        }
            //    }
            //    else
            //    {
            //        tree.Add(new SearchTreeEntry(new GUIContent(display_name, indentationIcon))
            //        {
            //            level = 1,
            //            userData = type
            //        });
            //    }
            //}; 
            #endregion

            //tree.Add(new SearchTreeEntry(new GUIContent("Group", indentationIcon))
            //{
            //    level = 1,
            //    userData = new Group()
            //});

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var mousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent, context.screenMousePosition - window.position.position);
            var graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);
            switch (SearchTreeEntry.userData)
            {
                case Type nodeData:
                    {
                        graphView.CreateNode(graphMousePosition, nodeData);
                        return true;
                    }
                    //case Group group:
                    //    graphView.CreateGroupBlock(graphMousePosition);
                    //    return true;
            }
            return false;
        }
    }
}