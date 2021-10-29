using UnityEngine.UI;
/// <summary>
/// 空图 用于遮罩 降低overdraw
/// </summary>
public class EmptyGraphic : Graphic
{
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();
    }
}
