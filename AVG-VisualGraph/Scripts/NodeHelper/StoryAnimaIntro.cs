using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(StoryAnimaHelper))]
public class StoryAnimaIntro : MonoBehaviour
{
    public enum AnimaType
    {
        //刷出来 回到原来位置
        Type1, 
        //刷出来 到对面位置
        Type2
    }
    public AnimaType animaType = AnimaType.Type2; 
    public Image imgbg;
    public RectTransform[] upTrans;
    public RectTransform[] bottomTrans;

    public float duration { get; } = 3.8f; //计算得来


    Vector2[] targetUpTransPos;//目标位置 
    Vector2[] baseUpTransPos; //初始位置

    Vector2[] targetBottomTransPos;//目标位置 
    Vector2[] baseBottomTransPos; //初始位置

    private float screenHeight;

    private void Awake()
    {
        screenHeight = (this.transform as RectTransform).rect.height;
        targetUpTransPos = new Vector2[upTrans.Length];//目标位置 
        baseUpTransPos = new Vector2[upTrans.Length]; //初始位置
        for (int i = 0; i < upTrans.Length; i++)
        {
            RectTransform item = upTrans[i] as RectTransform;
            targetUpTransPos[i] = item.anchoredPosition;
            baseUpTransPos[i] = new Vector2(item.anchoredPosition.x, item.rect.height * 0.5f + 50f);
        }

        targetBottomTransPos = new Vector2[bottomTrans.Length];//目标位置 
        baseBottomTransPos = new Vector2[bottomTrans.Length]; //初始位置
        for (int i = 0; i < bottomTrans.Length; i++)
        {
            RectTransform item = bottomTrans[i] as RectTransform;
            targetBottomTransPos[i] = item.anchoredPosition;
            baseBottomTransPos[i] = new Vector2(item.anchoredPosition.x, (item.rect.height * 0.5f + 50f) * -1f);
        }
    }
    private void OnEnable()
    {
        DoAnima();
    }

    [NaughtyAttributes.Button]
    private void DoAnima()
    {
        imgbg.color = new Color(1, 1, 1, 0);
        imgbg.DOFade(1, 1f);
        imgbg.DOFade(0, 0.3f).SetDelay(3.5f);

        for (int i = 0; i < upTrans.Length; i++)
        {
            RectTransform item = upTrans[i] as RectTransform;
            item.anchoredPosition = baseUpTransPos[i];

            switch (animaType)
            {
                case AnimaType.Type1:
                    Sequence seq = DOTween.Sequence();
                    float targetY = targetUpTransPos[i].y;
                    seq.Append(item.DOAnchorPosY(targetY, Random.Range(0.3f, 0.4f)).SetEase(Ease.OutCubic));
                    seq.Append(item.DOAnchorPosY(targetY - 20f, Random.Range(3f, 3.1f)).SetEase(Ease.InOutSine));
                    seq.Append(item.DOAnchorPosY(baseUpTransPos[i].y, Random.Range(0.2f, 0.3f)).SetEase(Ease.OutCubic));
                    seq.Play();
                    break;
                case AnimaType.Type2:
                    Sequence seq2 = DOTween.Sequence();
                    float targetY2 = targetUpTransPos[i].y;
                    seq2.Append(item.DOAnchorPosY(targetY2, Random.Range(0.3f, 0.4f)).SetEase(Ease.OutCubic));
                    seq2.Append(item.DOAnchorPosY(targetY2 - 50f, Random.Range(2.5f, 2.6f)).SetEase(Ease.InOutSine));
                    seq2.Append(item.DOAnchorPosY(targetY2 - 50f - screenHeight, Random.Range(0.2f, 0.3f)).SetEase(Ease.OutCubic));
                    seq2.Play();
                    break;
            }
        }


        for (int i = 0; i < bottomTrans.Length; i++)
        {
            RectTransform item = bottomTrans[i] as RectTransform;
            item.anchoredPosition = baseBottomTransPos[i];

            switch (animaType)
            {
                case AnimaType.Type1:
                    Sequence seq = DOTween.Sequence();
                    float targetY = targetBottomTransPos[i].y;
                    seq.Append(item.DOAnchorPosY(targetY, Random.Range(0.3f, 0.4f)).SetEase(Ease.OutCubic));
                    seq.Append(item.DOAnchorPosY(targetY + 20, Random.Range(3f, 3.1f)).SetEase(Ease.InOutSine));
                    seq.Append(item.DOAnchorPosY(baseBottomTransPos[i].y, Random.Range(0.2f, 0.3f)).SetEase(Ease.OutCubic));
                    seq.Play();
                    break;
                case AnimaType.Type2:
                    Sequence seq2 = DOTween.Sequence();
                    float targetY2 = targetBottomTransPos[i].y;
                    seq2.Append(item.DOAnchorPosY(targetY2, Random.Range(0.3f, 0.4f)).SetEase(Ease.OutCubic));
                    seq2.Append(item.DOAnchorPosY(targetY2 + 50, Random.Range(2.5f, 2.6f)).SetEase(Ease.InOutSine));
                    seq2.Append(item.DOAnchorPosY(targetY2 + 50 + screenHeight, Random.Range(0.2f, 0.3f)).SetEase(Ease.OutCubic));
                    seq2.Play();
                    break;
            }
        }
    }
}
