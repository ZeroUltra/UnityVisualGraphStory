using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using VisualGraphRuntime;
namespace VisualGraphInEditor
{
    public class VisualGraphNodeView : Node
    {
        [HideInInspector] public virtual Vector2 default_size => new Vector2(250, 150);
        [HideInInspector] public virtual bool ShowNodeProperties => true;

        [HideInInspector] public VisualGraphNode node;

        public virtual void DrawNode()
        {
            
        }

        public virtual void InitNode(VisualGraphNode graphNode)
        {
            node = graphNode;
        }

        public virtual Capabilities SetCapabilities(Capabilities capabilities)
        {
            return capabilities;
        }
    }
}
