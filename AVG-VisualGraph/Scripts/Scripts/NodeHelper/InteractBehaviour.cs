using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
/// <summary>
/// 操作类
/// </summary>
public class InteractBehaviour : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    [Space(10)]
    public bool useDrag;

    //public bool useDragCheck;
    //[ShowIf(nameof(useDragCheck))] public float needToTargetScale = 0f;  //必须到达规定大小
    //[ShowIf(nameof(useDragCheck))] public RectTransform areaRectTrans; //要检测到达的区域

    public bool useDrop;
    [ShowIf(nameof(useDrop))] public UnityEvent onDropEvent;

    #region Click
    public bool useClick;
    [ShowIf(nameof(useClick))] public bool checkClickCount = false;//需要点击到到达次数 才算点击
    private bool NeedClickAndCount { get { return useClick & checkClickCount; } }

    [ShowIf(nameof(NeedClickAndCount))] public int needClickCount = 0;//需要点击到到达次数 才算点击

    [ShowIf(nameof(useClick))] public UnityEvent onClickEvent;
    #endregion

    [Space(10)]
    public bool useDragZoom; //滑动缩放
    [ShowIf(nameof(useDragZoom))] public Vector2 minMaxZoom = new Vector2(0.3f, 2f);
    public bool useDragRotate; //滑动旋转

    [Space(10)]
    public bool doScale;
    [ShowIf(nameof(doScale))] public bool scalePlayAwake;
    [ShowIf(nameof(doScale))] public Vector2 minMaxScale;
    [ShowIf(nameof(doScale))] public float scaleDuration = 0.3f;

    public bool doMove;
    [ShowIf(nameof(doMove))] public bool  movePlayAwake;
    [ShowIf(nameof(doMove))] public Vector2 targetPos;
    [ShowIf(nameof(doMove))] public float moveDuration = 0.3f;
    [ShowIf(nameof(doMove))] public UnityEvent onMoveTargetEventStart;
    [ShowIf(nameof(doMove))] public UnityEvent onMoveTargetEventEnd;
    [ShowIf(nameof(doMove))] public UnityEvent onMoveBaseEventStart;
    [ShowIf(nameof(doMove))] public UnityEvent onMoveBaseEventEnd;


    private CanvasGroup cg;
    public CanvasGroup CG
    {
        get
        {
            if (cg == null)
                cg = this.GetComponent<CanvasGroup>() ?? this.gameObject.AddComponent<CanvasGroup>();
            return cg;
        }
    }


    private RectTransform rectTrans;
    private RectTransform rootRectTrans;
    private Vector2 offsetV2;
    private Vector2 oldPos1, oldPos2;
    private Vector2 curPos1, curPos2;

    private Tweener moveTw;
    private Vector2 basePos;
    private bool isMoveTwForward;

    private int clickCount;

    private void Awake()
    {
        rectTrans = transform as RectTransform;
        Transform parent = transform.parent;
        while (parent != null && parent.TryGetComponent<StoryInteractHelper>(out var helper) == false)
        {
            if (parent.parent == null) break;
            parent = parent.parent;
        }
        rootRectTrans = parent as RectTransform;

        if (doMove)
        {
            isMoveTwForward = true;
            basePos = rectTrans.anchoredPosition;
            if (movePlayAwake)
            {
                DoMoveToTarget();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (useDrag)
        {
            offsetV2 = rectTrans.position - Input.mousePosition;
            CG.alpha = .9f;
            CG.blocksRaycasts = false;
        }
        if (useDragZoom)
        {
            oldPos1 = Vector2.zero;
            oldPos2 = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (useDrag)
        {
            bool isTouchOne = false;
#if UNITY_EDITOR || UNITY_STANDALONE
            isTouchOne = true;
#elif UNITY_ANDROID || UNITY_IOS
            isTouchOne = Input.touchCount == 1;
#endif
            if (isTouchOne)//一个手指移动
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRectTrans, eventData.position, eventData.pressEventCamera, out Vector2 outPos))
                {
                    rectTrans.anchoredPosition = outPos + offsetV2;
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (useDrop)
        {
            onDropEvent?.Invoke();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (useDrag)
        {
            CG.alpha = 1f;
            CG.blocksRaycasts = true;
        }
        if (useDragZoom)
        {
            oldPos1 = Vector2.zero;
            oldPos2 = Vector2.zero;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (useClick)
        {
            if (checkClickCount)
            {
                clickCount++;
                if (clickCount >= needClickCount)
                {
                    onClickEvent?.Invoke();
                    clickCount = 0;
                }
            }
            else
                onClickEvent?.Invoke();
        }
    }


    #region Scale
    /// <summary>
    /// 扩大
    /// </summary>
    public void DoScaleExpand()
    {
        rectTrans.DOScale(Vector3.one * minMaxScale.y, scaleDuration);
    }
    /// <summary>
    /// 缩小
    /// </summary>
    public void DoScaleShrink()
    {
        rectTrans.DOScale(Vector3.one * minMaxScale.x, scaleDuration);
    }
    /// <summary>
    /// 缩放到正常
    /// </summary>
    public void DoScaleNormal()
    {
        rectTrans.DOScale(Vector3.one, scaleDuration);
    }
    #endregion

    #region Move
    /// <summary>
    /// 做位移回到原点
    /// </summary>
    public void DoMoveToBase()
    {
        if (moveTw.IsActive() && moveTw.IsPlaying()) moveTw.Kill();
        moveTw = rectTrans.DOAnchorPos(basePos, moveDuration).OnStart(() => onMoveBaseEventStart?.Invoke()).OnComplete(() => onMoveBaseEventEnd?.Invoke());
    }

    /// <summary>
    /// 做位移到目标点
    /// </summary>
    public void DoMoveToTarget()
    {
        if (moveTw.IsActive() && moveTw.IsPlaying()) moveTw.Kill();
        moveTw = rectTrans.DOAnchorPos(targetPos, moveDuration).OnStart(() => onMoveTargetEventStart?.Invoke()).OnComplete(() => onMoveTargetEventEnd?.Invoke());
    }

    /// <summary>
    /// 做位移来回移动
    /// </summary>
    public void DoMoveAuto()
    {
        if (isMoveTwForward)
            DoMoveToTarget();
        else
            DoMoveToBase();
        isMoveTwForward = !isMoveTwForward;
    }





    #endregion

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (useDragZoom)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                Vector2 tempScale = rectTrans.localScale;
                tempScale += Vector2.one * (scroll);
                rectTrans.localScale = Vector2.Lerp(rectTrans.localScale, tempScale, 0.5f);
                rectTrans.localScale = new Vector2(Mathf.Clamp(rectTrans.localScale.x, minMaxZoom.x, minMaxZoom.y), Mathf.Clamp(rectTrans.localScale.y, minMaxZoom.x, minMaxZoom.y));
            }
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (useDragZoom)
        {
            if (Input.touchCount == 2)//两个手指缩放
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);
                if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    curPos1 = Input.GetTouch(0).position;
                    curPos2 = Input.GetTouch(1).position;
                    //计算模长
                    float oldLength = Mathf.Sqrt((oldPos1.x - oldPos2.x) * (oldPos1.x - oldPos2.x) + (oldPos1.y - oldPos2.y) * (oldPos1.y - oldPos2.y));
                    float curLength = Mathf.Sqrt((curPos1.x - curPos2.x) * (curPos1.x - curPos2.x) + (curPos1.y - curPos2.y) * (curPos1.y - curPos2.y));
                    if (oldPos1 != Vector2.zero && oldPos2 != Vector2.zero)
                    {
                        if (Mathf.Abs(curLength - oldLength) > 1f)//滑动一定距离才能缩放
                        {

                            //根据模长进行缩放
                            float rate = (curLength - oldLength) * 0.01f;
                            Vector2 tempScale = rectTrans.localScale;
                            tempScale += Vector2.one * rate;

                            rectTrans.localScale = Vector2.Lerp(rectTrans.localScale, tempScale, 0.5f);
                            rectTrans.localScale = new Vector2(Mathf.Clamp(rectTrans.localScale.x, minMaxZoom.x, minMaxZoom.y), Mathf.Clamp(rectTrans.localScale.y, minMaxZoom.x, minMaxZoom.y));
                        }

                        if (useDragRotate)
                        {
                            //旋转
                            Vector2 newline = curPos1 - curPos2;
                            Vector2 oldline = oldPos1 - oldPos2;
                            float angle = Vector2.SignedAngle(newline, oldline);
                            rectTrans.Rotate(Vector3.back, angle);
                        }
                    }
                    oldPos1 = curPos1;
                    oldPos2 = curPos2;
                }
            }
       }
#endif
    }


    /// <summary>
    /// fade 显示
    /// </summary>
    /// <param name="duration">持续时间 如果为0 就是默认 0.3f</param>
    public void FadeOpen()
    {
        CG.interactable = CG.blocksRaycasts = true;
        CG.alpha = 0;
        CG.DOFade(1, 0.2f).SetEase(Ease.Linear);
    }

    public void FadeClose()
    {
        CG.DOFade(0, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            CG.interactable = CG.blocksRaycasts = false;
        });
    }
}
