using VisualGraphRuntime;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace LJ.VisualAVG
{
    [NodeName("选项面板",60)]
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.None)]
    public class Node_Menu : Node_Base
    {
        public int menuID;
        public List<string> options=new List<string>(4);

        public override void Init()
        {
            base.Init();
            WaitSetID();
        }
        private async void WaitSetID()
        {
            await System.Threading.Tasks.Task.Delay(50);
            int id = 100;
            for (int i = 0; i < graph.Nodes.Count(); i++)
            {
                var node = graph.Nodes[i];
                if (node is Node_Menu)
                {
                    if (node != this)
                    {
                        id++;
                    }
                    else this.menuID = id;
                }
            }
        }

        public void SetOutoutPorts()
        {
            if (Outputs.Count() != options.Count)
            {
                Debug.Log(Outputs.Count());
                ////移除多余的
                //for (int i = Ports.Count - 1; i > options.Count - 1; i--)
                //{
                //    if (Ports[i].Direction == VisualGraphPort.PortDirection.Output)
                //        Ports.RemoveAt(i);
                //}
                //添加
                for (int i = Outputs.Count(); i < options.Count; i++)
                {
                   
                    AddPort($"options{i + 1}", VisualGraphPort.PortDirection.Output);
                    (this.graphElement as Node).RefreshPorts();
                    (this.graphElement as Node).RefreshExpandedState();
                }
            }
        }
        public int OutputCount { get { return Outputs.Count(); } }
    }
}