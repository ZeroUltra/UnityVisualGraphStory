using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("隐藏动画",81)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_AnimaHide : Node_Base
    {
        [Label("动画类型"), AllowNesting]
        public AVGHelper.AnimaType animaType;

        [Label("名字")]
        [Dropdown(nameof(listAnimaGos)), AllowNesting]
        public string animaName = "";

        private string[] listAnimaGos
        {
            get
            {
                switch (animaType)
                {
                    case AVGHelper.AnimaType.Intro:
                        return (graph as AVGGraph).graphAssets.listAnimaIntroNames;
                    case AVGHelper.AnimaType.Img:
                        return (graph as AVGGraph).graphAssets.listAnimaImgNames;
                    case AVGHelper.AnimaType.Cg:
                        return (graph as AVGGraph).graphAssets.listAnimaCgNames;
                }
                return new string[] { AVGHelper.None };
            }
        }
    }
}