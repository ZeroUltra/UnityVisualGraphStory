using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("隐藏立绘",41)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_PotHide : Node_Base
    {
        [Label("姓名"), Dropdown(nameof(listRoles)), AllowNesting]
        public string potname = "";
        [Label("效果"), AllowNesting]
        public AVGHelper.FXType effectType;
        private string[] listRoles { get { return (graph as AVGGraph).graphAssets.RoleNames; } }
    }
}