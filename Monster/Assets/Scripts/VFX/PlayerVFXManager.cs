using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXManager : MonoBehaviour
{
    public PlayerHandler inputHandler;
    public GameObject impactVFX;
    public GameObject aoeVFX;
    public GameObject ultiRdyVFX;
    public GameObject ultimateVFX;
    public GameObject deathVFX;
    public GameObject dashFootVFX;
    public GameObject upgradeVFX;
    public GameObject dashBodyVFX;
    public GameObject blackSquare;
    public GameObject rageOnTxt;
    public GameObject rageOffTxt;

    private GameObject player;

    public float deathVFXRadius;
    public float footTremorRadius;
    public float aoeTremorRadius;
    public int numberOfVFX = 3;

    public float fadeDuration;

    public float moveSpeed;
    public float rageFadeDuration;
    public float delayRageFade;

    public SpriteRenderer objectRenderer;
    private Color targetColor;
    private Color initialColor;

    public GameObject[] legLocations;

    [SerializeField] private bool isTriggered;
    public bool isDashing;
    public bool hasAppeared;

    private PlayerHandler playerHandler;

    private void Start()
    {
        objectRenderer = GameObject.Find("BlackSquare").GetComponent<SpriteRenderer>();

        initialColor = objectRenderer.color;
        playerHandler = GetComponent<PlayerHandler>();
        isTriggered = false;
    }

    public void footImpact(int foot)
    {
        Vector2 correction = new Vector2(legLocations[foot].transform.position.x, legLocations[foot].transform.position.y);

        if (!isDashing)
        {
            Instantiate(impactVFX, correction, Quaternion.identity);
        }
        else
        {
            Instantiate(dashFootVFX, correction, Quaternion.identity);
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(correction, footTremorRadius);
        foreach (Collider2D colldier in hitColliders)
        {
            if (colldier.CompareTag("Tree"))
            {
                ObjectShakeScript tree = colldier.GetComponent<ObjectShakeScript>();
                tree.StartShake();
            }
        }
    }

    public void TriggerAoeTremor()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, aoeTremorRadius);
        foreach (Collider2D colldier in hitColliders)
        {
            if (colldier.CompareTag("Tree"))
            {
                ObjectShakeScript tree = colldier.GetComponent<ObjectShakeScript>();
                tree.StartShake();
            }
        }
    }

    public void SpawnDeathVFX()
    {
        if (!isTriggered)
        {
            for (int i = 0; i < numberOfVFX; i++)
            {
                isTriggered = true;
                Vector3 spawnLoc = new Vector3(transform.position.x, transform.position.y + 2f);
                Vector3 randomPosition = spawnLoc + Random.insideUnitSphere * deathVFXRadius;
                Instantiate(deathVFX, randomPosition, Quaternion.identity);
            }
        }
    }

    public void SpawnAoeVFX()
    {
        Vector2 aoePos = new Vector2(transform.position.x, transform.position.y - 1f);
        GameObject aoe = Instantiate(aoeVFX, aoePos, Quaternion.identity);
    }

    public void SpawnUltiVFX()
    {
        Vector2 ultiPos = new Vector2(this.transform.position.x, this.transform.position.y - 2.5f);
        GameObject ultiVFX = Instantiate(ultimateVFX, ultiPos, Quaternion.identity);
    }

    public void SpawnUpgradeVFX()
    {
        Vector2 upgradePos = new Vector2(this.transform.position.x, this.transform.position.y + 2f);
        Instantiate(upgradeVFX, upgradePos, Quaternion.identity);
    }

    public void SpawnRageOnText()
    {
        Vector2 spawnPosition = new Vector2(transform.position.x, transform.position.y + 5f);
        GameObject rageOnObject = Instantiate(rageOnTxt, spawnPosition, Quaternion.identity);
        StartCoroutine(MoveUpAndFade(rageOnObject));
    }

    private IEnumerator MoveUpAndFade(GameObject obj)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("The spawned object must have a SpriteRenderer component.");
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < rageFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f; // Reset elapsed time for fading

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Move the object upward
            obj.transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);

            // Fade the object over time
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            yield return null;
        }

        Destroy(obj);
    }

    public void SpawnRageOffText()
    {
        Vector2 spawnPosition = new Vector2(transform.position.x, transform.position.y + 8f);
        GameObject rageOnObject = Instantiate(rageOffTxt, spawnPosition, Quaternion.identity);
        StartCoroutine(MoveDownAndFade(rageOnObject));
    }

    private IEnumerator MoveDownAndFade(GameObject obj)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("The spawned object must have a SpriteRenderer component.");
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < delayRageFade)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f; // Reset elapsed time for fading

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Move the object downward
            obj.transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);

            // Fade the object over time
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            yield return null;
        }

        Destroy(obj);
    }


    public void StartAppearing()
    {
        StartCoroutine(FadeObject(0.5f)); // 0.5f is the target alpha value for fade in
    }

    public void StartFading()
    {
        StartCoroutine(FadeObject(0f)); // 0f is the target alpha value for fade out
    }

    private IEnumerator FadeObject(float targetAlpha)
    {
        float startAlpha = objectRenderer.color.a; // Initial alpha value

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            objectRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, currentAlpha);
            yield return null;
        }

        objectRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha); // Ensure it reaches the target color exactly

        if (targetAlpha == 0f)
        {
            hasAppeared = false;
        }
        else 
        {
            hasAppeared = true;
        }
    }

}

