#define AVGEDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;


namespace LJ.VisualAVG
{
    /// <summary>
    /// 剧情编辑器资源管理
    /// </summary>
    [CreateAssetMenu(fileName = "new AVG Assets", menuName = "Create AVG Assets/DataAssets", order = -10)]
    public class AVGGraphAssets : ScriptableObject
    {
        [Header("玩家名字")]
        public string userName;

        #region 背景图
        [Header("背景图")]
        public List<Sprite> listBg = new List<Sprite>();
        public string[] BgNames
        {
            get
            {
                string[] bgNames = new string[listBg.Count];
                for (int i = 0; i < bgNames.Length; i++)
                {
                    bgNames[i] = listBg[i].name;
                }
                if (bgNames.Length > 0)
                    return bgNames;
                else return new string[] { AVGHelper.None };
            }
        }
        public Sprite GetBgSprite(string bgName)
        {
            for (int i = 0; i < listBg.Count; i++)
            {
                if (listBg[i].name == bgName)
                    return listBg[i];
            }
            return null;
        }
        #endregion

        #region 其他角色名
        [Header("其他角色名")]
        public RoleIconEmoji[] listOtherRole;
        public string[] OtherRoleNames
        {
            get
            {
                string[] roles = new string[listOtherRole.Length];
                for (int i = 0; i < roles.Length; i++)
                {
                    roles[i] = listOtherRole[i].otherRoleName;
                }
                if (roles.Length > 0)
                    return roles;
                else return new string[] { AVGHelper.None };
            }
        }
        /// <summary>
        /// 获取其他角色头像表情图
        /// </summary>
        /// <param name="otherRoleName"></param>
        /// <returns></returns>
        public string[] GetOtherRoleSpNames(string otherRoleName)
        {
            for (int i = 0; i < listOtherRole.Length; i++)
            {
                if (listOtherRole[i].otherRoleName == otherRoleName)
                {
                    string[] spNames = new string[listOtherRole[i].spIconEmojis.Length];
                    for (int a = 0; a < spNames.Length; a++)
                    {
                        spNames[a] = listOtherRole[i].spIconEmojis[a].name;
                    }
                    if (spNames.Length > 0)
                        return spNames;
                    else return new string[] { AVGHelper.None };
                }
            }
            return new string[] { AVGHelper.None };
        }
        public Sprite GetOtherSpIcon(string otherRoleName, string spName)
        {
            foreach (var item in listOtherRole)
            {
                if (otherRoleName == item.otherRoleName)
                {
                    for (int i = 0; i < item.spIconEmojis.Length; i++)
                    {
                        if (item.spIconEmojis[i].name == spName)
                            return item.spIconEmojis[i];
                    }
                }
            }
            return null;
        }
        #endregion

        #region CG,插图条漫动画文件
        [Header("各种动画")]
        public Animations animations;
        public GameObject GetAnimaGo(AVGHelper.AnimaType animatype, string name)
        {
            switch (animatype)
            {
                case AVGHelper.AnimaType.Intro:
                    return System.Array.Find<GameObject>(animations.anima_Intro, (item) => item.name == name);
                case AVGHelper.AnimaType.Img:
                    return System.Array.Find<GameObject>(animations.anima_Img, (item) => item.name == name);
                case AVGHelper.AnimaType.Cg:
                    return System.Array.Find<GameObject>(animations.anima_Cg, (item) => item.name == name);
            }
            return null;
        }
        public string[] listAnimaIntroNames
        {
            get
            {
                string[] animaNames = new string[animations.anima_Intro.Length];
                for (int i = 0; i < animaNames.Length; i++)
                {
                    animaNames[i] = animations.anima_Intro[i].name;
                }
                if (animaNames.Length > 0)
                    return animaNames;
                else return new string[] { AVGHelper.None };
            }
        }
        public string[] listAnimaImgNames
        {
            get
            {
                string[] animaNames = new string[animations.anima_Img.Length];
                for (int i = 0; i < animaNames.Length; i++)
                {
                    animaNames[i] = animations.anima_Img[i].name;
                }
                if (animaNames.Length > 0)
                    return animaNames;
                else return new string[] { AVGHelper.None };
            }
        }
        public string[] listAnimaCgNames
        {
            get
            {
                string[] animaNames = new string[animations.anima_Cg.Length];
                for (int i = 0; i < animaNames.Length; i++)
                {
                    animaNames[i] = animations.anima_Cg[i].name;
                }
                if (animaNames.Length > 0)
                    return animaNames;
                else return new string[] { AVGHelper.None };
            }
        }
        #endregion

        #region 可交互类型
        [Header("可交互类型")]
        public GameObject[] interactGo;
        public string[] InterNames
        {
            get
            {
                string[] bgNames = new string[interactGo.Length];
                for (int i = 0; i < bgNames.Length; i++)
                {
                    bgNames[i] = interactGo[i].name;
                }
                if (bgNames.Length > 0)
                    return bgNames;
                else return new string[] { AVGHelper.None };
            }
        }

        public GameObject GetInterGoByName(string name)
        {
            for (int i = 0; i < interactGo.Length; i++)
            {
                if (interactGo[i].name == name)
                    return interactGo[i];
            }
            return null;
        }
        #endregion

        #region 角色名
        [Header("角色名字")]
        public RoleName[] listRoleName;

