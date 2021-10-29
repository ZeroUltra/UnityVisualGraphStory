using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("其他说",22)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_SayOther : Node_Base
    {
        [TextArea(4, 6)]
        public string msg;
        public AudioClip msgAudio;

        [Label("姓名")]
        [Dropdown(nameof(listOtherRoleName)), AllowNesting]
        public string roleName = "";

        [Label("是否展示头像"), AllowNesting]
        public bool showIcon;

        [Label("头像")]
        [Dropdown(nameof(listSpNames)), ShowIf(nameof(showIcon)), AllowNesting]
        public string spName = "";

        private string[] listOtherRoleName { get { return (graph as AVGGraph).graphAssets.OtherRoleNames; } }
        private string[] listSpNames { get { return (graph as AVGGraph).graphAssets.GetOtherRoleSpNames(roleName); } }
    }
}