using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLinear : MonoBehaviour
{

    public Transform[] TargetPoints;
    public int currentPoint = 0;
    public float tolerance;
    public float speed;
    public AnimationCurve speedCurve;
    public bool automatic;
    public bool loop = false;
    public float delay_time = 2f;
    public int direction = 1;

    private Vector3 currentTarget;
    private Vector3 previousPoint;
    private float delay_start;
    private bool atTarget;
    private Vector3 currentVelocity;



    public static float getPercentageAlong(Vector3 a, Vector3 b, Vector3 c)
    {
        var ab = b - a;
        var ac = c - a;
        return Vector3.Dot(ac, ab) / ab.sqrMagnitude;
    }

    // Start is called before the first frame update
    void Start()
    {
        previousPoint = transform.position;
        atTarget = false;
        if (TargetPoints.Length > 0)
        {
            currentTarget = TargetPoints[0].position;
            transform.position = currentTarget;
            atTarget = true;
        }
        tolerance = speed * Time.deltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!atTarget)
        {
            movePlatform();
        }
        else
        {
            updateTarget();
        }

        
    
        
    }

    

    void movePlatform()
    {
        Vector3 previousPosition = transform.position;
        Vector3 heading = currentTarget - transform.position;

        transform.position += heading.normalized * speed *  speedCurve.Evaluate(getPercentageAlong(currentTarget, previousPoint, transform.position)) * Time.deltaTime;
        if (heading.magnitude <= tolerance)
        {
            transform.position = currentTarget;
            atTarget = true;
            delay_start = Time.time;
        }
        currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
    }

    void updateTarget()
    {
        if (automatic)
        {
            if (Time.time - delay_start > delay_time)
            {

                if (loop == false)
                {
                    currentPoint += direction;
                    if (currentPoint == TargetPoints.Length - 1)
                    {
                        direction = -1;
                    }
                    else if (currentPoint == 0)
                    {
                        direction = 1;
                    }
                    previousPoint = currentTarget;
                    currentTarget = TargetPoints[currentPoint].position;
                    atTarget = false;
                }
                else
                {
                    currentPoint += direction;
                    if (currentPoint == TargetPoints.Length)
                    {
                        currentPoint = 0;
                    }
                    if(currentPoint == -1)
                    {

                        currentPoint = TargetPoints.Length - 1;
                    }
                    previousPoint = currentTarget;
                    currentTarget = TargetPoints[currentPoint].position;
                    atTarget = false;
                }
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<Rigidbody>() != null)
        {
            col.transform.parent = transform;
        }
        
    }
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.GetComponent<Rigidbody>() != null)
        {
            col.transform.parent = null;
            col.GetComponent<Rigidbody>().velocity += currentVelocity;
        }
    }
    
}
