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
        ultimate,
        victory,
    }

    [SerializeField] private PlayerState currentState;
    [SerializeField] private PlayerState prevState;
    public PlayerStatScriptableObject playerData;
    public MiniGameLandmark landmark;
    public bool canAttack;
    public string currentAnimation;

    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, attack, ultimate, victory;

    //Hit Counts
    [SerializeField] int currentHitCount;
    [SerializeField] int ultimateHitThreshold;
    [SerializeField] bool lastTouch;
    public int hitCount;
    private TextMeshProUGUI hitCountDisplay;

    private void Start()
    {
        landmark = GameObject.FindGameObjectWithTag("Landmark").GetComponent<MiniGameLandmark>();
        skeletonAnimation.AnimationState.Event += OnSpineEvent;
        currentState = PlayerState.idle;
        hitCountDisplay = GameObject.Find("HitCount").GetComponent<TextMeshProUGUI>();
        hitCountDisplay.gameObject.SetActive(false);
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


        if (eventName == "land")
        {
            //Call the function that you want here
            Attack();
            //PlaySFX();
        }
    }

    public void TriggerAttack()
    {
        if (!canAttack)
        {
            canAttack = true;

            if (!currentState.Equals(PlayerState.attack) || !currentState.Equals(PlayerState.ultimate))
            {
                prevState = currentState;
            }
        }
    }

    public void Attack()
    {
        if(landmark != null)
        {
            hitCount++;
            currentHitCount++;
            landmark.TakeDamage(playerData.attackDamage);
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
            SetAnimation(0, attack, false, 1.4f);
        }

        if (states.Equals(PlayerState.ultimate))
        {
            SetAnimation(0, ultimate, false, 1f);
        }

        if (states.Equals(PlayerState.victory))
        {
            SetAnimation(0, victory, true, 1f);
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
        if (canAttack)
        {
            canAttack = false;
        }
        SetCharacterState(prevState);
    }

    private void Update()
    {
        hitCountDisplay.text = currentHitCount.ToString();

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // A touch has just begun
                TriggerAttack();
            }
        }

        if(landmark != null)
        {
            if (!canAttack)
            {
                SetCharacterState(PlayerState.idle);
            }
            else
            {
                if (hitCount % ultimateHitThreshold == 0)
                {
                    if (hitCount == 0)
                    {
                        SetCharacterState(PlayerState.attack);
                    }

                    else
                    {
                        SetCharacterState(PlayerState.ultimate);
                    }
                }
                else
                {
                    SetCharacterState(PlayerState.attack);
                }
            }
        }

        else
        {
            SetCharacterState(PlayerState.victory);
        }
        
        if(currentHitCount >= 3)
        {
            hitCountDisplay.gameObject.SetActive(true);
        }
    }
}
