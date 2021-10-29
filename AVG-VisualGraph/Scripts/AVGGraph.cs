using UnityEngine;
using VisualGraphRuntime;

namespace LJ.VisualAVG
{
    [CreateAssetMenu(fileName = "new AVG Graph", menuName = "Create AVG Assets/GraphAssets", order = -11)]

    [DefaultNodeType(typeof(Node_Base))]
    public class AVGGraph : VisualGraph
    {
        public AVGGraphAssets graphAssets;
    }

}