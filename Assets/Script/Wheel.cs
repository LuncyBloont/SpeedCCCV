using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float height = 0.7f;
    public Transform carBody;
    public float bounce = 1f;
    public float maxBounceForce = 100;
    public Rigidbody car;
    public Vector3 drag = new Vector3(0.9f, 0.7f, 0.9f);
    public Vector3 angleDrag = new Vector3(0.7f, 0.9f, 0.7f);
    public float force = 300;
    public float angleForce = 200;

    Vector3[] wheels;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        wheels = new Vector3[4];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        wheels[0] = transform.localPosition;
        wheels[1] = new Vector3(-wheels[0].x, wheels[0].y, wheels[0].z);
        wheels[2] = new Vector3(wheels[0].x, wheels[0].y, -wheels[0].z);
        wheels[3] = new Vector3(-wheels[0].x, wheels[0].y, -wheels[0].z);

        Control();

        foreach (var i in wheels)
        {
            Vector3 worldPos = carBody.transform.position +
                i.x * carBody.right + i.y * carBody.up + i.z * carBody.forward;
            Vector3 dir = (-carBody.up * height).normalized;
            Debug.DrawRay(worldPos, dir * height, Color.red);

            RaycastHit raycastHit;
            bool hit = Physics.Raycast(worldPos, dir, out raycastHit, height, ~LayerMask.NameToLayer("Default"));
            if (hit)
            {
                car.AddForceAtPosition(-Mathf.Min(bounce / raycastHit.distance, maxBounceForce)
                    * car.mass * dir, worldPos);
                car.velocity = new Vector3(
                    car.velocity.x * drag.x, 
                    car.velocity.y * drag.y, 
                    car.velocity.z * drag.z
                );
                car.angularVelocity = new Vector3(
                    car.angularVelocity.x * angleDrag.x, 
                    car.angularVelocity.y * angleDrag.y, 
                    car.angularVelocity.z * angleDrag.z
                );
            }
        }
    }

    void Control()
    {
        if (Input.GetKey(KeyCode.W))
        {
            car.AddForce(carBody.forward * force * car.mass);
        }
        if (Input.GetKey(KeyCode.S))
        {
            car.AddForce(-carBody.forward * force * car.mass);
        }
        if (Input.GetKey(KeyCode.A))
        {
            car.AddTorque(new Vector3(0, -1, 0) * angleForce * car.mass);
        }
        if (Input.GetKey(KeyCode.D))
        {
            car.AddTorque(new Vector3(0, 1, 0) * angleForce * car.mass);
        }
    }
}
