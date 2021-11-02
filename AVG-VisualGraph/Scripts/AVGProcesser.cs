using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using LJ.VisualAVG;
using DG.Tweening;
using Spine.Unity;

/// <summary>
/// 处理node 逻辑
/// </summary>
public class AVGProcesser : MonoBehaviour
{
    //#if UNITY_EDITOR
    public static event System.Action<Node_Base> OnChangeNodeEvent;
    //#endif

    public const string NICKNAME = "[#]"; //代表玩家名字

    public AVGGraph AvgNodeGraphAsset;//剧情节点资源
    #region Component
    [Space(10)]
    public Button btnNext;
    public Button btnSayMask; //说遮罩
    public Graphic maskImgAuto;

    [Header("背景")]
    public Image imgBgF;
    public Image imgBgB;

    [Header("旁白")]
    public CanvasGroup panel_SayAside; //旁白
    public Text txt_AsideContent;

    [Header("说话无头像")]
    public CanvasGroup panel_SayNoHead;
    public Text txt_NoHeadRoleName;
    public Text txt_NoHeadSayContent;

    [Header("说话有头像")]
    public CanvasGroup panel_SayHead;
    public Text txt_HeadRoleName;
    public Text txt_HeadSayContent;
    public Image img_HeadSayIcon;

    [Header("选项")]
    public CanvasGroup panel_cmdMenu;
    public Button[] menuBtns;
    private Text[] menuTxts;

    [Header("容器")]
    public Transform transParentPot; //立绘容器
    public Transform transParentAnimaImg; //介绍/插图动画容器
    public Transform transParentAnimaIntro; //介绍/插图动画容器
    public Transform transParentAnimaCG; //插图动画容器
    public Transform transParentInter; //交互容器

    [Header("底部按钮")]
    public Button btnAuto;
    public Button btnSkip;
    public Text txt_btnAuto, txt_btnSkip;
    #endregion

    private AVGGraphAssets avgAssets;

    private Tweener typerTwneer; //文字打字机tweener
    private float typerSpeed = 30;
    private Ease typerTwEase = Ease.OutSine;


    private List<CanvasGroup> listPanel = new List<CanvasGroup>(); //所有面板
    private CanvasGroup lastCG = null;
    private GameObject currentInterGo = null; //当前交互对象

    private Dictionary<int, int> dictMenu = new Dictionary<int, int>(); //选择结果存贮
    private int currentMenuID;
    //立绘集合
    private List<PotParams> listPot = new List<PotParams>();
    //动画集合
    private List<(AVGHelper.AnimaType animaType, GameObject go)> listAnima = new List<(AVGHelper.AnimaType animaType, GameObject go)>();
    private List<Timer> listTimer = new List<Timer>();

    #region Property
    private bool CanNextNode
    {
        get { return btnNext.interactable; }
        set { btnNext.interactable = value; }
    } //是否可以下一个节点

    private bool IsSaying
    {
        get { return btnSayMask.targetGraphic.raycastTarget; }
        set
        {
            btnSayMask.targetGraphic.raycastTarget = value;
        }
    } //是否正在说话 对话按钮遮罩是否可以点击    可以点击就在说话


    private Node_Base currentNode; //下一个节点
    public Node_Base CurrentNode { get => currentNode; set => currentNode = value; }

    private bool AutoMaskInteract
    {
        get { return maskImgAuto.raycastTarget; }
        set
        {
            maskImgAuto.raycastTarget = value;
        }
    }  //是否显示自动遮罩 显示了就不能点击下一步了

    private enum ReadTypeEnum
    {
        OneNextOne,//一个接一个
        Auto,//自动
        AutoDouble //自动双倍
    }

    [SerializeField] private ReadTypeEnum readType;
    private ReadTypeEnum ReadType
    {
        get => readType;
        set
        {
            readType = value;
            switch (readType)
            {
                case ReadTypeEnum.OneNextOne:
                    //  txt_btnAuto.text = "自动".ToLocalizeInner();
                    txt_btnAuto.text = "自动";
                    AutoMaskInteract = false;
                    Time.timeScale = 1f;
                    break;
                case ReadTypeEnum.Auto:
                    // txt_btnAuto.text = "1x自动中".ToLocalizeInner();
                    txt_btnAuto.text = "1x自动中";
                    AutoMaskInteract = true;
                    if (CanNextNode && IsSaying == false)
                    {
                        ProcessAVGNode();
                    }
                    break;
                case ReadTypeEnum.AutoDouble:
                    //txt_btnAuto.text = "2x自动中".ToLocalizeInner();
                    txt_btnAuto.text = "2x自动中";
                    AutoMaskInteract = true;
                    Time.timeScale = 1.5f;
                    break;
            }
        }
    }

