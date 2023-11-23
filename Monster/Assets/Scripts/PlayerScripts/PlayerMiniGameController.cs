using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;
using Spine;

public class PlayerMiniGameController : MonoBehaviour
{
    public enum PlayerState
    {
        idle,
        attack,
    }

    [SerializeField] private PlayerState currentState;
    [SerializeField] private PlayerState prevState;
    public PlayerStatScriptableObject playerData;
    public MiniGameLandmark landmark;
    public bool isAttacking;
    public string currentAnimation;

    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, attack;

    private void Start()
    {
        landmark = GameObject.FindGameObjectWithTag("Landmark").GetComponent<MiniGameLandmark>();
        skeletonAnimation.AnimationState.Event += OnSpineEvent;
        currentState = PlayerState.idle;
    }

    void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        string eventName = e.Data.Name;
        if (eventName == "damage")
        {
            //Call the function that you want here
            Attack();
            //PlaySFX();
        }
    }

    public void TriggerAttack()
    {
        if (!currentState.Equals(PlayerState.attack))
        {
            prevState = currentState;
        }
        isAttacking = true;
    }

    public void Attack()
    {
        if(landmark != null)
        {
            landmark.TakeDamage(playerData.attackDamage);
            isAttacking = false;
        }
        else { return; }
    }

    public void SetCharacterState(PlayerState states)
    {
        if (states.Equals(PlayerState.idle))
        {
            SetAnimation(0, idle, true, 1f);
        }

        if (states.Equals(PlayerState.attack))
        {
            SetAnimation(0, attack, true, 1.4f);
        }
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
        if (isAttacking )
        {
            isAttacking = false;
        }

        else
        {
            return;
        }

        SetCharacterState(prevState);
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // A touch has just begun
                TriggerAttack();
            }
        }

        if (!isAttacking)
        {
            SetCharacterState(PlayerState.idle);
        }
        else
        {
            SetCharacterState(PlayerState.attack);
        }
    }
}
