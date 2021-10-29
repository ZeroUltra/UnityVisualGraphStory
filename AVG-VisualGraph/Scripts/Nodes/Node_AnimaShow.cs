using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;

namespace LJ.VisualAVG
{
    [NodeName("显示动画",80)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
    public class Node_AnimaShow : Node_Base
    {
        [Label("动画类型"), AllowNesting]
        public AVGHelper.AnimaType animaType;

        [Label("动画预制名")]
        [Dropdown(nameof(listAnimaGos)), AllowNesting]
        public string animaName = "";

        [Label("播完自动消失"), AllowNesting]
        public bool autoDestory = false;

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