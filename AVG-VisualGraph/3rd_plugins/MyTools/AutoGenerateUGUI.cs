#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEditor;
using System.IO;

public class AutoGenerateUGUI : MonoBehaviour
{
    public Object 文件路径;
    public Sprite 图集;
    private Dictionary<Sprite, Rect> dictSpDatas = new Dictionary<Sprite, Rect>();
    [Button("自动布局(文件路径)")]
    public void AutoBuild()
    {
        if (文件路径 == null) return;
        ClearObj();
        string appPath = Application.dataPath.Remove(Application.dataPath.Length - 6, 6);
        string path = AssetDatabase.GetAssetPath(文件路径) + "/";
        path = appPath + path;
        FileInfo[] files = new DirectoryInfo(path).GetFiles("*.png");

        foreach (var item in files)
        {
            var sp = AssetDatabase.LoadAssetAtPath<Sprite>(item.FullName.Remove(0, appPath.Length));
            dictSpDatas.Add(sp, new Rect());
        }
        FileInfo jsonData = null;
        try
        {
            jsonData = new DirectoryInfo(path).GetFiles("*.json")[0];
        }
        catch (System.Exception e)
        {
            Debug.LogError("txt 读取错误:" + e.ToString());
        }
        using (StringReader sr = new StringReader(File.ReadAllText(jsonData.FullName)))
        {
            while (sr.Peek() != -1)
            {
                //读到="7":{"7":{"x":540,"y":960,"width":1080,"height":1920}},
                string data = sr.ReadLine();

                var dataStrs = data.Split(':');
                int count = dataStrs.Length;
                //Debug.Log(strs[0].Trim('"'));
                string secondStr = dataStrs[1].Remove(0, 1);
                //Debug.Log(secondStr.Trim('"'));
                //对比
                string str1 = dataStrs[0].Trim('"');
                string str2 = secondStr.Trim('"');
                if (str1 == str2)
                {
                    Sprite targetSP = null;
                    Rect targetRect = new Rect();
                    foreach (var item in dictSpDatas)
                    {
                        if (item.Key.name == str1)
                        {
                            targetSP = item.Key;
                            string jsonStr = "";
                            for (int i = 2; i < count; i++)
                            {
                                jsonStr += dataStrs[i] + ":";
                            }
                            jsonStr = jsonStr.Remove(jsonStr.Length - 3, 3);
                            var rect = DataRect.FormJson(jsonStr);
                            targetRect = new Rect(rect.x, rect.y, rect.width, rect.height);
                        }
                    }
                    dictSpDatas[targetSP] = new Rect(targetRect.x, targetRect.y, targetRect.width, targetRect.height);
                }
            }
        }
        SetSpritePos();
    }

    [Button("自动布局(图集)")]
    private void LoadFormSprite()
    {
        if (图集 == null) return;
        ClearObj();
        string appPath = Application.dataPath.Remove(Application.dataPath.Length - 6, 6);
        string atlaspath = AssetDatabase.GetAssetPath(图集);
        string jsonpath = appPath + atlaspath;

        FileInfo jsonData = null;
        try
        {
            jsonData = new DirectoryInfo(Path.GetDirectoryName(jsonpath)).GetFiles("*.json")[0];
        }
        catch (System.Exception e)
        {
            Debug.LogError("json 读取错误:" + e.ToString());
        }
        //读取子文件信息
        var sps = AssetDatabase.LoadAllAssetRepresentationsAtPath(atlaspath);
        foreach (var item in sps)
        {
            dictSpDatas.Add(item as Sprite, new Rect());
        }
        using (StringReader sr = new StringReader(File.ReadAllText(jsonData.FullName)))
        {
            while (sr.Peek() != -1)
            {
                //读到="7":{"7":{"x":540,"y":960,"width":1080,"height":1920}},
                string data = sr.ReadLine();

                var dataStrs = data.Split(':');
                int count = dataStrs.Length;
                //Debug.Log(strs[0].Trim('"'));
                string secondStr = dataStrs[1].Remove(0, 1);
                //Debug.Log(secondStr.Trim('"'));
                //对比
                string str1 = dataStrs[0].Trim('"');
                string str2 = secondStr.Trim('"');
                if (str1 == str2)
                {
                    Sprite targetSP = null;
                    Rect targetRect = new Rect();
                    foreach (var item in dictSpDatas)
                    {
                        if (item.Key.name == str1)
                        {
                            targetSP = item.Key;
                            string jsonStr = "";
                            for (int i = 2; i < count; i++)
                            {
                                jsonStr += dataStrs[i] + ":";
                            }
                            jsonStr = jsonStr.Remove(jsonStr.Length - 3, 3);
                            var rect = DataRect.FormJson(jsonStr);
                            targetRect = new Rect(rect.x, rect.y, rect.width, rect.height);
                        }
                    }
                    dictSpDatas[targetSP] = new Rect(targetRect.x, targetRect.y, targetRect.width, targetRect.height);
                }
            }
        }
        SetSpritePos();
    }

    private void ClearObj()
    {
        if (transform.childCount > 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
            (transform as RectTransform).anchoredPosition = Vector2.zero;
        (transform as RectTransform).sizeDelta = new Vector2(1080, 2490);
        dictSpDatas.Clear();
    }
    private void SetSpritePos()
    {
        //设置位置
        foreach (var item in dictSpDatas)
        {
            Image img = new GameObject(item.Key.name).AddComponent<Image>();
            img.raycastTarget = false;
            img.maskable = false;
            img.sprite = item.Key;
            img.transform.SetParent(this.transform);
            img.rectTransform.anchorMin = img.rectTransform.anchorMax = Vector2.zero;
            img.rectTransform.sizeDelta = new Vector2(item.Value.width,item.Value.height);
            img.rectTransform.anchoredPosition = new Vector2(item.Value.x, item.Value.y);
            img.transform.SetAsFirstSibling();
            img.rectTransform.anchorMin = img.rectTransform.anchorMax = Vector2.one*0.5f;
        }
    }
}
[System.Serializable]
public class DataRect
{
    /// <summary>
    /// 
    /// </summary>
    public int x;
    /// <summary>
    /// 
    /// </summary>
    public int y;
    /// <summary>
    /// 
    /// </summary>
    public int width;
    /// <summary>
    /// 
    /// </summary>
    public int height;

    public static DataRect FormJson(string json)
    {
        return JsonUtility.FromJson<DataRect>(json);
    }
}
#endif