        //所有角色中文名
        public string[] RoleNames
        {
            get
            {
                string[] zhusernames = new string[listRoleName.Length + 1];
                zhusernames[0] = "None";
                for (int i = 1; i < zhusernames.Length; i++)
                {
                    zhusernames[i] = listRoleName[i - 1].zhName;
                }
                return zhusernames;
            }
        }
        public string GetRolePyName(string zhName)
        {
            for (int i = 0; i < listRoleName.Length; i++)
            {
                if (listRoleName[i].zhName == zhName)
                    return listRoleName[i].pyName;
            }
            return null;
        }
        public string GetRoleZHName(string pyName)
        {
            for (int i = 0; i < listRoleName.Length; i++)
            {
                if (listRoleName[i].pyName == pyName)
                    return listRoleName[i].zhName;
            }
            return null;
        }

        public string[] GetRoleAnimas(string pyName)
        {
            for (int i = 0; i < listRoleName.Length; i++)
            {
                if (listRoleName[i].pyName == pyName)
                    return listRoleName[i].potAnimas;
            }
            return new string[] { AVGHelper.None };
        }
        public GameObject GetRolePotByPyName(string pyName)
        {
            for (int i = 0; i < listRoleName.Length; i++)
            {
                if (listRoleName[i].pot.name == pyName)
                    return listRoleName[i].pot;
            }
            return null;
        }

        [NaughtyAttributes.Button("更新角色动画")]
        public void UpdateRoleAnimas()
        {
            for (int i = 0; i < listRoleName.Length; i++)
            {
                var item = listRoleName[i];
                if (item.pyName.IsNullOrEmtryOrNone()) continue;
                var spine = GetRolePotByPyName(item.pyName).GetComponentInChildren<Spine.Unity.SkeletonGraphic>();
                if (spine != null)
                {
                    List<string> emojis = new List<string>();
                    emojis.Add(AVGHelper.None);
                    if (spine.SkeletonData != null)
                    {
                        foreach (var anima in spine.SkeletonData.Animations)
                        {
                            emojis.Add(anima.Name);
                        }
                        item.potAnimas = emojis.ToArray();
                    }
                }
            }
        }
        #endregion

        #region 声音
        [Header("声音")]
        public Audios audios;

        public string[] Bgms
        {
            get
            {
                if (audios.bgm != null)
                {
                    string[] bgms = new string[audios.bgm.Length];
                    for (int i = 0; i < audios.bgm.Length; i++)
                    {
                        bgms[i] = audios.bgm[i].name;
                    }
                    if (bgms.Length > 0)
                        return bgms;
                    else return new string[] { AVGHelper.None };
                }
                else return new string[] { AVGHelper.None };
            }
        }
        public string[] Envirsfx
        {
            get
            {
                if (audios.envirsfx != null)
                {
                    string[] envirsfx = new string[audios.envirsfx.Length];
                    for (int i = 0; i < audios.envirsfx.Length; i++)
                    {
                        envirsfx[i] = audios.envirsfx[i].name;
                    }
                    if (envirsfx.Length > 0)
                        return envirsfx;
                    else return new string[] { AVGHelper.None };
                }
                else return new string[] { AVGHelper.None };
            }
        }
        public string[] Sfx
        {
            get
            {
                if (audios.sfx != null)
                {
                    string[] sfx = new string[audios.sfx.Length];
                    for (int i = 0; i < audios.sfx.Length; i++)
                    {
                        sfx[i] = audios.sfx[i].name;
                    }
                    if (sfx.Length > 0)
                        return sfx;
                    else return new string[] { AVGHelper.None };
                }
                else return new string[] { AVGHelper.None };
            }
        }

        public AudioClip GetAudioClip(AVGHelper.SoundType soundType, string audioName)
        {
            switch (soundType)
            {
                case AVGHelper.SoundType.Bgm:
                    return System.Array.Find(audios.bgm, (item) => item.name == audioName);
                case AVGHelper.SoundType.EnvirSfx:
                    return System.Array.Find(audios.envirsfx, (item) => item.name == audioName);
                case AVGHelper.SoundType.Sfx:
                    return System.Array.Find(audios.sfx, (item) => item.name == audioName);
            }
            return null;
        }
        #endregion
    }

    #region datas
    [System.Serializable]
    public class RoleName
    {
        /// <summary>
        /// 中文名
        /// </summary>
        public string zhName;
        /// <summary>
        /// 拼音名
        /// </summary>
        public string pyName;
        /// <summary>
        /// 立绘
        /// </summary>
        public GameObject pot;
        /// <summary>
        /// 立绘动画名
        /// </summary>
        public string[] potAnimas;
    }

    [System.Serializable]
    public class Animations
    {
        [Header("介绍动画")]
        public GameObject[] anima_Intro;
        [Header("插图动画")]
        public GameObject[] anima_Img;
        [Header("CG动画")]
        public GameObject[] anima_Cg;
    }

    [System.Serializable]
    public class RoleIconEmoji
    {
        public string otherRoleName;
        public Sprite[] spIconEmojis;
    }

    [System.Serializable]
    public class Audios
    {
        [Header("背景音")]
        public AudioClip[] bgm;
        [Header("环境音")]
        public AudioClip[] envirsfx;
        [Header("特效声")]
        public AudioClip[] sfx;
    }
    #endregion
}
