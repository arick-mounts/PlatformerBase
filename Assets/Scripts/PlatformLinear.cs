using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLinear : MonoBehaviour
{

    public Transform[] TargetPoints;
    public int currentPoint = 0;
    public float tolerance;
    public float speed;
    public AnimationCurve forceCurve;
    public bool automatic;

    private Vector3 currentTarget;
    private Rigidbody rb;
    private float delay_time;
    private int direction = 1;
    private float delay_start;
    private bool atTarget;
    private Rigidbody collidedRB;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        if (TargetPoints.Length > 0)
        {
            currentTarget = TargetPoints[0].position;
        }
        tolerance = speed * Time.deltaTime;
        atTarget = false;
        if (transform.position == currentTarget)
        {
            atTarget = true;
        }
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

        /*if (collidedRB)
        {
            //if (Vector3.Distance(collidedRB.velocity.normalized, rb.velocity.normalized) < 1.2f)
            collidedRB.velocity = rb.velocity + velocity; // Vector3.ClampMagnitude(collidedRB.velocity + , rb.velocity.magnitude + 10f);
        }*/
    
        
    }

    

    void movePlatform()
    {
        Vector3 heading = currentTarget - transform.position;

        rb.AddForce( heading.normalized * speed * Time.deltaTime, ForceMode.VelocityChange);
        if (heading.magnitude <= tolerance)
        {
            transform.position = currentTarget;
            atTarget = true;
            delay_start = Time.time;
        }
    }

    void updateTarget()
    {
        if (automatic)
        {
            if (Time.time - delay_start > delay_time)
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
                currentTarget = TargetPoints[currentPoint].position;
                atTarget = false;
            }
        }
    }

    /*void OnTriggerStay(Collider col)
    {
        if (col.tag == "player") 
        { 
            collidedRB = col.gameObject.GetComponent<Rigidbody>();
            velocity = collidedRB.velocity;
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "player")
        {
            collidedRB = null;
        }
    }*/
    
}
