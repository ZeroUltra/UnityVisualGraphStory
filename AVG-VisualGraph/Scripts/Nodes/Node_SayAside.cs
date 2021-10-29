using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    /// <summary>
    /// 旁白说
    /// </summary>
    [NodeName("旁白说",21)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    [CustomNodeStyle("AVGNode_SayAsideStyle")]
    public class Node_SayAside : Node_Base
    {
        public AsideData[] Datas;
        [System.Serializable]
        public class AsideData
        {
            [TextArea(4, 6)]
            public string msg;
            public AudioClip msgAudio;
        }
    }

}
