using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class EditorTools
{
    [MenuItem("Tools/检查所有Story预制资源")]
    static void CheckAllPrefab()
    {
        string appPath = Application.dataPath + "/C-Game-Story/StoryWork/story";

        FileInfo[] files = new DirectoryInfo(appPath).GetFiles("*.prefab", SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++)
        {
            string assetpath = files[i].FullName.Remove(0, Application.dataPath.Length - 6);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetpath);
            if (PrefabUtility.IsPartOfPrefabAsset(go))
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) > 0)
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                }
                //删除AutoGenerateUGUI
                if (go.TryGetComponent<AutoGenerateUGUI>(out AutoGenerateUGUI auto))
                {
                    GameObject.DestroyImmediate(go.gameObject.GetComponent<AutoGenerateUGUI>());
                }

                if (go.name.StartsWith("hd") == false)
                {
                    if (!go.TryGetComponent(out StoryAnimaHelper storyAnimaHelper))
                    {
                        Debug.LogError($"该动画预制体{go.name} 没有绑定 [StoryAnimaHelper] 已自动添加");
                        go.gameObject.AddComponent<StoryAnimaHelper>();
                    }

                    if (!go.TryGetComponent(out CanvasGroup canvasGroup))
                    {
                        Debug.LogError($"该动画预制体{go.name} 没有绑定 [CanvasGroup] 已自动添加");
                        CanvasGroup cg = go.gameObject.AddComponent<CanvasGroup>();
                        cg.interactable = cg.blocksRaycasts = false;
                    }
                }
                else
                {
                    if (!go.TryGetComponent(out StoryInteractHelper StoryInteractHelper))
                    {
                        Debug.LogError($"该动画预制体{go.name} 没有绑定 [StoryInteractHelper] 已自动添加");
                        go.gameObject.AddComponent<StoryInteractHelper>();
                    }
                    if (!go.TryGetComponent(out CanvasGroup canvasGroup))
                    {
                        Debug.LogError($"该动画预制体{go.name} 没有绑定 [CanvasGroup] 已自动添加");
                        CanvasGroup cg = go.gameObject.AddComponent<CanvasGroup>();
                        cg.interactable = cg.blocksRaycasts = true;
                    }
                }
            }
        }
        FileInfo[] filesanima = new DirectoryInfo(appPath).GetFiles("*.anim", SearchOption.AllDirectories);
        for (int i = 0; i < filesanima.Length; i++)
        {
            string assetpath = filesanima[i].FullName.Remove(0, Application.dataPath.Length - 6);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetpath);
            AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(clip);
            if (clipSetting.loopTime)
            {
                clipSetting.loopTime = false;
                Debug.LogError($"该动画{AssetDatabase.GetAssetPath(clip)}: {clip.name} 是循环动画 已自动设置");
                AnimationUtility.SetAnimationClipSettings(clip, clipSetting);
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("检查完毕!!!");
    }

    [MenuItem("Assets/设置图片格式", priority = -100)]
    static void SetTextureFormat()
    {
        List<string> listPath = new List<string>();
        var objs = Selection.objects;
        for (int i = 0; i < objs.Length; i++)
        {
            string p = AssetDatabase.GetAssetPath(objs[i]);
            if (!Path.HasExtension(p))
            {
                Debug.Log($"<color=yellow>set folder:</color>{p}");
                p = p.Replace("Assets", "");
                listPath.Add(Application.dataPath + p);
            }
            else if (Path.GetExtension(p) == ".png" || Path.GetExtension(p) == ".jpg")
            {
                Debug.Log($"<color=yellow>set texture:</color>{p}");
                TextureImporter importer = AssetImporter.GetAtPath(p) as TextureImporter;
                SetTextureFormat(importer);
            }
        }

        List<FileInfo> listFile = new List<FileInfo>();
        foreach (var itemp in listPath)
        {
            listFile.AddRange(new DirectoryInfo(itemp).GetFiles("*.png", SearchOption.AllDirectories));
            listFile.AddRange(new DirectoryInfo(itemp).GetFiles("*.jpg", SearchOption.AllDirectories));
        }

        for (int i = 0; i < listFile.Count; i++)
        {
            string assetpath = listFile[i].FullName.Remove(0, Application.dataPath.Length - 6);
            TextureImporter importer = AssetImporter.GetAtPath(assetpath) as TextureImporter;
            SetTextureFormat(importer);
        }
        AssetDatabase.Refresh();
        Debug.Log("处理完成");
    }
    static void SetTextureFormat(TextureImporter textureImporter)
    {
        //根据路径获得文件夹目录，设置图集的packagingTag
        textureImporter.maxTextureSize = 2048;
        textureImporter.mipmapEnabled = false;
        textureImporter.isReadable = false;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.wrapMode = TextureWrapMode.Clamp;
        textureImporter.textureCompression = TextureImporterCompression.Compressed;

        //Android端单独设置
        TextureImporterPlatformSettings setting_android = new TextureImporterPlatformSettings();
        setting_android.maxTextureSize = 2048;
        setting_android.overridden = true;
        setting_android.name = "Android";
        setting_android.compressionQuality = 80;
        //根据是否有透明度，选择RGBA还是RGB
        //if (textureImporter.DoesSourceTextureHaveAlpha())
        //    setting_android.format = TextureImporterFormat.ETC2_RGBA8;
        //else
        //    setting_android.format = TextureImporterFormat.ETC2_RGB4;
        setting_android.format = TextureImporterFormat.ETC2_RGBA8Crunched;
        textureImporter.SetPlatformTextureSettings(setting_android);

        //IOS端单独设置
        TextureImporterPlatformSettings setting_iphone = new TextureImporterPlatformSettings();
        setting_iphone.maxTextureSize = 2048;
        setting_iphone.overridden = true;
        setting_iphone.name = "iOS";
        setting_iphone.compressionQuality = 80;

        ////根据是否有透明度，选择RGBA还是RGB
        //if (textureImporter.DoesSourceTextureHaveAlpha())
        //    setting_android.format = TextureImporterFormat.ASTC_RGBA_6x6;
        //else
        //    setting_android.format = TextureImporterFormat.ASTC_6x6;
        setting_iphone.format = TextureImporterFormat.ETC2_RGBA8Crunched;
        textureImporter.SetPlatformTextureSettings(setting_iphone);

        TextureImporterPlatformSettings setting_pc = new TextureImporterPlatformSettings();
        setting_pc.maxTextureSize = 2048;
        setting_pc.compressionQuality = 80;

        setting_pc.overridden = true;
        setting_pc.name = "Standalone";
        ////根据是否有透明度，选择RGBA还是RGB
        //if (textureImporter.DoesSourceTextureHaveAlpha())
        //    setting_android.format = TextureImporterFormat.ASTC_RGBA_6x6;
        //else
        //    setting_android.format = TextureImporterFormat.ASTC_6x6;
        setting_pc.format = TextureImporterFormat.DXT5Crunched;

        textureImporter.SetPlatformTextureSettings(setting_pc);
        textureImporter.SaveAndReimport();
    }


    //[MenuItem("Assets/保存剧情配置表", priority = -99)]
    //static void StoryToCode()
    //{
    //    var obj = Selection.activeObject;
    //    if (obj is Graph_AVG)
    //    {
    //        var avg = obj as Graph_AVG;
    //        if (!string.IsNullOrEmpty(avg.savePath))
    //        {
    //            try
    //            {
    //                string avgPath = Application.dataPath + "/" + avg.savePath;
    //                File.WriteAllText(avgPath, (obj as Graph_AVG).ToCode());
    //                bool isOpen = !EditorUtility.DisplayDialog("Info", "保存成功!!!", "OK", "打开保存文件");

    //                AssetDatabase.Refresh();
    //                if (isOpen)
    //                {
    //                    System.Diagnostics.Process.Start(avgPath);
    //                }
    //            }
    //            catch (System.Exception e)
    //            {
    //                EditorUtility.DisplayDialog("Error ", $"保存错误,请检查 {e.ToString()}", "OK");
    //            }
    //        }
    //        else
    //        {
    //            EditorUtility.DisplayDialog("Error ", $"该 [{avg.name}] 剧情配置没有保存路径", "OK");
    //        }
    //    }
    //}

    //[MenuItem("Tools/保存所有剧情配置表")]
    //static void AllStoryToCode()
    //{
    //    string[] guids = AssetDatabase.FindAssets("t:scriptableobject", new[] { "Assets/C-Game-Story/StoryWork/story" });
    //    foreach (string item in guids)
    //    {
    //        string aPath = AssetDatabase.GUIDToAssetPath(item);
    //        var obj = AssetDatabase.LoadMainAssetAtPath(aPath);
    //        if (obj is Graph_AVG)
    //        {
    //            var avg = obj as Graph_AVG;
    //            string avgPath = Application.dataPath + "/" + avg.savePath;
    //            if (!string.IsNullOrEmpty(avgPath))
    //            {
    //                if (File.Exists(avgPath))
    //                {
    //                    try
    //                    {
    //                        File.WriteAllText(avgPath, (obj as Graph_AVG).ToCode());
    //                    }
    //                    catch (System.Exception e)
    //                    {
    //                        Debug.LogError($"保存错误,请检查 {e.ToString()}");
    //                    }
    //                }
    //                else
    //                {
    //                    Debug.LogError($"[{obj.name}] 不存在保存路径 需要重新配置");
    //                }
    //            }
    //            else
    //            {
    //                Debug.LogError($"该[{avg.name}] 剧情配置没有保存路径");
    //            }
    //        }
    //    }
    //    EditorUtility.DisplayDialog("Info ", $"保存完成,查看信息", "OK");
    //}
}
