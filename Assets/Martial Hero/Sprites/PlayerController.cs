using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Vector2 Running_Speed;
    private Animator animator;
    private Rigidbody2D playerRigidBody;
    private SpriteRenderer spriteRenderer;
    private Camera cam;

    private float angle;
    private float horizontal;

    private Vector2 MousePos;
    private Vector2 AtkPos;

    public float MoveSpeed = 200.0f;
    public float MaxSpeed = 200.0f;
    public float JumpForce = 1000.0f;

    public float AtkPow = 3000.0f;

    private bool isGrounded;
    private bool isJumped;

    private int AtkCnt = 0;

    public Transform Collpos;
    public Vector2 BoxSize;

    private bool canRoll = true;
    private bool isRoll;
    private float rollPower = 1200f;
    private float rollTime = 0.3f;
    private float rollCooldown = 1f;

    Animator melee;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        melee = GameObject.Find("Melee").GetComponent<Animator>();

        isGrounded = false;
        isJumped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRoll)
        {
            return;
        }
        Debug.Log(AtkCnt);
    }
    void FixedUpdate()
    {
        if (isRoll)
        {
            return;
        }

        Run();
        RunAni();
        Jump();
        Attack1();
        RollAct();

        Debug.Log("점프" + isJumped);
        Debug.Log("그라운드" + isGrounded);
    }

    void Run()
    {
        horizontal = Input.GetAxis("Horizontal");

        if (horizontal * MoveSpeed < MaxSpeed && horizontal * MoveSpeed > -MaxSpeed)    //속도 제한
        {
            Running_Speed = new Vector2(horizontal * MoveSpeed, playerRigidBody.velocity.y);
        }
        else
        {
            Running_Speed = new Vector2(horizontal * MaxSpeed, playerRigidBody.velocity.y);
        }

        playerRigidBody.velocity = Running_Speed;   //이동속도 입력
    }

    void RunAni()   //달리기 애니메이션
    {
        if (Input.GetButtonUp("Horizontal"))
        {
            animator.SetBool("Run", false);
        }
        else if (Input.GetButton("Horizontal"))
        {
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }

        if (Input.GetAxis("Horizontal") == 0.0f)
        {
            spriteRenderer.flipX = spriteRenderer.flipX;
        }
        else if (Input.GetAxis("Horizontal") < 0.0f)
        {
            spriteRenderer.flipX = true;
        }
        else if (Input.GetAxis("Horizontal") > 0.0f)
        {
            spriteRenderer.flipX = false;
        }
    }

    void Jump()
    {
        //Debug.Log(Input.GetAxis("Vertical"));
        if (Input.GetKey(KeyCode.Space) && isGrounded == true)
        {
            Debug.Log("점프 조건 통과");
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, JumpForce);
            //playerRigidBody.AddForce(new Vector2(0, JumpForce));
            isJumped = true;
            Debug.Log(isJumped + "점프");
            Debug.Log(isGrounded + "그라운드");
            animator.SetBool("Jumped", isJumped);
            animator.SetBool("Grounded", isGrounded);
            isJumped = false;
        }
        if (Input.GetButtonUp("Vertical"))
        {
            //playerRigidBody.AddForce(new Vector2(0, -40000));
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, playerRigidBody.velocity.y * 0.6f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.9f)
        {
            Debug.Log("worked");
            isGrounded = true;
            Debug.Log(isJumped);
            animator.SetBool("Jumped", isJumped);
            animator.SetBool("Grounded", isGrounded);

        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.9f)
        {
            AtkCnt = 0;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
        animator.SetBool("Grounded", isGrounded);
        Debug.Log("not collide");
    }

    void anim_fall()
    {
        isJumped = false;
        animator.Play("Fall0");
    }

    public void RotZero()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Debug.Log("회전초기화됨");
    }

    void Attack1()
    {

        if (Input.GetMouseButton(0) && AtkCnt == 0)
        {


            Debug.Log("공격작동함");
            MousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x,
                Input.mousePosition.y));

            AtkPos = MousePos - new Vector2(playerRigidBody.position.x, playerRigidBody.position.y);

            Vector2 AtkSpd = new Vector2(AtkPos.normalized.x * AtkPow, AtkPos.normalized.y * AtkPow * 0.25f);

            playerRigidBody.velocity = Vector2.zero;
            playerRigidBody.velocity = AtkSpd;
            //playerRigidBody.AddForce(AtkSpd);

            //transform.rotation = Quaternion.Euler(0.0f, 0.0f, AtkPos.normalized.y / AtkPos.normalized.x);

            animator.SetTrigger("Attack");

            melee.SetTrigger("AtkEf0");

            angle = Mathf.Atan2(MousePos.y, MousePos.x) * Mathf.Rad2Deg;

            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(Collpos.position, BoxSize, angle);
            foreach (Collider2D collider in collider2Ds)
            {
                Debug.Log(collider);
            }

            Invoke(nameof(RotZero), 0.34f);

            //스프라이트 뒤집음

            if (transform.position.x > MousePos.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }

            AtkCnt++;

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Collpos.position, BoxSize);
    }

    private IEnumerator Roll()
    {
        if (isGrounded)
        {
            canRoll = false;
            isRoll = true;
            animator.SetTrigger("Roll");
            playerRigidBody.velocity = new Vector2(transform.localScale.x * rollPower, 0f);
            yield return new WaitForSeconds(rollTime);
            isRoll = false;
            yield return new WaitForSeconds(rollCooldown);
            canRoll = true;
        }
    }

    void RollAct()
    {
        if (Input.GetKey(KeyCode.LeftShift) && canRoll)
        {
            StartCoroutine(Roll());
        }
    }
}
