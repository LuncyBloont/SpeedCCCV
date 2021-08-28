using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public float distance;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tp = target.position + offset;
        transform.LookAt(tp);
        RaycastHit raycastHit;
        if (Physics.Raycast(tp, transform.position - tp,
            out raycastHit, distance, (~LayerMask.NameToLayer("Default") & LayerMask.NameToLayer("CAR"))))
        {
            transform.position = tp + (transform.position - tp).normalized * raycastHit.distance;
        }
        else
        {
            transform.position = tp + (transform.position - tp).normalized * distance;
        }
    }
}
