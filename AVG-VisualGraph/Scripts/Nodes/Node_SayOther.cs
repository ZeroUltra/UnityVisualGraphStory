using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("其他说",22)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_SayOther : Node_Base
    {
        public string msg;
        public AudioClip msgAudio;

        public string roleName = "";
   
        public bool showIcon;
    
        public string spName = "";

        public string[] listOtherRoleName { get { return (graph as AVGGraph).graphAssets.OtherRoleNames; } }
        public string[] listSpNames { get { return (graph as AVGGraph).graphAssets.GetOtherRoleSpNames(roleName); } }
    }
}