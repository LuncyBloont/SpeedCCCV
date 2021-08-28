using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAngle : MonoBehaviour
{
    public KeyCode rk;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(rk))
        {
            transform.rotation = new Quaternion();
        }
    }
}
