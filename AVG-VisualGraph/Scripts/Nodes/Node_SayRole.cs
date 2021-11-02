using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("角色说",23)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_SayRole : Node_Base
    {
        public string msg;
        public AudioClip msgAudio;

        public string roleName;

        public string enterTalkEmoji; //进入表情

        public string talkEmoji;//说话表情

        public string exitTalkEmoji; //退出表情

        public string talkEndEmoji; //结束后表情

        //角色名字
        public string[] listRolesName { get { return (graph as AVGGraph).graphAssets.RoleNames; } }
        //角色表情动画
        public string[] listRoleEmojis { get { return (graph as AVGGraph).graphAssets.GetRoleAnimas((graph as AVGGraph).graphAssets.GetRolePyName(roleName)); } }
    }
}