    /// <summary>
    /// 是否为多个数据 例如旁白说可以添加多个数据
    /// </summary>
    private bool IsMultiDatas { get; set; } = false;
    private int MultiDatasIndex { get; set; } = -1;
    #endregion

    private void Start()
    {
        if (AvgNodeGraphAsset == null) Debug.LogError("AVG Asset Null!!!");
        avgAssets = AvgNodeGraphAsset.graphAssets;

        (AvgNodeGraphAsset as AVGGraph).InitializeGraph();
        CurrentNode = (AvgNodeGraphAsset as AVGGraph).StartingNode.Outputs.First().GetConnectionsFirstNode() as Node_Base;


        //进行下一个Node
        btnNext.onClick.AddListener(() => ProcessAVGNode());
        //如果正在说话 点击直接结束
        btnSayMask.onClick.AddListener(() =>
        {
            //如果动画还在播放就结束
            if (typerTwneer.IsActive() && !typerTwneer.IsComplete())
            {
                typerTwneer.Complete();
                return;
            }
        });
        //自动剧情
        btnAuto.onClick.AddListener(() =>
        {
            int v = ((int)ReadType);
            v++;
            if (v > (int)ReadTypeEnum.AutoDouble)
                v = 0;
            ReadType = (ReadTypeEnum)v;
        });

        //menu菜单
        menuTxts = new Text[menuBtns.Length];
        for (int i = 0; i < menuBtns.Length; i++)
        {
            menuTxts[i] = menuBtns[i].GetComponentInChildren<Text>();
            int index = i;
            menuBtns[i].onClick.AddListener(() => OnMenuBtnClick(index));
        }

        listPanel.Add(panel_SayAside);
        listPanel.Add(panel_SayHead);
        listPanel.Add(panel_SayNoHead);
        listPanel.Add(panel_cmdMenu);


        ResetUIState();
        ProcessAVGNode();
    }

    /// <summary>
    /// 获取下一个Node
    /// </summary>
    /// <param name="outputPortIndex">节点索引</param>
    private void LoadNextNode(int outputPortIndex)
    {

        switch (outputPortIndex)
        {
            case 0:
                CurrentNode = CurrentNode.GetOutput1Port()?.GetConnectionsFirstNode() as Node_Base;
                break;
            case 1:
                CurrentNode = CurrentNode.GetOutput2Port()?.GetConnectionsFirstNode() as Node_Base;
                break;
            case 2:
                CurrentNode = CurrentNode.GetOutput3Port()?.GetConnectionsFirstNode() as Node_Base;
                break;
        }
    }

