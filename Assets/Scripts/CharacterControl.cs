using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public int life;

    [Header("Movement Variables")]
    public float jumpPower;
    public bool isRunning;
    public bool isRollback;
    public float rollbackDuration;

    [Header("Turning Variables")]
    public float lerpTime;
    public float laneXPosition;
    float lerpPos;
    float lastLaneChange;
    int lanePos = 0;
    Rigidbody rbody;
    Animator anim;
    bool isJumping, isTurningRight, isTurningLeft;

    [Header("SFX")]
    public AudioSource coinSFX;
    public AudioSource impactSFX;
    public AudioSource jumpSFX;
    public AudioSource stepSFX;
    public AudioSource turnSFX;


    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isJumping == true)
        {
            if(rbody.velocity.magnitude <= 0.1f)
            {
                jumpSFX.Play();
                rbody.AddForce(0, jumpPower, 0);
                anim.SetTrigger("jump");
            }
            isJumping = false;
        }

        if(isTurningLeft)
        {
            if(lanePos - 1 >= -1)
            {
                turnSFX.Play();
                lastLaneChange = Time.time;
                lanePos -= 1;
                lerpPos -= laneXPosition;
            }
            isTurningLeft = false;
        }

        if(isTurningRight)
        {
            if(lanePos + 1 <= 1)
            {
                turnSFX.Play();
                lastLaneChange = Time.time;
                lanePos += 1;
                lerpPos += laneXPosition;
            }
            isTurningRight = false;
        }

        float lerp = Mathf.Lerp(transform.position.x, lerpPos, (Time.time - lastLaneChange) / lerpTime);
        transform.position = new Vector3(lerp, transform.position.y, transform.position.z);
    }

    public void Jump()
    {
        if (isRunning == false) return;
        isJumping = true;
    }

    public void Turn(int nextPos)
    {
        if (isRunning == false) return;
        if(nextPos == 1)
        {
            isTurningLeft = false;
            isTurningRight = true;
        } else
        {
            isTurningLeft = true;
            isTurningRight = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "obstacle")
        {
            if(transform.position.y < 1)
            {
                Flinch();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "coin") 
        {
            coinSFX.Play();
            other.gameObject.SetActive(false);
            GameControl.current.AddScore(1);
        }
    }

    void Flinch()
    {
        impactSFX.Play();
        anim.SetTrigger("fall");
        isRunning = false;
        life--;

        GameControl.current.lifePanel.GetChild(life).gameObject.SetActive(false);

        if(life <= 0)
        {
            anim.SetBool("dead", true);
            GameControl.current.Invoke("GameOver", 2);

        }
    }

    public void StartRun()
    {
        isRunning = true;
        anim.SetTrigger("run");
    }

    public void StartRollback()
    {
        isRollback = true;
        Invoke("StopRollback", rollbackDuration);
    }

    void StopRollback()
    {
        isRollback = false;
        anim.SetTrigger("idle");
        GameControl.current.StartCountdown();
    }

    public void PlayStepSFX()
    {
        stepSFX.Play();
    }
}
