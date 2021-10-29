using UnityEngine;
using NaughtyAttributes;
using VisualGraphRuntime;
namespace LJ.VisualAVG
{
    [NodeName("背景",11)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_BG : Node_Base
    {
        public string spBgName;
        public AVGHelper.FXType2 fXType;
    } 
}
