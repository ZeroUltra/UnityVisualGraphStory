using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class ImagePlus : Image
{
    private new Collider2D collider;
    public Collider2D Collider
    {
        get
        {
            if (collider == null) collider = GetComponent<Collider2D>();
            return collider;
        }
    }
    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return Collider.OverlapPoint(screenPoint);
    }


    /// <summary>
    /// 设置PolygonCollider2D 边缘大小
    /// </summary>
    [ContextMenu("SetPolygonColliderPoints")]
    private void SetPolygonColliderPoints()
    {
        if (Collider is PolygonCollider2D)
        {
            PolygonCollider2D polygonCollider2D = Collider as PolygonCollider2D;
            List<Vector2[]> pathPointList = new List<Vector2[]>(polygonCollider2D.pathCount);
            for (int i = 0; i < polygonCollider2D.pathCount; i++)
            {
                pathPointList.Add(polygonCollider2D.GetPath(i));
            }
            Debug.Log(pathPointList.Count);
            for (int i = 0; i < pathPointList.Count; i++)
            {
                Vector2[] tempV2s = pathPointList[i];
                for (int j = 0; j < tempV2s.Length; j++)
                {
                    tempV2s[j] = new Vector2(tempV2s[j].x * 100f, tempV2s[j].y * 100f);
                }
                polygonCollider2D.SetPath(i, tempV2s);
            }

            Debug.Log("PolygonCollider2D 设置完成");
        }
    }
}