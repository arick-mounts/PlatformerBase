using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    public Animator anim;
    public MovementController playerMovement;
    public Rigidbody playerRB;

    int velocityHash = Animator.StringToHash("Velocity");

    // Start is called before the first frame update
    void Start()
    {
        
            
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat(velocityHash, playerRB.velocity.magnitude);
    }


}
