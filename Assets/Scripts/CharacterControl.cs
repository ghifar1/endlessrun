using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    #region Event

    public Action<bool> Hit;
    public Action<bool> Countdown;
    public Action Score;
    
    #endregion
    
    [Header("Unity Component")]
    private Rigidbody rbody;
    public Animator anim;

    [Header("Movement Variables")]
    [SerializeField]private float jumpPower;
    public bool isRunning;
    public bool isRollback;
    [SerializeField] private float rollbackDuration;

    [Header("Turning Variables")]
    [SerializeField]private float lerpTime;
    [SerializeField]private float laneXPosition;
    float lerpPos;
    float lastLaneChange;
    int lanePos = 0;

    private bool isJumping, isTurningRight, isTurningLeft;

    public bool isHit = false;
    public bool isStartCountdown;

    [Header("SFX")]
    [SerializeField]private AudioSource coinSFX;
    [SerializeField]private AudioSource impactSFX;
    [SerializeField]private AudioSource jumpSFX;
    [SerializeField]private AudioSource stepSFX;
    [SerializeField]private AudioSource turnSFX;

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
                isHit = true;
                Flinch(isHit);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "coin") 
        {
            coinSFX.Play();
			other.gameObject.SetActive(false);
			AddScore();
        }
    }

    public void AddScore()
    {
        Score?.Invoke();
    }

    public void Flinch(bool value)
    {
        Hit?.Invoke(value);
        impactSFX.Play();
        isRunning = false;
        isHit = false;
    }

    public void StartRun()
    {
        isRunning = true;
        anim.SetTrigger("run");
    }

    //Di panggil lewat Animatior
    private void StartRollBack()
    {
        isRollback = true;
        StartCoroutine(StopRollBack());
    }

    private IEnumerator StopRollBack()
    {
        yield return new WaitForSeconds(rollbackDuration);
        isRollback = false;
        isStartCountdown = true;
        anim.SetTrigger("idle");
        Countdown?.Invoke(isStartCountdown);
    } 
    

    public void PlayStepSFX()
    {
        stepSFX.Play();
    }
}
