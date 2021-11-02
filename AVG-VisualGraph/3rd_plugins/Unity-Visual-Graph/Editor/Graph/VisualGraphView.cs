///-------------------------------------------------------------------------------------------------
// author: William Barry
// date: 2020
// Copyright (c) Bus Stop Studios.
///-------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VisualGraphRuntime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace VisualGraphInEditor
{
    /// <summary>
    /// VisualGraph视图界面 继承GraphView
    /// </summary>
    public class VisualGraphView : GraphView, IEdgeConnectorListener
    {
        private readonly Color edgeColor = new Color(0, 0.8f, 0.8f, 0.8f);
        private readonly Color edgeDropColor = new Color(0, 0.6f, 0.6f, 0.6f);
        //private readonly Color edgeDropColor = Color.yellow;

        public VisualGraph VisualGraph { get { return visualGraph; } private set { } }
        public MiniMap Minimap { get; private set; }
        public BlackboardView BlackboardView { get; private set; }
        public Blackboard Blackboard { get { return BlackboardView.blackboard; } private set { } }


        private VisualGraph visualGraph;
        private VisualGraphSearchWindow searchWindow;
        private VisualGraphEditor editorWindow;
        private Orientation orientation;

        // Runtime Type / Editor Type
        private Dictionary<Type, Type> visualGraphNodeLookup = new Dictionary<Type, Type>();
        private Dictionary<Type, Type> visualGraphPortLookup = new Dictionary<Type, Type>();

        public bool activeVisualGraph = false;
        private bool isMouseMove = false;
        private Vector2 mouseUpPos;
        public VisualGraphView(VisualGraphEditor editorWindow)
        {
            this.editorWindow = editorWindow;

            Undo.undoRedoPerformed += OnUndoRedoCallback;

            graphViewChanged = OnGraphChange;


            styleSheets.Add(Resources.Load<StyleSheet>("VisualGraphStyle"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            var contentDrag = new ContentDragger(); //整体内容移动
            contentDrag.activators.Clear();
            contentDrag.activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse }); //换成鼠标右键移动

            this.AddManipulator(contentDrag);
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            //背景Grid
            //var grid = new GridBackground();
            //Insert(0, grid);
            //grid.StretchToParentSize();

            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    CustomNodeViewAttribute nodeAttrib = type.GetCustomAttribute<CustomNodeViewAttribute>();
                    if (nodeAttrib != null && nodeAttrib.type.IsAbstract == false)
                    {
                        visualGraphNodeLookup.Add(nodeAttrib.type, type);
                    }

                    CustomPortViewAttribute portAttrib = type.GetCustomAttribute<CustomPortViewAttribute>();
                    if (portAttrib != null && portAttrib.type.IsAbstract == false)
                    {
                        visualGraphPortLookup.Add(portAttrib.type, type);
                    }
                }
            }

            unserializeAndPaste = OnDuplicate; //Ctrl+D

        }
        public void OnEnable()
        {
            // this.RegisterCallback<MouseDownEvent>(OnMouseDown); //不起作用
            this.RegisterCallback<MouseMoveEvent>(OnMouseMove);  //注册移动事件
            this.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        /// <summary>
        /// 窗口关闭不可见
        /// </summary>
        public void OnDisable()
        {
            // this.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            this.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            this.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }
        private async void OnMouseUp(MouseUpEvent evt)
        {
            mouseUpPos = evt.mousePosition;
            if (evt.button == 1)
            {
                //要加延迟是因为 鼠标抬起 在BuildContextualMenu方法之前 
                //逻辑是 如果鼠标有移动 那么就不响应鼠标右键菜单 
                await System.Threading.Tasks.Task.Delay(50);
                isMouseMove = false;
                //Debug.Log("OnMouseUp"); 
            }
        }
        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (evt.pressedButtons == 2) //必须是右键按下
            {
                //Debug.Log("OnMouseMove");
                isMouseMove = true;
            }
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="operationName"></param>
        /// <param name="data"></param>
        private void OnDuplicate(string operationName, string data)
        {
            if (selection.Count == 1)
            {
                if (selection[0] is VisualGraphNodeView)
                {
                    var snode = (selection[0] as VisualGraphNodeView);
                    var node = CreateNode(snode.GetPosition().position + new Vector2(1, 1) * 50, snode.userData.GetType());
                }
            }
        }

        public void CreateMinimap(float windowWidth)
        {
            Minimap = new MiniMap { anchored = true };
            Minimap.capabilities &= ~Capabilities.Movable;
            Minimap.SetPosition(new Rect(windowWidth - 210, 30, 200, 140));
            Add(Minimap);
        }

        public void CreateBlackboard()
        {
            BlackboardView = new BlackboardView();
            BlackboardView.visualGraphView = this;
            Blackboard.SetPosition(new Rect(10, 30, 250, 300));
            Add(BlackboardView.blackboard);
        }

        #region View OnGUI/Update

        public void OnGUI()
        {
            if (Minimap != null) Minimap.SetPosition(new Rect(contentRect.width - 210, 30, 200, 140));
            //if (Blackboard != null) Blackboard.SetPosition(new Rect(10, 30, Blackboard.style.width.value.value, Blackboard.style.height.value.value));
            //Debug.Log(contentViewContainer.transform.position);

        }

        //public void Update()
        //{
        //    nodes.ForEach(nodeView =>
        //    {
        //        VisualGraphNode node = nodeView.userData as VisualGraphNode;
        //        if (node != null)
        //        {
        //            if (node.editor_ActiveNode)
        //            {
        //                nodeView.AddToClassList("VisualGraphNodeSelected");
        //            }
        //            else
        //            {
        //                nodeView.RemoveFromClassList("VisualGraphNodeSelected");
        //            }
        //        }
        //    });
        //}
        #endregion

        #region Init the Graph

        public void OnUndoRedoCallback()
        {
            SetGraph(visualGraph);
        }

        /// <summary>
        /// Load the Visual Graph into the Editor Graph View
        /// </summary>
        /// <param name="_graph"></param>
        public void SetGraph(VisualGraph _visualGraph)
        {
            // Set the graph to null and clear the edges and nodes before we get going.
            visualGraph = null;
            DeleteElements(graphElements.ToList());
            DeleteElements(nodes.ToList());
            DeleteElements(edges.ToList());
            activeVisualGraph = false;
            BlackboardView?.ClearBlackboard();

            visualGraph = _visualGraph;
            if (visualGraph != null)
            {
                // When the graph is loaded connections need to be remade
                visualGraph.InitializeGraph();

                activeVisualGraph = true;

                GraphOrientationAttribute orientationAttrib = visualGraph.GetType().GetCustomAttribute<GraphOrientationAttribute>();
                Debug.Assert(orientationAttrib != null, $"Graph node requires a GraphOrientationAttribute {visualGraph.GetType().Name}");
                orientation = (orientationAttrib.GrapOrientation == GraphOrientationAttribute.Orientation.Horizontal) ? Orientation.Horizontal : Orientation.Vertical;

                BlackboardView?.SetVisualGraph(visualGraph);

                searchWindow = ScriptableObject.CreateInstance<VisualGraphSearchWindow>();
                searchWindow.Configure(editorWindow, this);
                nodeCreationRequest = context =>
                {
                    SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
                };

                // If the graph doesn't have a start node it's probably the first time we opened it. This means
                // we will create one to get going.
                if (visualGraph.StartingNode == null)
                {
                    VisualGraphNode startingNode = Activator.CreateInstance(typeof(VisualGraphStartNode)) as VisualGraphNode;
                    visualGraph.StartingNode = startingNode;
                    startingNode.name = "Start";
                    startingNode.position = new Vector2(30, 150);

                    VisualGraphPort graphPort = startingNode.AddPort("Next", VisualGraphPort.PortDirection.Output);
                    graphPort.CanBeRemoved = false;
                    visualGraph.Nodes.Add(startingNode);

                    if (startingNode.name == null || startingNode.name.Trim() == "") startingNode.name = UnityEditor.ObjectNames.NicifyVariableName(startingNode.name);
                    if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(visualGraph))) AssetDatabase.AddObjectToAsset(startingNode, visualGraph);

                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(startingNode));
                }

                // Create all other Nodes for the graph
                foreach (var graphNode in visualGraph.Nodes)
                {
                    Node node = AddGraphNode(graphNode);
                    Vector2 pos = new Vector2(node.style.left.value.value, node.style.top.value.value);
                    ((VisualGraphNode)node.userData).position = pos;
                }

                foreach (VisualGraphNode graphNode in visualGraph.Nodes)
                {
                    foreach (VisualGraphPort graphPort in graphNode.Ports)
                    {
                        if (graphPort.Direction == VisualGraphPort.PortDirection.Output)
                        {
                            Port port = graphPort.editor_port as Port;
                            foreach (VisualGraphPort.VisualGraphPortConnection graph_connection in graphPort.Connections)
                            {
                                VisualGraphPort other_port = graph_connection.Node.FindPortByGuid(graph_connection.port_guid);
                                Port other_editor_port = other_port.editor_port as Port;
                                AddElement(port.ConnectTo(other_editor_port));
                            }
                        }
                    }
                }

                //foreach (var group in visualGraph.Groups)
                //{
                //	AddGroupBlock(group);
                //}
            }

        }
        #endregion

        /* Group Block

        //public void CreateGroupBlock(Vector2 position)
        //{
        //    Undo.RecordObject(visualGraph, "Create Node");
        //    VisualGraphGroup block = new VisualGraphGroup()
        //    {
        //        title = "Graph Group",
        //        position = position
        //    };
        //    visualGraph.Add(block);
        //    AddGroupBlock(block);
        //}

        //public void AddGroupBlock(VisualGraphGroup graphGroup)
        //{
        //    var group = new VisualGraphGroupView
        //    {
        //        autoUpdateGeometry = true,
        //        title = graphGroup.title,
        //        userData = graphGroup
        //    };
        //    group.SetPosition(new Rect(graphGroup.position.x, graphGroup.position.y, 300, 200));
        //    AddElement(group);

        //    HashSet<GraphElement> nodes = new HashSet<GraphElement>();
        //    foreach (var node_guid in graphGroup.node_guids)
        //    {
        //        VisualGraphNode node = visualGraph.FindNodeByGuid(node_guid);
        //        if (node != null)
        //        {
        //            nodes.Add(node.graphElement as GraphElement);
        //        }
        //    }
        //    group.CollectElements(nodes, null);
        //}

        */

        #region Node Creation
        /// <summary>
        /// Create a node based off the type. Once the node is created it will be added to the Graph and a View
        /// node will be created and added
        /// </summary>
        /// <param name="position"></param>
        /// <param name="nodeType"></param>
        public Node CreateNode(Vector2 position, Type nodeType)
        {
            //Debug.Log("Create Node");
            Undo.RecordObject(visualGraph, "Create Node");
            VisualGraphNode graphNode = visualGraph.AddNode(nodeType);
            Undo.RegisterCreatedObjectUndo(graphNode, "Create Node");
            graphNode.position = position;

            if (graphNode.name == null || graphNode.name.Trim() == "") graphNode.name = UnityEditor.ObjectNames.NicifyVariableName(nodeType.Name);
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(visualGraph))) AssetDatabase.AddObjectToAsset(graphNode, visualGraph);

            Node node = AddGraphNode(graphNode);

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(graphNode));
            return node;
        }

        /// <summary>
        /// Add the node from the graph to the view based off property and attribute settings
        /// </summary>
        /// <param name="graphNode"></param>
        /// <returns></returns>
        private Node AddGraphNode(VisualGraphNode graphNode)
        {
            // By default we will create all nodes from VisualGraphNodeView
            Type visualNodeType = typeof(VisualGraphNodeView);
            if (visualGraphNodeLookup.ContainsKey(graphNode.GetType()) == true)
            {
                visualNodeType = visualGraphNodeLookup[graphNode.GetType()];
            }

            // Create the Node View based off the type, set the class for styling
            VisualGraphNodeView node = Activator.CreateInstance(visualNodeType) as VisualGraphNodeView;
            node.AddToClassList("VisualGraphNode");
            node.title = GetGraphNodeName(graphNode.GetType());
            node.userData = graphNode;
            node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            // The Editor stores a reference to the graph created;
            graphNode.graphElement = node;

            // If there are extra Styles apply them
            IEnumerable<CustomNodeStyleAttribute> customStyleAttribs = graphNode.GetType().GetCustomAttributes<CustomNodeStyleAttribute>();
            if (customStyleAttribs != null)
            {
                foreach (var customStyleAttrib in customStyleAttribs)
                {
                    try
                    {
                        StyleSheet styleSheet = Resources.Load<StyleSheet>(customStyleAttrib.style);
                        if (styleSheet != null)
                        {
                            node.styleSheets.Add(styleSheet);
                        }
                        else throw new Exception();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Style sheet does not exit: {customStyleAttrib.style}");
                    }
                }
            }

            // Get the Port Dynamics. Base class type already has the attribute so this should never fail
            NodePortAggregateAttribute dynamicsAttrib = graphNode.GetType().GetCustomAttribute<NodePortAggregateAttribute>();
            Debug.Assert(dynamicsAttrib != null, $"Graph node requires a NodePortAggregateAttribute {graphNode.GetType().Name}");

            if (dynamicsAttrib.InputPortAggregate == NodePortAggregateAttribute.PortAggregate.Multiple)
            {
                // Button for input ports
                var button = new Button(() => { CreatePort(node, "Input", VisualGraphPort.PortDirection.Input); })
                {
                    text = "Add Input"
                };
                node.titleButtonContainer.Add(button);
            }
            if (dynamicsAttrib.OutputPortAggregate == NodePortAggregateAttribute.PortAggregate.Multiple)
            {
                var button = new Button(() => { CreatePort(node, "Exit", VisualGraphPort.PortDirection.Output); })
                {
                    text = "Add Exit"
                };
                node.titleButtonContainer.Add(button);
            }

            // Set the node capabilites. The default View node can be overriden
            node.capabilities = node.SetCapabilities(node.capabilities);
            node.style.width = node.default_size.x;
            node.SetPosition(new Rect(graphNode.position, node.default_size));

            // Add the needed ports
            foreach (var graphPort in graphNode.Ports)
            {
                AddPort(graphPort, node);
            }

            // If there are custom elements or drawing, let the derived node handle this
            node.InitNode(graphNode);
            node.DrawNode();
         
            // If the node wants to hide the properties the user must make a View node and set this to false
            if (node.ShowNodeProperties)
            {
                VisualElement divider = new VisualElement();
                divider.style.borderBottomColor = divider.style.borderTopColor = divider.style.borderLeftColor = divider.style.borderRightColor = Color.black;
                divider.style.borderBottomWidth = divider.style.borderTopWidth = divider.style.borderLeftWidth = divider.style.borderRightWidth = 0.5f;
                node.mainContainer.Add(divider);

                VisualElement node_data = new VisualElement();
                node_data.AddToClassList("node_data");
                node.mainContainer.Add(node_data);

                UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor((VisualGraphNode)node.userData);
                IMGUIContainer inspectorIMGUI = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
                node_data.Add(inspectorIMGUI);
            }

            // Finally add the element
            AddElement(node);

            // Refresh the view
            node.RefreshExpandedState();
            node.RefreshPorts();

            return node;
        }

        private string GetGraphNodeName(Type type)
        {
            string display_name = "";
            if (type.GetCustomAttribute<NodeNameAttribute>() != null)
            {
                display_name = type.GetCustomAttribute<NodeNameAttribute>().name;
            }
            else
            {
                display_name = type.Name;
            }
            return display_name;
        }
        #endregion

        #region Port Connections
        /// <summary>
        /// Create a port for the given node based off the direction. Once the port is created for the node in the graph
        /// a port will be added to the view node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="direction"></param>
        public void CreatePort(Node node, string name, VisualGraphPort.PortDirection direction, bool CanBeRemoved = true)
        {

            VisualGraphNode graphNode = node.userData as VisualGraphNode;

            Undo.RecordObject(graphNode, "Add Port to Node");

            VisualGraphPort graphPort = graphNode.AddPort(name, direction);
            graphPort.CanBeRemoved = CanBeRemoved;
            AddPort(graphPort, node);

            EditorUtility.SetDirty(visualGraph);
        }

        /// <summary>
        /// Add a port to the view node
        /// </summary>
        /// <param name="graphPort"></param>
        /// <param name="node"></param>
        private void AddPort(VisualGraphPort graphPort, Node node)
        {
            VisualGraphNode graphNode = node.userData as VisualGraphNode;

            // Determine the direction of the port (In/Out)
            Direction direction = (graphPort.Direction == VisualGraphPort.PortDirection.Input) ? Direction.Input : Direction.Output;

            // Get the capacity of the port (how many connections can this port have)
            PortCapacityAttribute capacityAttrib = graphNode.GetType().GetCustomAttribute<PortCapacityAttribute>();
            Debug.Assert(capacityAttrib != null, $"Graph node requires a PortCapacityAttribute {graphNode.GetType().Name}");
            Port.Capacity capacity = Port.Capacity.Single;
            if (graphPort.Direction == VisualGraphPort.PortDirection.Input)
            {
                capacity = (capacityAttrib.InputPortCapacity == PortCapacityAttribute.Capacity.Single) ? Port.Capacity.Single : Port.Capacity.Multi;
            }
            else
            {
                capacity = (capacityAttrib.OutputPortCapacity == PortCapacityAttribute.Capacity.Single) ? Port.Capacity.Single : Port.Capacity.Multi;
            }

            // Get the data type for the port.
            // TODO: can we optimze/change this to be more dynamic?
            Type port_type = (graphPort.Direction == VisualGraphPort.PortDirection.Input) ? graphNode.InputType : graphNode.OutputType;

            // Create the port based off supplied information
            var port = node.InstantiatePort(orientation, direction, capacity, port_type);
            port.portName = "";// Don't set the name this helps with the view.
            port.userData = graphPort;
            if (direction == Direction.Input)
                newNodeInputPort = port;
            graphPort.editor_port = port;

            // Custom View for ports
            NodePortAggregateAttribute portAggregateAttrib = graphNode.GetType().GetCustomAttribute<NodePortAggregateAttribute>();
            NodePortAggregateAttribute.PortAggregate aggregate = NodePortAggregateAttribute.PortAggregate.None;
            if (graphPort.Direction == VisualGraphPort.PortDirection.Input)
            {
                aggregate = (portAggregateAttrib.InputPortAggregate == NodePortAggregateAttribute.PortAggregate.Single) ? NodePortAggregateAttribute.PortAggregate.Single : NodePortAggregateAttribute.PortAggregate.Multiple;
            }
            else
            {
                aggregate = (portAggregateAttrib.OutputPortAggregate == NodePortAggregateAttribute.PortAggregate.Single) ? NodePortAggregateAttribute.PortAggregate.Single : NodePortAggregateAttribute.PortAggregate.Multiple;
            }

            VisualGraphPortView graphPortView = null;
            if (aggregate == NodePortAggregateAttribute.PortAggregate.Single)
            {
                graphPortView = Activator.CreateInstance(typeof(VisualGraphLabelPortView)) as VisualGraphLabelPortView;
            }
            else
            {
                Type portViewType = null;
                visualGraphPortLookup.TryGetValue(graphPort.GetType(), out portViewType);
                if (portViewType == null)
                {
                    portViewType = typeof(VisualGraphPortView);
                }
                graphPortView = Activator.CreateInstance(portViewType) as VisualGraphPortView;
            }

            graphPortView.CreateView(graphPort);

            port.Add(graphPortView);

            // If the user can remove a port add a button
            if (graphPort.CanBeRemoved)
            {
                var deleteButton = new Button(() => RemovePort(node, port))
                {
                    text = "X"
                };
                port.Add(deleteButton);
            }
            port.AddManipulator(new EdgeConnector<Edge>(this));

            // Put the port in the proper container for the view
            if (direction == Direction.Input)
            {
                node.inputContainer.Add(port);
            }
            else
            {
                node.outputContainer.Add(port);
            }
            port.portColor = graphPort.Connections.Count() > 0 ? edgeColor : edgeDropColor;

            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        /// <summary>
        /// Connect the nodes together through the port Connections
        /// </summary>
        /// <param name="graphView"></param>
        /// <param name="edge"></param>
        public void OnDrop(GraphView graphView, Edge edge)
        {
            edge.input.portColor = edgeColor;
            edge.output.portColor = edgeColor;

            VisualGraphNode graph_input_node = edge.input.node.userData as VisualGraphNode;
            VisualGraphNode graph_output_node = edge.output.node.userData as VisualGraphNode;

            Undo.RecordObjects(new UnityEngine.Object[] { graph_input_node, graph_output_node }, "Add Port to Node");

            VisualGraphPort graph_input_port = edge.input.userData as VisualGraphPort;
            VisualGraphPort graph_output_port = edge.output.userData as VisualGraphPort;

            graph_input_port.Connections.Add(new VisualGraphPort.VisualGraphPortConnection()
            {
                initialized = true,
                Node = edge.output.node.userData as VisualGraphNode,
                port = graph_output_port,
                port_guid = graph_output_port.guid,
                node_guid = graph_output_node.guid
            });
            graph_output_port.Connections.Add(new VisualGraphPort.VisualGraphPortConnection()
            {
                initialized = true,
                Node = edge.input.node.userData as VisualGraphNode,
                port = graph_input_port,
                port_guid = graph_input_port.guid,
                node_guid = graph_input_node.guid
            });

            EditorUtility.SetDirty(visualGraph);
        }


        // private VisualGraphNode newNode = null;
        private Port newNodeInputPort = null;
        private Port lastNodeOutputPort = null;
        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            //  newNode = null;
            newNodeInputPort = null;
            lastNodeOutputPort = edge.output;
            //拖到空白地方
            nodeCreationRequest(new NodeCreationContext() { screenMousePosition = edge.candidatePosition + editorWindow.position.position, target = null, index = -1 });
            WaitCreateNode();
        }

        private async void WaitCreateNode()
        {
            while (newNodeInputPort == null)
            {
                await System.Threading.Tasks.Task.Delay(50);
            }

            var edge = lastNodeOutputPort.ConnectTo(newNodeInputPort);
            edge.input.portColor = edgeColor;
            edge.output.portColor = edgeColor;

            VisualGraphNode graph_input_node = edge.input.node.userData as VisualGraphNode;
            VisualGraphNode graph_output_node = edge.output.node.userData as VisualGraphNode;

            VisualGraphPort graph_input_port = edge.input.userData as VisualGraphPort;
            VisualGraphPort graph_output_port = edge.output.userData as VisualGraphPort;

            graph_input_port.Connections.Add(new VisualGraphPort.VisualGraphPortConnection()
            {
                initialized = true,
                Node = edge.output.node.userData as VisualGraphNode,
                port = graph_output_port,
                port_guid = graph_output_port.guid,
                node_guid = graph_output_node.guid
            });
            graph_output_port.Connections.Add(new VisualGraphPort.VisualGraphPortConnection()
            {
                initialized = true,
                Node = edge.input.node.userData as VisualGraphNode,
                port = graph_input_port,
                port_guid = graph_input_port.guid,
                node_guid = graph_input_node.guid
            });

            AddElement(edge);

            EditorUtility.SetDirty(visualGraph);
        }

        /// <summary>
        /// Remove the given port from the node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="socket"></param>
        public void RemovePort(Node node, Port socket)
        {
            VisualGraphPort socket_port = socket.userData as VisualGraphPort;
            List<Edge> edgeList = edges.ToList();
            foreach (var edge in edgeList)
            {
                VisualGraphPort graphPort = edge.output.userData as VisualGraphPort;
                if (graphPort.guid.Equals(socket_port.guid))
                {
                    RemoveEdge(edge);
                    edge.input.Disconnect(edge);
                    RemoveElement(edge);
                }
            }

            VisualGraphNode graphNode = node.userData as VisualGraphNode;

            Undo.RecordObject(graphNode, "Remove Port");

            graphNode.Ports.Remove(socket_port);

            if (socket.direction == Direction.Input)
            {
                node.inputContainer.Remove(socket);
            }
            else
            {
                node.outputContainer.Remove(socket);
            }
            node.RefreshPorts();
            node.RefreshExpandedState();

            EditorUtility.SetDirty(visualGraph);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }
        #endregion

        #region Graph Changes
        private GraphViewChange OnGraphChange(GraphViewChange change)
        {
            // Return from this if we don't have a Visual Graph. This means we are reseting the
            // graph

            if (visualGraph == null) return change;

            if (change.elementsToRemove != null)
            {
                foreach (GraphElement element in change.elementsToRemove)
                {
                    if (typeof(Edge).IsInstanceOfType(element) == true)
                    {
                        VisualGraphNode graph_input_node = ((Edge)element).input.node.userData as VisualGraphNode;
                        VisualGraphNode graph_output_node = ((Edge)element).output.node.userData as VisualGraphNode;
                        Undo.RecordObjects(new UnityEngine.Object[] { graph_input_node, graph_output_node }, "Add Port to Node");

                        RemoveEdge((Edge)element);
                    }
                    else if (typeof(Node).IsInstanceOfType(element) == true)
                    {
                        RemoveNode((Node)element);
                    }
                    //else if (typeof(Group).IsInstanceOfType(element) == true)
                    //{
                    //    VisualGraphGroup block = ((Group)element).userData as VisualGraphGroup;
                    //    Undo.RecordObjects(new UnityEngine.Object[] { visualGraph }, "Add Port to Node");
                    //    visualGraph.Groups.Remove(block);
                    //}
                }
            }

            if (change.movedElements != null)
            {
                List<VisualGraphNode> movedNodes = new List<VisualGraphNode>();
                foreach (GraphElement element in change.movedElements)
                {
                    if (typeof(Node).IsInstanceOfType(element) == true)
                    {
                        movedNodes.Add((VisualGraphNode)element.userData);
                    }
                }
                Undo.RecordObjects(movedNodes.ToArray(), "Moved VisualGraphNode");

                foreach (GraphElement element in change.movedElements)
                {
                    if (typeof(Node).IsInstanceOfType(element) == true)
                    {
                        movedNodes.Add((VisualGraphNode)element.userData);
                        ((VisualGraphNode)element.userData).position = new Vector2(element.style.left.value.value, element.style.top.value.value);
                    }
                }
            }

            EditorUtility.SetDirty(visualGraph);

            return change;
        }

        private void RemoveNode(Node node)
        {
            VisualGraphNode graphNode = node.userData as VisualGraphNode;
            Undo.RecordObjects(new UnityEngine.Object[] { graphNode, visualGraph }, "Delete Node");
            visualGraph.RemoveNode(graphNode);
            EditorUtility.SetDirty(visualGraph);
            AssetDatabase.SaveAssets();
            Undo.DestroyObjectImmediate(graphNode);
        }

        private void RemoveEdge(Edge edge)
        {
            VisualGraphPort graph_input_port = edge.input.userData as VisualGraphPort;
            VisualGraphPort graph_output_port = edge.output.userData as VisualGraphPort;
            graph_input_port.RemoveConnectionByPortGuid(graph_output_port.guid);
            graph_output_port.RemoveConnectionByPortGuid(graph_input_port.guid);
        }
        #endregion


        #region 鼠标右键菜单
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (isMouseMove) return;
            if (evt.target is GraphView && nodeCreationRequest != null)
            {
                //evt.menu.AppendAction("Create Node", OnContextMenuNodeCreate, DropdownMenuAction.AlwaysEnabled);
                // evt.menu.AppendSeparator();
                //OnContextMenuNodeCreate(evt.menu.);
                SearchWindow.Open(new SearchWindowContext(mouseUpPos+ editorWindow.position.position), searchWindow);
            }
            // base.BuildContextualMenu(evt);
            //Debug.Log(evt.menu.MenuItems()[0].ToString());
            //BuildGroupContextualMenu(evt);

        }
        //protected virtual void BuildGroupContextualMenu(ContextualMenuPopulateEvent evt, int menuPosition = -1)
        //{
        //    if (menuPosition == -1)
        //        menuPosition = evt.menu.MenuItems().Count;
        //    Vector2 position = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
        //    evt.menu.InsertAction(menuPosition, "Create Group", (e) => SearchWindow.Open(new SearchWindowContext(position), searchWindow));
        //}

        void OnContextMenuNodeCreate(DropdownMenuAction a)
        {
            // RequestNodeCreation(null, -1, a.eventInfo.mousePosition);
            if (nodeCreationRequest == null)
                return;

            //GUIView guiView = elementPanel.ownerObject as GUIView;
            //if (guiView == null)
            //    return;
            //Vector2 screenPoint = this as .screenPosition.position + position;
            nodeCreationRequest(new NodeCreationContext() { screenMousePosition = a.eventInfo.mousePosition + editorWindow.position.position, target = null, index = -1 });
        }
        #endregion
    }
}