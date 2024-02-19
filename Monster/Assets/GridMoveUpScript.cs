using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMoveUpScript : MonoBehaviour
{
    [SerializeField]
    private int movespeed;
    // Start is called before the first frame update
    void Start()
    {
        movespeed = 10;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * movespeed * Time.deltaTime);
    }
}
