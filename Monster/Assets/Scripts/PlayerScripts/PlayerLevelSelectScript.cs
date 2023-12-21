using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class PlayerLevelSelectScript : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, chosen;
    public float animationSpeed;
    public string currentAnimation;

    // Start is called before the first frame update
    void Start()
    {
        SetAnimation(0, idle, true, animationSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerSelectedAnimation()
    {
        SetAnimation(0, chosen, false, animationSpeed);
    }

    public void SetAnimation(int track, AnimationReferenceAsset animation, bool loop, float timeScale)
    {
        if (animation.name.Equals(currentAnimation))
        {
            return;
        }
        Spine.TrackEntry animationEntry = skeletonAnimation.state.SetAnimation(track, animation, loop);
        animationEntry.TimeScale = timeScale;
        animationEntry.Complete += AnimationEntry_Complete;
        currentAnimation = animation.name;
    }

    //Triggers after the animation has played
    private void AnimationEntry_Complete(Spine.TrackEntry trackEntry)
    {
        if(currentAnimation != "idle")
        {
            SetAnimation(0, idle, true, animationSpeed);
        }
    }
}
