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
    private float rollTime = 0.7f;
    private float rollCooldown = 0.3f;

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

        
        
    }

    void Run()
    {
        horizontal = Input.GetAxis("Horizontal");

        if (horizontal * MoveSpeed < MaxSpeed && horizontal * MoveSpeed > -MaxSpeed)    //�ӵ� ����
        {
            Running_Speed = new Vector2(horizontal * MoveSpeed, playerRigidBody.velocity.y);
        }
        else
        {
            Running_Speed = new Vector2(horizontal * MaxSpeed, playerRigidBody.velocity.y);
        }

        playerRigidBody.velocity = Running_Speed;   //�̵��ӵ� �Է�
    }

    void RunAni()   //�޸��� �ִϸ��̼�
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
            
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, JumpForce);
            //playerRigidBody.AddForce(new Vector2(0, JumpForce));
            isJumped = true;
            
            
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
            
            isGrounded = true;
            
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
        
    }

    void anim_fall()
    {
        isJumped = false;
        animator.Play("Fall0");
    }

    public void RotZero()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Debug.Log("ȸ���ʱ�ȭ��");
    }

    void Attack1()
    {

        if (Input.GetMouseButton(0) && AtkCnt == 0)
        {


            Debug.Log("�����۵���");
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
                if (collider.tag == "Enemy")
                {
                    collider.GetComponent<EnemyController>().TakeDamage();
                }
            }

            Invoke(nameof(RotZero), 0.34f);

            //��������Ʈ ������

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
            animator.SetBool("Roll", true);

            if (spriteRenderer.flipX)
            {
                playerRigidBody.velocity = new Vector2(-transform.localScale.x * rollPower, 0f);
            }
            else
            {
                playerRigidBody.velocity = new Vector2(transform.localScale.x * rollPower, 0f);
            }

            yield return new WaitForSeconds(rollTime);
            isRoll = false;
            animator.SetBool("Roll", false);
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
