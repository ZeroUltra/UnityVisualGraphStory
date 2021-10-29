//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.IO;
//using System.Text;
//using UnityEditor.Presets;
//using NaughtyAttributes;

///// <summary>
///// 设置资源格式
/////// </summary>
//[CreateAssetMenu(fileName = "AssetFormatSetting", menuName = "资源格式设置", order = -20)]
//public class SetAssetFormat : ScriptableObject
//{
//    //  const string ASSETS = "Assets/";

//    [InfoBox("", type: EInfoBoxType.Normal)]
//    [Header("说明: 选择资源路径(相对于Assets目录,eg:Assets/aa/b/cc),设置资源Preset")]
//    public List<AssetData> listAssetDatas = new List<AssetData>();

//    [Button]
//    private void Apply()
//    {
//        for (int i = 0; i < listAssetDatas.Count; i++)
//        {
//            AssetData assetData = listAssetDatas[i];
//            //找到路径下所有资源guid
//            string[] guids = AssetDatabase.FindAssets("", new string[] { AssetDatabase.GetAssetPath(assetData.assetPath) });
//            foreach (var guidItem in guids)
//            {
//                //guid->资源路径
//                string aPath = AssetDatabase.GUIDToAssetPath(guidItem);
//                if (Path.GetExtension(aPath) != ".preset")
//                {
//                    //获取资源的导入器
//                    var importer = AssetImporter.GetAtPath(aPath);
//                    //如果可以应用
//                    if (assetData.preset.CanBeAppliedTo(importer) && !assetData.preset.DataEquals(importer))
//                    {
//                        if (assetData.filterLabel.Length > 0)
//                        {
//                            //获取该资源的lable
//                            var labels = AssetDatabase.GetLabels(new GUID(guidItem));
//                            if (labels.Length > 0)
//                            {
//                                if (StrsAContainsStrsB(assetData.filterLabel, labels))
//                                {
//                                    //跳过这个
//                                    continue;
//                                }
//                            }
//                            else
//                            {
//                                assetData.preset.ApplyTo(importer);
//                                Debug.Log($"设置了该资源格式: {RichString.ColorWhite(aPath)}");
//                                EditorUtility.SetDirty(importer);
//                                importer.SaveAndReimport();
//                            }
//                        }
//                        else
//                        {
//                            assetData.preset.ApplyTo(importer);
//                            Debug.Log($"设置了该资源格式: {RichString.ColorWhite(aPath)}");
//                            EditorUtility.SetDirty(importer);
//                            importer.SaveAndReimport();
//                        }
//                    }
//                }
//            }
//        }
//        Debug.Log(RichString.ColorYellow("设置完成"));
//    }

//    /// <summary>
//    /// 字符串组A是否包含 字符串组B中任一字符串
//    /// </summary>
//    /// <param name="strsA"></param>
//    /// <param name="strsB"></param>
//    /// <returns></returns>
//    private bool StrsAContainsStrsB(string[] strsA, string[] strsB)
//    {
//        bool isExists = false;
//        foreach (var itemA in strsA)
//        {
//            isExists = System.Array.Exists(strsB, itemB => itemB == itemA);
//            if (isExists) return true;
//        }
//        return isExists;
//    }

//    private void OnValidate()
//    {
//        for (int i = 0; i < listAssetDatas.Count; i++)
//        {
//            AssetData assetData = listAssetDatas[i];
//            assetData.fullPath = AssetDatabase.GetAssetPath(assetData.assetPath);
//        }
//        EditorUtility.SetDirty(this);
//    }

//    [System.Serializable]
//    public class AssetData
//    {
//        public Preset preset;
//        public Object assetPath;
//        [ReadOnly, AllowNesting] public string fullPath;
//        [Tooltip("过滤lable,资源右下角设置")] public string[] filterLabel;
//    }
//}
