using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float height = 0.7f;
    public float minHeight = 0.5f;
    public Transform carBody;
    public float bounce = 1f;
    public float maxBounceForce = 100;
    public Rigidbody car;
    public Vector3 drag = new Vector3(0.9f, 0.7f, 0.9f);
    public Vector3 angleDrag = new Vector3(0.7f, 0.9f, 0.7f);
    public float force = 300;
    public float angleForce = 200;
    public float turnSpeed = 1;
    public float maxAngleSpeed = 45;
    public float forceToMoveFromTurn = 0.3f;
    public float heightFix = 0.7f;
    public float maxSpeed = 10;
    public GameObject model;
    public float modelPosition = 0.6f;
    public float maxDeg = 30;
    public bool[] rotatable = new bool[4];
    public Vector3 rotateDir = new Vector3(-1, 0, 0);

    GameObject[] mods;
    Vector3[] wheels;
    float turnAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        wheels = new Vector3[4];
        mods = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            mods[i] = Instantiate(model);
            mods[i].transform.parent = car.gameObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int wi = 0;
        foreach (var i in wheels)
        {
            Vector3 worldPos = carBody.transform.position +
                i.x * carBody.right + i.y * carBody.up + i.z * carBody.forward;
            Vector3 dir = (-carBody.up * height).normalized;
            RaycastHit raycastHit;
            bool hit = Physics.Raycast(worldPos, dir, out raycastHit, height, 
                ~LayerMask.NameToLayer("Default"));
            float dis = height;
            if (hit)
            {
                if (raycastHit.distance > minHeight)
                    Debug.DrawRay(worldPos, raycastHit.distance * dir, new Color(0.3f, 1f, 0.5f, 1));
                else
                    Debug.DrawRay(worldPos, raycastHit.distance * dir, new Color(1f, 0.5f, 0.5f, 1));
                dis = Mathf.Max(minHeight, raycastHit.distance);
            }
            else
            {
                Debug.DrawRay(worldPos, dir * height, Color.gray);
            }

            mods[wi].transform.position = worldPos + dir * (dis - height * modelPosition);
            mods[wi].transform.localScale = new Vector3(
                mods[wi].transform.localScale.x,
                mods[wi].transform.localScale.y,
                Mathf.Abs(mods[wi].transform.localScale.z)
                * Mathf.Sign(Vector3.Dot(carBody.right, mods[wi].transform.position - carBody.position))
            ); 
            wi++;
        }
    }

    void FixedUpdate()
    {
        wheels[0] = transform.localPosition;
        wheels[1] = new Vector3(-wheels[0].x, wheels[0].y, wheels[0].z);
        wheels[2] = new Vector3(wheels[0].x, wheels[0].y, -wheels[0].z);
        wheels[3] = new Vector3(-wheels[0].x, wheels[0].y, -wheels[0].z);

        Vector3 localVelocity = new Vector3(
            Vector3.Dot(car.velocity, carBody.right),
            Vector3.Dot(car.velocity, carBody.up),
            Vector3.Dot(car.velocity, carBody.forward)
        );

        float num = 0;
        int n = 0;
        int wi = 0;
        foreach (var i in wheels)
        {
            Vector3 worldPos = carBody.transform.position +
                i.x * carBody.right + i.y * carBody.up + i.z * carBody.forward;
            Vector3 dir = (-carBody.up * height).normalized;

            RaycastHit raycastHit;
            bool hit = Physics.Raycast(worldPos, dir, out raycastHit, height, 
                ~LayerMask.NameToLayer("Default"));
            if (hit)
            {
                float power = (height - raycastHit.distance) / (height - minHeight);
                car.AddForceAtPosition(-Mathf.Min(bounce * power, maxBounceForce)
                    * car.mass * dir, worldPos);

                float dpower = Mathf.Pow(power, heightFix);
                localVelocity = new Vector3(
                    localVelocity.x * drag.x, 
                    localVelocity.y * drag.y, 
                    localVelocity.z * drag.z
                ) * dpower + localVelocity * (1 - dpower);
                car.angularVelocity = (
                    Vector3.Dot(car.angularVelocity, carBody.right) * 
                    angleDrag.x * carBody.right + 
                    Vector3.Dot(car.angularVelocity, carBody.up) * 
                    angleDrag.y * carBody.up +
                    Vector3.Dot(car.angularVelocity, carBody.forward) * 
                    angleDrag.z * carBody.forward
                ) * dpower + car.angularVelocity * (1 - dpower);
                num += power;
                n += 1;
            }

            mods[wi].transform.GetChild(0).transform.Rotate(rotateDir * localVelocity.z);
            Vector3 lookTo = carBody.right;
            if (rotatable[wi])
            {
                lookTo = carBody.right * Mathf.Cos(maxDeg * turnAngle / 180f * Mathf.PI) +
                    carBody.forward * Mathf.Sin(maxDeg * turnAngle / 180f * Mathf.PI);
            }
            mods[wi].transform.LookAt(mods[wi].transform.position + lookTo);
            wi++;
        }

        localVelocity.z = Mathf.Min(localVelocity.z, maxSpeed * (4 - num));

        car.velocity = localVelocity.x * carBody.right + 
            localVelocity.y * carBody.up + 
            localVelocity.z * carBody.forward;

        Control(num, n >= 1);
    }

    void Control(float num, bool two)
    {
        bool turnning = false;
        if (Input.GetKey(KeyCode.A))
        {
            turnAngle += turnSpeed * Time.fixedDeltaTime;
            turnning = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            turnAngle -= turnSpeed * Time.fixedDeltaTime;
            turnning = true;
        }

        if (turnAngle < -1) turnAngle = -1;
        if (turnAngle > 1) turnAngle = 1;

        if (!turnning)
        {
            if (Mathf.Abs(turnAngle) > turnSpeed * Time.fixedDeltaTime)
                turnAngle -= Mathf.Sign(turnAngle) * turnSpeed * Time.fixedDeltaTime;
            else
                turnAngle = 0;
        }

        if (Input.GetKey(KeyCode.W))
        {
            car.AddForce(carBody.forward * force * car.mass * num * 
                (1 + Mathf.Abs(turnAngle) * forceToMoveFromTurn));
        }
        if (Input.GetKey(KeyCode.S))
        {
            car.AddForce(-carBody.forward * force * car.mass * num * 
                (1 + Mathf.Abs(turnAngle) * forceToMoveFromTurn));
        }

        float asp = angleForce * turnAngle * Vector3.Dot(car.velocity, carBody.forward) * (two ? 1 : 0);
        if (asp < -maxAngleSpeed) asp = -maxAngleSpeed;
        if (asp > maxAngleSpeed) asp = maxAngleSpeed;
        car.transform.Rotate(new Vector3(0, -1, 0) * asp * Time.fixedDeltaTime);
    }
}
