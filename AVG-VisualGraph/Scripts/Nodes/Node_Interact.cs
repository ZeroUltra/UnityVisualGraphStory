using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("交互",130)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_Interact : Node_Base
    {
        [Label("交互预制")]
        [Dropdown(nameof(InterNames)), AllowNesting]
        public string interactName = "";

        [Label("自动销毁->下一条"), AllowNesting]
        public bool isAutoDestory = false;
        private string[] InterNames { get { return (graph as AVGGraph).graphAssets.InterNames; } }
    }
}