    /// <summary>
    /// 处理节点
    /// </summary>
    /// <param name="node"></param>
    private void ProcessAVGNode()
    {
        OnChangeNodeEvent?.Invoke(CurrentNode);

        if (currentInterGo != null)
        {
            if (currentInterGo.TryGetComponent<CanvasGroup>(out CanvasGroup cg))
                cg.DOFade(0, 0.2f).OnComplete(() =>
                {
                    if (currentInterGo != null)
                        GameObject.Destroy(currentInterGo.gameObject);
                });
            else
                if (currentInterGo != null) { GameObject.Destroy(currentInterGo.gameObject); }
        }

        if (CurrentNode != null)
        {
            if (CurrentNode is Node_Wait)
            {
                var nodewait = CurrentNode as Node_Wait;
                CanNextNode = false;
                Timer.Register(nodewait.waitTimer, () =>
                {
                    CanNextNode = true;
                    LoadNextNode(0);
                    ProcessAVGNode();
                });
            }

            else if (CurrentNode is Node_BG)
            {
                var node_bg = CurrentNode as Node_BG;
                Sprite sp = avgAssets.GetBgSprite(node_bg.spBgName);
                switch (node_bg.fXType)
                {
                    case AVGHelper.FXType2.None:
                        imgBgF.color = Color.white;
                        imgBgF.sprite = sp;
                        break;
                    case AVGHelper.FXType2.FadeInOut:
                        if (imgBgF.sprite != null)
                        {
                            imgBgB.color = Color.white;
                            imgBgB.sprite = imgBgF.sprite;
                            imgBgB.DOColor(Color.clear, 0.4f).SetEase(Ease.OutSine);
                        }
                        imgBgF.color = Color.clear;
                        imgBgF.sprite = sp;
                        imgBgF.DOColor(Color.white, 0.4f).SetEase(Ease.InSine);
                        break;
                }
                LoadNextNode(0);
                ProcessAVGNode();
            }
            //旁白
            else if (CurrentNode is Node_SayAside)
            {
                var node_sayaside = CurrentNode as Node_SayAside;
                ShowPanel(panel_SayAside);
                int dataLength = node_sayaside.Datas.Count;
                if (dataLength > 1)
                {
                    MultiDatasIndex++;
                    IsMultiDatas = true;
                    //最后一个
                    if (MultiDatasIndex == dataLength-1)
                        IsMultiDatas = false;
                    //IsMultiDatas=true 就不会load下一个节点   否则会自动load下一个节点
                    SayDoText(txt_AsideContent, node_sayaside.Datas[MultiDatasIndex].msg);
                }
                else
                    SayDoText(txt_AsideContent, node_sayaside.Datas[0].msg);
            }
            //玩家说
            else if (CurrentNode is Node_SayMe)
            {
                DOSayBgAnimation(panel_SayNoHead);
                ShowPanel(panel_SayNoHead);
                var node_sayme = CurrentNode as Node_SayMe;
                txt_NoHeadRoleName.text = avgAssets.userName;

                string msgContent = node_sayme.msg.Replace(NICKNAME, avgAssets.userName);
                //如果目标对象不为null
                if (node_sayme.targetRoleName.IsNullOrEmtryOrNone() == false)
                {
                    string enterTalkEmoji = node_sayme.enterTalkEmoji;
                    string talkEmoji = node_sayme.talkEmoji;
                    string exitTalkEmoji = node_sayme.exitTalkEmoji;
                    string talkEndEmoji = node_sayme.talkEndEmoji;
                    string pyname = avgAssets.GetRolePyName(node_sayme.targetRoleName);
                    //找到当前对应的并且播放动画
                    var potParamsa = listPot.Find((item) => item.potGo.name == pyname);
                    if (potParamsa != null)
                    {
                        if (!enterTalkEmoji.IsNullOrEmtryOrNone())//进入表情不为空
                        {
                            potParamsa.potSpine.AnimationState.SetAnimation(1, enterTalkEmoji, false).Complete += (data) =>
                            {
                                if (!talkEmoji.IsNullOrEmtryOrNone())
                                    potParamsa.potSpine.AnimationState.SetAnimation(1, talkEmoji, true);
                            };
                        }
                        else
                        {
                            if (!talkEmoji.IsNullOrEmtryOrNone())
                                potParamsa.potSpine.AnimationState.SetAnimation(1, talkEmoji, true);
                        }
                    }
                    SayDoText(txt_NoHeadSayContent, msgContent, onComplete: () =>
                    {
                        if (!exitTalkEmoji.IsNullOrEmtryOrNone())
                        {
                            potParamsa.potSpine.AnimationState.SetAnimation(1, exitTalkEmoji, false).Complete += (data) =>
                            {
                                if (!talkEndEmoji.IsNullOrEmtryOrNone())
                                    potParamsa.potSpine.AnimationState.SetAnimation(1, talkEndEmoji, true);
                            };
                        }
                        else
                        {
                            if (!talkEndEmoji.IsNullOrEmtryOrNone())
                                potParamsa.potSpine.AnimationState.SetAnimation(1, talkEndEmoji, true);//说话内容
                        }
                    });
                }
                else SayDoText(txt_NoHeadSayContent, msgContent);//说话内容
            }
            //其他说
            else if (CurrentNode is Node_SayOther)
            {
                var node_sayother = CurrentNode as Node_SayOther;
                if (node_sayother.showIcon)
                {
                    DOSayBgAnimation(panel_SayHead);
                    ShowPanel(panel_SayHead);

                    img_HeadSayIcon.sprite = avgAssets.GetOtherSpIcon(node_sayother.roleName, node_sayother.spName);
                    img_HeadSayIcon.SetNativeSize();
                    if (img_HeadSayIcon.color.a <= 0.5f) //渐变显示头像
                        img_HeadSayIcon.DOFade(1, 0.2f);
                }
                else
                {
                    DOSayBgAnimation(panel_SayNoHead);
                    ShowPanel(panel_SayNoHead);
                    if (img_HeadSayIcon.color.a >= 0.5f)//渐变隐藏头像
                        img_HeadSayIcon.DOFade(0, 0.1f);
                }
            }
            //角色说
            else if (CurrentNode is Node_SayRole)
            {
                DOSayBgAnimation(panel_SayNoHead);
                ShowPanel(panel_SayNoHead);
                var node_sayrole = CurrentNode as Node_SayRole;
                txt_NoHeadRoleName.text = node_sayrole.roleName;

                string roleEnterTalkEmoji = node_sayrole.enterTalkEmoji;
                string roletalkEmoji = node_sayrole.talkEmoji;
                string roleExitTalkEmoji = node_sayrole.exitTalkEmoji;
                string roletalkEndEmoji = node_sayrole.talkEndEmoji;
                string rolepyname = avgAssets.GetRolePyName(node_sayrole.roleName);
                string contentsayrole = node_sayrole.msg.Replace(NICKNAME, avgAssets.userName);

                var potParams = listPot.Find((item) => item.potGo.name == rolepyname);
                if (potParams == null)
                {
                    Debug.LogError($"立绘为null: {rolepyname}");
                }
                if (potParams != null)
                {
                    if (potParams.potSpine.AnimationState != null)
                    {
                        var track = potParams.potSpine.AnimationState.GetCurrent(1);
                        if (!roleEnterTalkEmoji.IsNullOrEmtryOrNone())
                        {
                            potParams.potSpine.AnimationState.SetAnimation(1, roleEnterTalkEmoji, false).Complete += (data) =>
                            {
                                if (!roletalkEmoji.IsNullOrEmtryOrNone())
                                    potParams.potSpine.AnimationState.SetAnimation(1, roletalkEmoji, true);
                            };
                        }
                        else if (track == null || track.Animation.Name != roletalkEmoji)
                        {
                            potParams.potSpine.AnimationState.SetAnimation(1, roletalkEmoji, true);
                        }
                    }
                }
                SayDoText(txt_NoHeadSayContent, contentsayrole, onComplete: () =>
                {
                    if (potParams.potSpine?.AnimationState != null)
                    {
                        var track = potParams.potSpine.AnimationState.GetCurrent(1);
                        if (!roleExitTalkEmoji.IsNullOrEmtryOrNone())
                        {
                            potParams.potSpine.AnimationState.SetAnimation(1, roleExitTalkEmoji, false).Complete += (data) =>
                            {
                                if (!roletalkEndEmoji.IsNullOrEmtryOrNone())
                                    potParams.potSpine.AnimationState.SetAnimation(1, roletalkEndEmoji, true);
                            };
                        }
                        else if (track == null || track.Animation.Name != roletalkEndEmoji)
                        {
                            try
                            {
                                if (!roletalkEndEmoji.IsNullOrEmtryOrNone())
                                    potParams.potSpine.AnimationState.SetAnimation(1, roletalkEndEmoji, true);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError($"名字:{potParams.potSpine.name} 动画:{roletalkEndEmoji} 内容:{contentsayrole} {e.ToString()}");
                            }
                        }
                    }
                });//说话内容
            }
            else if (CurrentNode is Node_SayHide)
            {
                ShowPanel(null, isCloseAll: true);
                LoadNextNode(0);
                ProcessAVGNode();
            }

            else if (CurrentNode is Node_Menu)
            {
                var node_menu = CurrentNode as Node_Menu;
                ShowPanel(panel_cmdMenu, isLastCGHide: false);
                currentMenuID = node_menu.menuID;
                int optionlength = node_menu.options.Count;
                for (int i = 0; i < menuBtns.Length; i++)
                {
                    if (i < optionlength)
                    {
                        menuBtns[i].gameObject.SetActive(true);
                        menuTxts[i].text = node_menu.options[i];
                    }
                    else
                    {
                        menuBtns[i].gameObject.SetActive(false);
                    }
                }
            }
            else if (CurrentNode is Node_MenuResult)
            {
                var node_menuresult = CurrentNode as Node_MenuResult;
                if (dictMenu.TryGetValue(node_menuresult.targetMenuID, out int index))
                {
                    LoadNextNode(index);
                    ProcessAVGNode();
                }
                else Debug.LogError($"Node_MenuResult 没有这个Key:{node_menuresult.targetMenuID}");
            }

            else if (CurrentNode is Node_PotShow)
            {
                var node_potshow = CurrentNode as Node_PotShow;
                string potpyName = avgAssets.GetRolePyName(node_potshow.potname);
                var effectType = node_potshow.effectType;
                var posType = node_potshow.posType;
                var disType = node_potshow.disType;

                var potGoParams = listPot.Find((item) => item.potGo.name == potpyName);
                //如果没有就克隆
                if (potGoParams == null)
                {
                    GameObject goPotPrefab = avgAssets.GetRolePotByPyName(potpyName);
                    GameObject goPot = GameObject.Instantiate(goPotPrefab, transParentPot);
                    goPot.name = potpyName;
                    goPot.transform.rotation = Quaternion.identity;
                    var potSpine = goPot.GetComponentInChildren<SkeletonGraphic>();
                    try
                    {
                        potSpine.AnimationState.SetAnimation(0, "breath", true);//默认呼吸动画
                        potSpine.AnimationState.SetAnimation(1, "idle", true);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(RichString.ColorWhite("播放动画 breath 错误:" + e.ToString()));
                    }
                    potGoParams = new PotParams(goPot, potSpine, posType, disType);
                    listPot.Add(potGoParams);
                }

                switch (posType)
                {
                    case AVGHelper.PosType.Left:
                        potGoParams.potGo.transform.localPosition = new Vector3(-250f, potGoParams.potGo.transform.localPosition.y, 0);
                        break;
                    case AVGHelper.PosType.Middle:
                        potGoParams.potGo.transform.localPosition = new Vector3(0f, potGoParams.potGo.transform.localPosition.y, 0);
                        break;
                    case AVGHelper.PosType.Right:
                        potGoParams.potGo.transform.localPosition = new Vector3(250f, potGoParams.potGo.transform.localPosition.y, 0);
                        break;
                }
                switch (disType)
                {
                    case AVGHelper.DistanceType.Normal:
                        potGoParams.potGo.transform.localScale = Vector3.one;
                        potGoParams.potGo.transform.localPosition = new Vector3(potGoParams.potGo.transform.localPosition.x, -1792f, 0);
                        break;
                    case AVGHelper.DistanceType.Front:
                        potGoParams.potGo.transform.localScale = Vector3.one * 1.4f;
                        potGoParams.potGo.transform.localPosition = new Vector3(potGoParams.potGo.transform.localPosition.x, -2838f, 0);
                        break;
                    case AVGHelper.DistanceType.SFront:
                        break;
                }

                if (potGoParams.potGo.gameObject.activeInHierarchy == false)
                    potGoParams.potGo.gameObject.SetActive(true);

                //位置不同 隐藏重新到新位置
                if (posType != potGoParams.potPosType || disType != potGoParams.potDisType)
                    potGoParams.potSpine.color = new Color(1, 1, 1, 0);

                if (!Mathf.Approximately(potGoParams.potSpine.color.a, 1f))
                    potGoParams.potSpine.DOFade(1, 0.2f);
                LoadNextNode(0);
                ProcessAVGNode();
            }
            else if (CurrentNode is Node_PotHide)
            {
                var node_pothide = CurrentNode as Node_PotHide;
                string potHideName = avgAssets.GetRolePyName(node_pothide.potname);
                var effectHideType = node_pothide.effectType;
                for (int i = listPot.Count - 1; i >= 0; i--)
                {
                    if (listPot[i].potGo.name == potHideName)
                    {
                        int index = i;
                        SkeletonGraphic spinePothide = listPot[i].potSpine;
                        spinePothide.DOFade(0, 0.2f).OnComplete(() =>
                        {
                            listPot[index].potGo.gameObject.SetActive(false);
                        });
                        break;
                    }
                }
                LoadNextNode(0);
                ProcessAVGNode();
            }

            else if (CurrentNode is Node_AnimaShow)
            {
                var node_animashow = CurrentNode as Node_AnimaShow;
                btnSkip.gameObject.SetActive(false);
                btnAuto.gameObject.SetActive(false);

                AVGHelper.AnimaType animaType = node_animashow.animaType;
                Transform targetParent = null;
                switch (animaType)
                {
                    case AVGHelper.AnimaType.Intro:
                        targetParent = transParentAnimaIntro;
                        break;
                    case AVGHelper.AnimaType.Img:
                        targetParent = transParentAnimaImg;
                        break;
                    case AVGHelper.AnimaType.Cg:
                        targetParent = transParentAnimaCG;
                        break;
                }
                string animaName = node_animashow.animaName;
                if (animaName.IsNullOrEmtryOrNone()) LoadNextNode(0);

                bool autoDestory = node_animashow.autoDestory;
                var animaGoab = avgAssets.GetAnimaGo(animaType, animaName);
                var animaGo = GameObject.Instantiate<GameObject>(animaGoab, Vector3.zero, Quaternion.identity, targetParent);
                animaGo.name = animaName;
                animaGo.transform.localPosition = Vector3.zero;
                CanNextNode = false;
                animaGo.gameObject.GetComponent<StoryAnimaHelper>().DoStory(() =>
                {
                    CanNextNode = true;
                    LoadNextNode(0);
                    ProcessAVGNode();
                    //是否自动销毁
                    btnSkip.gameObject.SetActive(true);
                    btnAuto.gameObject.SetActive(true);
                    if (autoDestory)
                    {
                        if (animaGo.TryGetComponent<CanvasGroup>(out CanvasGroup cg))
                            cg.DOFade(0, 0.2f).SetEase(Ease.Linear).OnComplete(() => GameObject.Destroy(animaGo));
                        else
                            GameObject.Destroy(animaGo);
                    }
                });
                if (!autoDestory)
                    listAnima.Add((animaType, animaGo));
            }
            else if (CurrentNode is Node_AnimaHide)
            {
                var node_animahide = CurrentNode as Node_AnimaHide;
                var animaHType = node_animahide.animaType;
                string animaHName = node_animahide.animaName;
                for (int i = listAnima.Count - 1; i >= 0; i--)
                {
                    if (listAnima[i].animaType == animaHType && listAnima[i].go.name == animaHName)
                    {
                        //有 CanvasGroup 先渐渐隐藏
                        if (listAnima[i].go.TryGetComponent<CanvasGroup>(out CanvasGroup cg))
                        {
                            cg.DOFade(0, 0.2f).OnComplete(() =>
                            {
                                if (listAnima.Count > 0)
                                {
                                    GameObject.Destroy(listAnima[i].go.gameObject);
                                    listAnima.RemoveAt(i);
                                }
                            });
                        }
                        else
                        {
                            GameObject.Destroy(listAnima[i].go.gameObject);
                            listAnima.RemoveAt(i);
                        }
                        break;
                    }
                }
                LoadNextNode(0);
                ProcessAVGNode();
            }

            else if (CurrentNode is Node_SoundPlay)
            {
                var node_soundplay = CurrentNode as Node_SoundPlay;
                AVGHelper.SoundType soundType = node_soundplay.soundType;
                string soundName = node_soundplay.soundName;
                bool isLoop = node_soundplay.isLoop;
                float waitTime = node_soundplay.waitTime;
                if (Mathf.Approximately(waitTime, 0) == false)
                {
                    Timer waitSoundTimer = null;
                    waitSoundTimer = Timer.Register(waitTime, () =>
                    {
                        PlayAudio(soundType, soundName, isLoop);
                        listTimer.Remove(waitSoundTimer);
                    });
                    listTimer.Add(waitSoundTimer);
                }
                else
                    PlayAudio(soundType, soundName, isLoop);
                LoadNextNode(0);
                ProcessAVGNode();
            }
            else if (CurrentNode is Node_SoudeStop)
            {
                var node_soudestop = CurrentNode as Node_SoudeStop;
                AVGHelper.SoundType soundTy = node_soudestop.soundType;
                if (soundTy == AVGHelper.SoundType.Bgm)
                {
                    AudioSource audio1 = this.transform.Find(AVGHelper.SoundType.Bgm.ToString()).GetComponent<AudioSource>();
                    DOTween.To(() => audio1.volume, x => audio1.volume = x, 0, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        Destroy(audio1.gameObject);
                    });
                }
                else if (soundTy == AVGHelper.SoundType.EnvirSfx)
                {
                    AudioSource audio2 = this.transform.Find($"{soundTy.ToString()}/{node_soudestop.soundName}").GetComponent<AudioSource>();
                    DOTween.To(() => audio2.volume, x => audio2.volume = x, 0, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        Destroy(audio2.gameObject);
                    });
                }
                else
                    Destroy(this.transform.Find($"{soundTy.ToString()}/{node_soudestop.soundName}").gameObject);
                LoadNextNode(0);
                ProcessAVGNode();

            }

