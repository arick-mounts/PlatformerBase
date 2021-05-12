using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    public Animator anim;
    public Renderer ball;

    private float radius;

    int velocityHash = Animator.StringToHash("Velocity");
    int LRDirectionHash = Animator.StringToHash("Left/Right");
    int FBDirectionHash = Animator.StringToHash("Front/Back");
    int RollSpeedHash = Animator.StringToHash("RollSpeed");

    // Start is called before the first frame update
    void Start()
    {
        radius = ball.bounds.extents.magnitude;
            
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void setVelocityAnim(float vel)
    {
        anim.SetFloat(velocityHash, vel);
        anim.SetFloat(RollSpeedHash, (vel/radius) / 4);


    }

    public void setDirectionAnim(float LRdir,float FBdir)
    {
        anim.SetFloat(LRDirectionHash, LRdir, .3f, Time.deltaTime);
        anim.SetFloat(FBDirectionHash, FBdir, .3f, Time.deltaTime);

    }


}
