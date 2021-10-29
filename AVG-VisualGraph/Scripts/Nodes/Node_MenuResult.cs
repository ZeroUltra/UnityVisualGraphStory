using VisualGraphRuntime;

namespace LJ.VisualAVG
{
    [NodeName("选项结果",61)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.None)]
    public class Node_MenuResult : Node_Base
    {
        public int targetMenuID; //目标选项ID
    }
}