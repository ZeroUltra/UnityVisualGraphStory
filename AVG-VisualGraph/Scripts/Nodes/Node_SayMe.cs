using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("玩家说",24)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_SayMe : Node_Base
    {
        [TextArea(4, 6)]
        public string msg;
        public AudioClip msgAudio;

        public string targetRoleName = "";

        public string enterTalkEmoji = "";

        public string talkEmoji = "";

        public string exitTalkEmoji = "";

        public string talkEndEmoji = "";

        //角色名字
        public string[] listRolesName
        {
            get
            {
               return (graph as AVGGraph).graphAssets.RoleNames;
            }
        }
        //角色表情动画
        public string[] listRoleEmojis { get { return (graph as AVGGraph).graphAssets.GetRoleAnimas((graph as AVGGraph).graphAssets.GetRolePyName(targetRoleName)); } }

    }
}