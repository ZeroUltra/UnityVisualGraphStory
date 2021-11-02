using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
namespace LJ.VisualAVG
{
    /// <summary>
    /// 旁白说
    /// </summary>
    [NodeName("旁白说",21)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_SayAside : Node_Base
    {
        public List<AsideData> Datas;
        [System.Serializable]
        public class AsideData
        {
            public string msg;
            public AudioClip msgAudio;
        }
    }

}
