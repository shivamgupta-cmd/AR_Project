using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public float Adjust;
    Transform Player;

    private void Start()
    {
        Player = GameObject.FindWithTag("MainCamera").transform;
    }
    void Update()
    {
        if (Player != null) 
        {
            transform.LookAt(Player);
            transform.eulerAngles = new Vector3(0,transform.eulerAngles.y + Adjust, 0);
        }
    }
}
