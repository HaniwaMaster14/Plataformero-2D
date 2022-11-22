using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private float WallJumpCooldown;
    private float HorizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
 
    private void Update()
    {
        HorizontalInput = Input.GetAxis("Horizontal"); 

        //Voltea el sprite
        if (HorizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (HorizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        //setteo de parametros del animator
        anim.SetBool("Running", HorizontalInput != 0);
        anim.SetBool("Grounded", isGrounded());

        //logica de wall jump
        if (WallJumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(HorizontalInput * speed, body.velocity.y);

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
                body.gravityScale = 7;
        }
        else
            WallJumpCooldown += Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
            Jump();
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("Jump");
        }
        else if (onWall() && !isGrounded())
        {
            if (HorizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);

            WallJumpCooldown = 0;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
    
    public bool canAttack()
    {
        return HorizontalInput == 0 && isGrounded() && !onWall();
    }
}