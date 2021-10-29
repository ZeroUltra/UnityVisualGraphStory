using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("角色说",23)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_SayRole : Node_Base
    {
        [TextArea(4, 6)]
        public string msg;
        public AudioClip msgAudio;

        [Label("姓名")]
        [Dropdown(nameof(listRolesName)), AllowNesting]
        public string roleName = "";

        [Header("表情")]
        [Label("进入说话(once)")]
        [Dropdown(nameof(listRoleEmojis)), AllowNesting]
        public string enterTalkEmoji = "";
        [Label("说话表情(loop)")]
        [Dropdown(nameof(listRoleEmojis)), AllowNesting]
        public string talkEmoji = "";

        [Label("退出说话(once)")]
        [Dropdown(nameof(listRoleEmojis)), AllowNesting]
        public string exitTalkEmoji = "";

        [Label("说完表情(loop)")]
        [Dropdown(nameof(listRoleEmojis)), AllowNesting]
        public string talkEndEmoji = "";

        //角色名字
        private string[] listRolesName { get { return (graph as AVGGraph).graphAssets.RoleNames; } }
        //角色表情动画
        private string[] listRoleEmojis { get { return (graph as AVGGraph).graphAssets.GetRoleAnimas((graph as AVGGraph).graphAssets.GetRolePyName(roleName)); } }
    }
}