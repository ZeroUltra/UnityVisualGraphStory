using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("隐藏立绘",41)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_PotHide : Node_Base
    {
  
        public string potname = "";
  
        public AVGHelper.FXType effectType;
        public string[] listRoles { get { return (graph as AVGGraph).graphAssets.RoleNames; } }
    }
}