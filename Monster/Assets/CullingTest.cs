using System.Collections;
using UnityEngine;

public class CullingTest : MonoBehaviour
{
    private Transform player;
    public Transform body;
    public float cullingDistance = 20f;
    public float checkInterval = 0.5f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
    }
    private void Start()
    {
        StartCoroutine(CheckDistance());
    }
    
    private IEnumerator CheckDistance()
    {
        while (true)
        {
            float distance = Vector3.Distance(body.transform.position, player.position);

            if (distance < cullingDistance)
            {
                body.gameObject.SetActive(true);
            }
            else
            {
                body.gameObject.SetActive(false);
            }
            
            yield return new WaitForSeconds(checkInterval);
        }
    }
}