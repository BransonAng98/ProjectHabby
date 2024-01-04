using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;
using Spine;
using Haptics.Vibrations;

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

    [SerializeField] float animationSpeed;
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, attack1, attack2, attack3, ultimate, victory;

    //Hit Counts
    [SerializeField] int currentHitCount;
    [SerializeField] int ultimateHitThreshold;
    [SerializeField] bool lastTouch;
    public int hitCount;
    private TextMeshProUGUI hitCountDisplay;
    private TextMeshProUGUI hitFeedbackDisplay;

    public float hideUITextDelay;
    private float timeSinceLastTap = 0f;
    [SerializeField] bool hasInput;
    [SerializeField] CameraShake camShake;

    [SerializeField] private float idleRoarTimer = 0f;
    private float minRoarThreshold = 6f;
    private float maxRoarThreshold = 10f;
    private float varTime;
    public AudioSource roarAudioSource;
    public AudioClip monsterRoarSFX;

    private void Start()
    {
        landmark = GameObject.FindGameObjectWithTag("Landmark").GetComponent<MiniGameLandmark>();
        skeletonAnimation.AnimationState.Event += OnSpineEvent;
        currentState = PlayerState.idle;
        hitCountDisplay = GameObject.Find("HitCount").GetComponent<TextMeshProUGUI>();
        hitCountDisplay.gameObject.SetActive(false);
        hitFeedbackDisplay = GameObject.Find("HitFlavorText").GetComponent<TextMeshProUGUI>();
        hitFeedbackDisplay.gameObject.SetActive(false);
        camShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
        varTime = Random.Range(minRoarThreshold, maxRoarThreshold);
        VibrateHaptics.Initialize();
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
            camShake.ShakeCamera();
            Invoke("StopShake", 1f);
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
    void StopShake()
    {
        camShake.StopShaking();
    }

    public void Attack()
    {
        if (landmark != null)
        {
            VibrateHaptics.VibrateHeavyClick();
            hitCount++;
            currentHitCount++;
            hitCountDisplay.text = currentHitCount.ToString() + " Hits";
            landmark.TakeDamage(playerData.attackDamage);
            VibrateHaptics.Release();
            if (hitCountDisplay.gameObject.activeSelf)
            {
                hitCountDisplay.GetComponent<TextStretchAndSquash>().StretchSquashAnimation();
            }

            else
            {
                return;
            }

        }
        else { return; }
    }

    void SetAttackSpeed(float hitCount)
    {
        if (hitCount >= 0)
        {
            animationSpeed = 1.6f;
        }

        if (hitCount > 5)
        {
            animationSpeed = 2.2f;
        }

        if (hitCount > 10)
        {
            animationSpeed = 2.8f;
        }

        if (hitCount > 15)
        {
            animationSpeed = 3.4f;
        }

        if (hitCount > 25)
        {
            animationSpeed = 4f;
        }
    }

    void UpdateHitFlavorText()
    {
        StartCoroutine(FadeOutText(hitCountDisplay));

        currentHitCount = 0;
        //if (hasInput)
        //{
        //    StartCoroutine(FadeOutText(hitFeedbackDisplay));
        //}

        //else
        //{
        //    Invoke("TurnOffText", 3f);
        //}
    }

    IEnumerator FadeOutText(TextMeshProUGUI text)
    {
        // Adjust the duration based on your preference
        float duration = 1.5f;

        // Record the starting color
        Color startColor = text.color;

        // Calculate the end color with zero alpha
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Initialize the elapsed time
        float elapsedTime = 0f;

        // Gradually interpolate between the starting and ending colors
        while (elapsedTime < duration)
        {
            text.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final color is set
        text.color = endColor;

        // Destroy the text after fading out
        text.gameObject.SetActive(false);

        if (!hitCountDisplay.gameObject.activeSelf)
        {
            hitFeedbackDisplay.gameObject.SetActive(true);
            hitFeedbackDisplay.GetComponent<TextStretchAndSquash>().StretchSquashAnimation();
        }
    }

    void TurnOffText()
    {
        hitFeedbackDisplay.gameObject.SetActive(false);
    }

    public void SetCharacterState(PlayerState states)
    {
        if (states.Equals(PlayerState.idle))
        {
            SetAnimation(0, idle, true, 1f);
        }

        if (states.Equals(PlayerState.attack))
        {
            SetAnimation(0, attack3, false, animationSpeed);
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

    void DisplayFlavorText()
    {
        if (currentHitCount == 5)
        {
            hitFeedbackDisplay.color = new Color(hitFeedbackDisplay.color.r, hitFeedbackDisplay.color.g, hitFeedbackDisplay.color.b, 1f);
            hitFeedbackDisplay.gameObject.SetActive(true);
            hitFeedbackDisplay.GetComponent<TextStretchAndSquash>().StretchSquashAnimation();
            hitFeedbackDisplay.text = "Getting there...";
            StartCoroutine(FadeOutText(hitFeedbackDisplay));
        }

        if (currentHitCount == 10)
        {
            hitFeedbackDisplay.color = new Color(hitFeedbackDisplay.color.r, hitFeedbackDisplay.color.g, hitFeedbackDisplay.color.b, 1f);
            hitFeedbackDisplay.gameObject.SetActive(true);
            hitFeedbackDisplay.GetComponent<TextStretchAndSquash>().StretchSquashAnimation();
            hitFeedbackDisplay.text = "DEVASTATION";
            StartCoroutine(FadeOutText(hitFeedbackDisplay));
        }

        if (currentHitCount == 15)
        {
            hitFeedbackDisplay.color = new Color(hitFeedbackDisplay.color.r, hitFeedbackDisplay.color.g, hitFeedbackDisplay.color.b, 1f);
            hitFeedbackDisplay.gameObject.SetActive(true);
            hitFeedbackDisplay.GetComponent<TextStretchAndSquash>().StretchSquashAnimation();
            hitFeedbackDisplay.text = "CALL THEM A NEW HOME";
            StartCoroutine(FadeOutText(hitFeedbackDisplay));
        }

        if (currentHitCount == 20)
        {
            hitFeedbackDisplay.color = new Color(hitFeedbackDisplay.color.r, hitFeedbackDisplay.color.g, hitFeedbackDisplay.color.b, 1f);
            hitFeedbackDisplay.gameObject.SetActive(true);
            hitFeedbackDisplay.GetComponent<TextStretchAndSquash>().StretchSquashAnimation();
            hitFeedbackDisplay.text = "END OF THE WORLD";
            StartCoroutine(FadeOutText(hitFeedbackDisplay));
        }

        if (currentHitCount == 25)
        {
            hitFeedbackDisplay.color = new Color(hitFeedbackDisplay.color.r, hitFeedbackDisplay.color.g, hitFeedbackDisplay.color.b, 1f);
            hitFeedbackDisplay.gameObject.SetActive(true);
            hitFeedbackDisplay.GetComponent<TextStretchAndSquash>().StretchSquashAnimation();
            hitFeedbackDisplay.text = "Its pinching time";
            StartCoroutine(FadeOutText(hitFeedbackDisplay));
        }

        if (currentHitCount == 30)
        {
            hitFeedbackDisplay.color = new Color(hitFeedbackDisplay.color.r, hitFeedbackDisplay.color.g, hitFeedbackDisplay.color.b, 1f);
            hitFeedbackDisplay.gameObject.SetActive(true);
            hitFeedbackDisplay.GetComponent<TextStretchAndSquash>().StretchSquashAnimation();
            hitFeedbackDisplay.text = "Stop being so crabby";
            StartCoroutine(FadeOutText(hitFeedbackDisplay));
        }

        if (currentHitCount == 35)
        {
            hitFeedbackDisplay.color = new Color(hitFeedbackDisplay.color.r, hitFeedbackDisplay.color.g, hitFeedbackDisplay.color.b, 1f);
            hitFeedbackDisplay.gameObject.SetActive(true);
            hitFeedbackDisplay.GetComponent<TextStretchAndSquash>().StretchSquashAnimation();
            hitFeedbackDisplay.text = "Crabtacular";
            StartCoroutine(FadeOutText(hitFeedbackDisplay));
        }
    }

    private void Update()
    {
        PlayRoar();
        timeSinceLastTap += Time.deltaTime;


        if (landmark != null)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    SetAttackSpeed(currentHitCount);
                    hasInput = true;
                    // A touch has just begun
                    TriggerAttack();
                    timeSinceLastTap = 0f;
                }
            }

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

        if (currentHitCount >= 1)
        {
            hitCountDisplay.color = new Color(hitCountDisplay.color.r, hitCountDisplay.color.g, hitCountDisplay.color.b, 1f);
            hitCountDisplay.gameObject.SetActive(true);
            DisplayFlavorText();
            if (timeSinceLastTap > hideUITextDelay)
            {
                hasInput = false;
                UpdateHitFlavorText();
            }
        }
    }

    public void PlayRoar()
    {
        idleRoarTimer += Time.deltaTime;

        if (idleRoarTimer > varTime)
        {
            roarAudioSource.PlayOneShot(monsterRoarSFX);
            varTime = Random.Range(minRoarThreshold, maxRoarThreshold);
            idleRoarTimer = 0f;
        }
    }
}
