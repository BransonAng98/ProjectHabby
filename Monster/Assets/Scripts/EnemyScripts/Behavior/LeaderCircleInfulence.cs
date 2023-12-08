using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderCircleInfulence : MonoBehaviour
{
    [SerializeField] Leader leader;
    // Start is called before the first frame update
    void Start()
    {
        leader = GetComponentInParent<Leader>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Civilian"))
        {
            Civilian civiRecruited = collision.gameObject.GetComponentInChildren<Civilian>();
            if (civiRecruited != null)
            {
                leader.AddFollowers(civiRecruited);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Civilian"))
        {
            Civilian civiRecruited = collision.gameObject.GetComponentInChildren<Civilian>();
            if (civiRecruited != null)
            {
                civiRecruited.RemoveCivilan();
                leader.followerList.Remove(civiRecruited);
            }
        }
    }
}
