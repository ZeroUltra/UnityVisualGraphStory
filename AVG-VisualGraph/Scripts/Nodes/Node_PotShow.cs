using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("显示立绘",40)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_PotShow : Node_Base
    {
        public string potname = "";
        public AVGHelper.FXType effectType;
        public AVGHelper.PosType posType = AVGHelper.PosType.Middle;
        public AVGHelper.DistanceType disType = AVGHelper.DistanceType.Normal;

        public string[] listRoles { get { return (graph as AVGGraph).graphAssets.RoleNames; } }
    }
}