            else if (CurrentNode is Node_Interact)
            {
                var node_interact = CurrentNode as Node_Interact;
                string interName = node_interact.interactName;
                bool isautodestory = node_interact.isAutoDestory;
                GameObject goInterPrefab = avgAssets.GetInterGoByName(interName);
                currentInterGo = GameObject.Instantiate(goInterPrefab, transParentInter);
                var _cg = currentInterGo.GetComponent<CanvasGroup>();
                _cg.alpha = 0f;
                _cg.DOFade(1, 0.2f);
                currentInterGo.transform.rotation = Quaternion.identity;
                CanNextNode = false;
                currentInterGo.GetComponent<StoryInteractHelper>().OnInteractEndEvent += () =>
                {
                    CanNextNode = true;
                    if (isautodestory)
                    {
                        if (currentInterGo.TryGetComponent<CanvasGroup>(out CanvasGroup cg))
                        {
                            cg.DOFade(0, 0.2f).OnComplete(() =>
                            {
                                GameObject.Destroy(currentInterGo.gameObject);
                                currentInterGo = null;
                            });
                        }
                    }
                    LoadNextNode(0);
                    ProcessAVGNode();
                };
            }
        }
        else
        {
            Debug.Log("结束");
        }
    }

    #region Say
    /// <summary>
    /// 不是旁白说的时候 做一个动画
    /// </summary>
    private void DOSayBgAnimation(CanvasGroup panel)
    {
        //说面板 动画效果 判定给0.5 是表示如果已经显示了就不在做动画
        if (panel.alpha <= 0.5f)
        {
            RectTransform rectTrans = (panel.transform as RectTransform);
            rectTrans.anchoredPosition = new Vector2(-27, 25f);
            rectTrans.DOAnchorPos(new Vector2(0f, 0f), 0.2f).SetEase(typerTwEase);
        }
    }
    private Timer timertext;
    private void SayDoText(Text text, string msg, System.Action onComplete = null)
    {
        if (timertext != null && timertext.isCompleted == false) timertext.Cancel();
        if (text.text.IsNullOrEmtryOrNone() == false) text.text = string.Empty;
        IsSaying = true;
        typerTwneer = text.DOText(msg, typerSpeed).SetSpeedBased().SetEase(typerTwEase).OnComplete(() =>
        {
            IsSaying = false;
            if (IsMultiDatas == false)
            {
                MultiDatasIndex = -1;
                LoadNextNode(0);
                //如果下一条是菜单 自动弹出
                if (CurrentNode is Node_Menu)
                    ProcessAVGNode();
            }
            //如果是自动
            if (ReadType != ReadTypeEnum.OneNextOne)
            {
                timertext = Timer.Register(0.6f * Time.timeScale, () => { ProcessAVGNode(); });
            }
            onComplete?.Invoke();
        });
    }
    #endregion

    #region Audio
    private void PlayAudio(AVGHelper.SoundType soundType, string soundName, bool isLoop)
    {
        AudioSource targetAudio;
        Transform parent = this.transform.Find(soundType.ToString());
        if (parent == null)
        {
            parent = new GameObject(soundType.ToString()).transform;
            parent.SetParent(this.transform);
        }
        if (soundType == AVGHelper.SoundType.Bgm)
        {
            if (!parent.TryGetComponent(out AudioSource bgmaudio))
                targetAudio = parent.gameObject.AddComponent<AudioSource>();
            else targetAudio = bgmaudio;
        }
        else
        {
            targetAudio = new GameObject(soundName).AddComponent<AudioSource>();
            targetAudio.transform.SetParent(parent);
        }
        if (soundType == AVGHelper.SoundType.Bgm)
        {
            //bgm 有声音 先淡出
            if (targetAudio.clip != null)
            {
                DOTween.To(() => targetAudio.volume, x => targetAudio.volume = x, 0, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    targetAudio.clip = avgAssets.GetAudioClip(soundType, soundName);
                    if (targetAudio.clip == null) return;
                    targetAudio.loop = true;
                    targetAudio.Play();
                    DOTween.To(() => targetAudio.volume, x => targetAudio.volume = x, 1, 0.3f).SetEase(Ease.Linear);
                });
            }
            else
            {
                targetAudio.clip = avgAssets.GetAudioClip(soundType, soundName);
                if (targetAudio.clip == null) return;
                targetAudio.volume = 0;
                targetAudio.loop = true;
                targetAudio.Play();
                DOTween.To(() => targetAudio.volume, x => targetAudio.volume = x, 1, 0.3f).SetEase(Ease.Linear);
            }
        }
        else
        {
            targetAudio.clip = avgAssets.GetAudioClip(soundType, soundName);
            if (targetAudio.clip == null) return;
            targetAudio.loop = isLoop;
            if (targetAudio.loop == false) Destroy(targetAudio.gameObject, targetAudio.clip.length / Time.timeScale);
            targetAudio.Play();
        }
    }
    #endregion

    #region Menu
    private void OnMenuBtnClick(int index)
    {
        dictMenu.Add(currentMenuID, index); //存贮了选择
        //关闭界面
        ShowPanel(null, isCloseAll: true);
        LoadNextNode(index);
        ProcessAVGNode();
    }
    #endregion

    /// <summary>
    /// 显示特定的panel 隐藏其他
    /// </summary>
    /// <param name="targetCG"></param>
    /// <param name="isLastCGHide">上一个是否隐藏</param>
    /// <param name="isCloseAll">是否关闭所有</param>
    private void ShowPanel(CanvasGroup targetCG, bool isLastCGHide = true, bool isCloseAll = false)
    {
        if (!isCloseAll)
        {
            for (int i = 0; i < listPanel.Count; i++)
            {
                //隐藏已经显示的panel
                if (listPanel[i] != targetCG)
                {
                    //上一个不隐藏 
                    if (isLastCGHide == false)
                    {
                        if (lastCG != null) continue;
                    }
                    listPanel[i].DOKill();
                    listPanel[i].DOFade(0, 0.1f);
                    listPanel[i].interactable = listPanel[i].blocksRaycasts = false;
                }
                else
                {
                    targetCG.DOKill();
                    targetCG.DOFade(1, 0.2f);
                    listPanel[i].interactable = listPanel[i].blocksRaycasts = true;
                    lastCG = targetCG;
                }
            }
        }
        else
        {
            for (int i = 0; i < listPanel.Count; i++)
            {
                listPanel[i].DOKill();
                listPanel[i].DOFade(0, 0.1f);
                listPanel[i].interactable = listPanel[i].blocksRaycasts = false;
            }
        }
    }

    private void ResetUIState()
    {
        //ReadType = ReadTypeEnum.OneNextOne;
        dictMenu.Clear();
        listPot.Clear();
        listAnima.Clear();
        listTimer.Clear();
        //ShowAutoMask = false;
        IsSaying = false;
        CanNextNode = true;
        AutoMaskInteract = false;
        ShowPanel(null, isCloseAll: true);
        imgBgB.color = Color.clear;
        imgBgF.color = Color.clear;
    }

    private class PotParams
    {
        public GameObject potGo;
        public SkeletonGraphic potSpine;
        public AVGHelper.PosType potPosType;
        public AVGHelper.DistanceType potDisType;
        public PotParams(GameObject _potGo, SkeletonGraphic _potSpine, AVGHelper.PosType _potPosType, AVGHelper.DistanceType _potDisType)
        {
            this.potGo = _potGo;
            this.potSpine = _potSpine;
            this.potPosType = _potPosType;
            this.potDisType = _potDisType;
        }
    }
}
