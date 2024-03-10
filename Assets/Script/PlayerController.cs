using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    int y = 0;

    private Vector2 Running_Speed;
    private Animator animator;
    private Rigidbody2D playerRigidBody;
    private SpriteRenderer spriteRenderer;
    private Camera cam;

    private float angle;
    private float horizontal;

    private Vector2 MousePos;
    private Vector2 AtkPos;

    public float MoveSpeed = 200f;
    public float MaxSpeed = 200f;
    public float JumpForce = 1000f;

    public float AtkPow = 3000f;

    private bool isGrounded;
    private bool isJumped;
    private bool isWallJump = false;

    private int AtkCnt = 0;

    public Transform Collpos;
    public Vector2 BoxSize;

    private bool canRoll = true;
    private bool isRoll;
    private float rollPower = 1200f;
    private float rollTime = 0.7f;
    private float rollCooldown = 0.3f;

    public Transform wallChk;
    public float wallchkDistance;

    BoxCollider2D Collider;

    private float ColliderY;

    private float wallCastSize = 0.1f;
    private float wallCastGap = 0.11f;

    public float wallJumpForce = 1000f;

    private bool isAttacking = false;



    Animator melee;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        melee = GameObject.Find("Melee").GetComponent<Animator>();
        Collider = GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.FindWithTag("Enemy").GetComponent<Collider2D>());

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
        Debug.Log(isWallJump);

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
        attackAct();
        RollAct();
        WallJump();
        

        
    }

    int isRightPos()
    {
        if (spriteRenderer.flipX == false)
        {
            return 1;
        }
        else return -1;
    }

    void Run()
    {
        if (!isWallJump)
        {
            if (Input.GetAxis("Horizontal") != 0)
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

                playerRigidBody.velocity = Running_Speed;
            }
        }
        //이동속도 입력
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

        Vector2 boxCastFloor = new Vector2(transform.position.x, Collider.bounds.min.y - 1f);

        RaycastHit2D boxcastHit = Physics2D.BoxCast(boxCastFloor, new Vector2(95f, 1f), 0f, Vector2.down, 1f);


        if (boxcastHit.collider != null && boxcastHit.collider.tag == "Floor")
        {
            Debug.Log("박스캐스트 충돌함");
            isGrounded = true;
            //AtkCnt = 0;
            animator.SetBool("Jumped", isJumped);
            animator.SetBool("Grounded", isGrounded);
        }
        else if (boxcastHit.collider == null)
        {
            isGrounded = false;

            animator.SetBool("Jumped", isJumped);
            animator.SetBool("Grounded", isGrounded);
        }

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

    void WallJump()
    {
        if (isRightPos() == 1)
        {
            Vector2 boxCastWall = new Vector2(Collider.bounds.max.x + wallCastGap, transform.position.y + Collider.size.y / 2);

            RaycastHit2D isWall = Physics2D.BoxCast(boxCastWall, new Vector2(wallCastSize, 160f), 0f, Vector2.right, 1f);

            if (isWall.collider != null && isWall.collider.tag == "Wall")
            {
                isWallJump = false;
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, playerRigidBody.velocity.y * 0.8f);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isWallJump = true;
                    Invoke("FreezeX", 0.3f);
                    playerRigidBody.velocity = new Vector2(-isRightPos() * wallJumpForce, wallJumpForce);
                }
            }
            animator.SetBool("Wall", isWall);
        }
        else
        {
            Vector2 boxCastWall = new Vector2(Collider.bounds.min.x - wallCastGap, transform.position.y + Collider.size.y / 2);

            RaycastHit2D isWall = Physics2D.BoxCast(boxCastWall, new Vector2(wallCastSize, 160f), 0f, Vector2.left, 1f);

            if (isWall.collider != null && isWall.collider.tag == "Wall")
            {
                isWallJump = false;
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, playerRigidBody.velocity.y * 0.8f);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isWallJump = true;
                    Invoke("FreezeX", 0.3f);
                    playerRigidBody.velocity = new Vector2(-isRightPos() * wallJumpForce, wallJumpForce);
                }
            }
            animator.SetBool("Wall", isWall);
        }
    }
    void FreezeX()
    {
        isWallJump = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
    private void OnCollisionStay2D(Collision2D collision)
    {

    }
    private void OnCollisionExit2D(Collision2D collision)
    {

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
        isGrounded = true;
    }

    private void OnDrawGizmos()
    {
        Collider = GetComponent<BoxCollider2D>();

        Vector2 boxCastFloor = new Vector2(transform.position.x, Collider.bounds.min.y - 1f);

        Vector2 boxCastWallRight = new Vector2(Collider.bounds.max.x + wallCastGap, transform.position.y + Collider.size.y / 2);
        Vector2 boxCastWallLeft = new Vector2(Collider.bounds.min.x - wallCastGap, transform.position.y + Collider.size.y / 2);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Collpos.position, BoxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCastFloor, new Vector3(95f, 1f, 0));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boxCastWallRight, new Vector2(wallCastSize, 160f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boxCastWallLeft, new Vector2(wallCastSize, 160f));
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
        if (Input.GetKey(KeyCode.LeftShift) && canRoll && !isAttacking)
        {
            StartCoroutine(Roll());
        }
    }

    void attackAct()
    {
        if (Input.GetMouseButtonDown(0) && AtkCnt == 0)
        {
            StartCoroutine(Attack1Coroutine());
        }
    }

    IEnumerator Attack1Coroutine()
    {
        if (isAttacking)  // 이미 공격 중이면 중복 호출 방지
            yield break;

        isAttacking = true;
        animator.SetBool("Attack0", true);
        Debug.Log("공격작동함 " + y++);

        MousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

        AtkPos = MousePos - new Vector2(playerRigidBody.position.x, playerRigidBody.position.y);

        Vector2 AtkSpd = new Vector2(AtkPos.normalized.x * AtkPow, AtkPos.normalized.y * AtkPow);

        if (isGrounded)
        {
            playerRigidBody.velocity += AtkSpd;
        }
        else
        {
            playerRigidBody.velocity = Vector2.zero;
            playerRigidBody.velocity = AtkSpd;
        }

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
        if (transform.position.x > MousePos.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
        AtkCnt++;
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("Attack0", false);
        //Invoke(nameof(RotZero), 0.34f);
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        yield return new WaitForSeconds(1f);
        AtkCnt = 0;
        // This will make the coroutine wait for one frame
    }

}
