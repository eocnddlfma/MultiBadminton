using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInput : NetworkBehaviour
{
    private AudioSource audioSource;
    public ColliderControll cc;
    public Vector2 moveVec2;
    public enum SwingStyle
    {
        None,
        Up,
        Right,
        Down,
        Hairpin
    };

    public SwingStyle swingStyle;
    
    public float jumpAmount;
    public float moveSpeed;
    public State state;
    public bool isJumpable;
    public Rigidbody2D Rigidbody2D;
    public Animator animator;
    public bool isLeft;

    public float movePowerHorizontal;
    public float movePowerVertical;
    
    private void Start()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            isLeft = true;
            if (IsOwner)
            {
                transform.position = new Vector3(-7, 1);
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.position = new Vector3(7,1);
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else
        {
            isLeft = false;
            if (IsOwner)
            {
                transform.position = new Vector3(7,1);
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.position = new Vector3(-7, 1);
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        // print(isLeft);
        if (!IsOwner) enabled = false;
        
        audioSource = GetComponent<AudioSource>();
        cc = GetComponentInChildren<ColliderControll>();
        cc.pi = this;
        animator = GetComponent<Animator>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        isJumpable = true;
    }

    void FixedUpdate()
    {
        CheckInput();
        if (IsOwner)
        {
            transform.Translate(moveVec2 * moveSpeed * Time.fixedDeltaTime );
        }
    }
    private void Update()
    { 
        CheckInput();
    }

    public void CheckInput()
    {
        OnMove();
        if (Input.GetKey(KeyCode.UpArrow))
        {
            OnJump();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            DownSwing();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            RightSwing();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            UpSwing();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            OnHairpin();
        }
    }
    public void OnMove()
    {
        float x, y;
        x = Input.GetAxisRaw("Horizontal");
        if (!isJumpable)
        {
            y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            y = 0;
        }
        moveVec2 = new Vector2(x, y);
        animator.SetFloat("MoveSpeed", moveVec2.magnitude);
        movePowerHorizontal = x;
        movePowerVertical = y;
    }
    public void UpSwing()
    {
        animator.SetTrigger("Swing");
        swingStyle = SwingStyle.Up;
    }
    public void RightSwing()
    {
        animator.SetTrigger("Swing");
        swingStyle = SwingStyle.Right;
    }
    public void DownSwing()
    {
        animator.SetTrigger("Swing");
        swingStyle = SwingStyle.Down;
    }
    public void OnHairpin()
    {
        animator.SetTrigger("Hairpin");
        swingStyle = SwingStyle.Hairpin;
    }
    public void OnJump()
    {
        if (isJumpable)
        {
            StartCoroutine(Jump());
            isJumpable = false;
        }
    }
    IEnumerator Jump()
    {
        Rigidbody2D.AddForce(new Vector2(0,jumpAmount), ForceMode2D.Impulse);
        yield return new WaitForSeconds(1f);
        isJumpable = true;
    }
    
    public void Turnoff(){cc.TurnOff();}
    public void Turnon(){cc.TurnOn();}
    public void SwingSound(){audioSource.Play();}

    public void AnimationHitSlow()
    {
        StartCoroutine(AnimationSpeedDown());
    }

    IEnumerator AnimationSpeedDown()
    {
        animator.SetFloat("AnimationSpeed", 0);
        yield return new WaitForSeconds(0.1f);
        animator.SetFloat("AnimationSpeed", 1f);
    }
}
