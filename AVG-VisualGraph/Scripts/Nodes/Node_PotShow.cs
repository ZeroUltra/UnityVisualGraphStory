using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("显示立绘",40)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_PotShow : Node_Base
    {
        [Label("姓名"), Dropdown(nameof(listRoles)), AllowNesting]
        public string potname = "";
        [Label("效果"), AllowNesting]
        public AVGHelper.FXType effectType;

        [Label("左右位置"), AllowNesting]
        public AVGHelper.PosType posType = AVGHelper.PosType.Middle;
        [Label("远近位置"), AllowNesting]
        public AVGHelper.DistanceType disType = AVGHelper.DistanceType.Normal;

        private string[] listRoles { get { return (graph as AVGGraph).graphAssets.RoleNames; } }
    }
}