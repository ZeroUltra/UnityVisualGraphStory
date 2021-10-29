using UnityEngine;
using Spine.Unity;

[DisallowMultipleComponent]
public class StoryAnimaHelper : MonoBehaviour
{

    /// <summary>
    /// 动画时长
    /// </summary>
    public float AnimaDuration
    {
        get
        {
            Animator animator = this.gameObject.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                var animactrl = animator.runtimeAnimatorController;
                var animaClips = animactrl.animationClips;
                float duration = 0;
                for (int i = 0; i < animaClips.Length; i++)
                    duration += animaClips[i].length;
                return duration;
            }
            SkeletonGraphic spine = this.gameObject.GetComponentInChildren<SkeletonGraphic>();
            if (spine != null)
            {
                return spine.SkeletonData.FindAnimation(spine.AnimationState.GetCurrent(0).Animation.Name).Duration;
            }
            else
            {
                StoryAnimaIntro srotyIntro = this.gameObject.GetComponentInChildren<StoryAnimaIntro>();
                if (srotyIntro != null)
                {
                    return srotyIntro.duration;
                }
            }
            return 0f;
        }
    }
    public void DoStory(System.Action onStoryEndCb)
    {
        Timer.Register(AnimaDuration, () =>
        {
            onStoryEndCb?.Invoke();
        }, autoDestroyOwner: this);
    }
}
