using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("等待",1)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]

    public class Node_Wait : Node_Base
    {
        [Label("等待时间"), AllowNesting]
        public float waitTimer;
    }
}