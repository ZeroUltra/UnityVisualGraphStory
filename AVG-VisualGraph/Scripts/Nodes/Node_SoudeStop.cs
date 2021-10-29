using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("停止声音",101)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_SoudeStop : Node_Base
    {
        [Label("声音类型"), AllowNesting]
        public AVGHelper.SoundType soundType = AVGHelper.SoundType.Bgm;

        [Label("名字")]
        [Dropdown(nameof(listAudios)), AllowNesting]
        public string soundName = "";
        private string[] listAudios
        {
            get
            {
                switch (soundType)
                {
                    case AVGHelper.SoundType.Bgm:
                        return (graph as AVGGraph).graphAssets.Bgms;
                    case AVGHelper.SoundType.EnvirSfx:
                        return (graph as AVGGraph).graphAssets.Envirsfx;
                    case AVGHelper.SoundType.Sfx:
                        return (graph as AVGGraph).graphAssets.Sfx;
                }
                return new string[] { AVGHelper.None };
            }
        }
    }
}