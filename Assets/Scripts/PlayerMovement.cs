using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float runSpeed = 5.0f;

    [SerializeField]
    float jumpSpeed = 8.0f;

    [SerializeField]
    float climbingSpeed = 2.0f;

    [SerializeField]
    GameObject bullet;

    [SerializeField]
    Transform gun;

    [SerializeField]
    float bulletSpeed = 10.0f;

    [SerializeField]
    float bulletSpinSpeed = 10.0f;

    // [SerializeField]
    // int numberOfJump = 2;
    Vector2 moveInput;

    Rigidbody2D myRigidbody;

    CapsuleCollider2D myCapsuleBody;

    CapsuleCollider2D myCapsuleFeet;

    Animator myAnimator;

    float gravityScaleAtStart;

    bool isPlayerClimbing = false;

    bool isPlayerJumping = false;

    bool isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCapsuleBody = GetComponents<CapsuleCollider2D>()[0];
        myCapsuleFeet = GetComponents<CapsuleCollider2D>()[1];
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) return;

        Run();
        FlipSprite();
        ClimbLadder();
        // Die();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) return;
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) return;
        if (
            (
            myCapsuleFeet.IsTouchingLayers(LayerMask.GetMask("Ground")) ||
            myCapsuleFeet.IsTouchingLayers(LayerMask.GetMask("Ladder")) ||
            myCapsuleFeet.IsTouchingLayers(LayerMask.GetMask("Bullets"))
            ) &&
            value.isPressed
        )
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myRigidbody.velocity += new Vector2(0.0f, jumpSpeed);
        }
    }

    void OnFire(InputValue value)
    {
        if (!isAlive) return;
        if (value.isPressed)
        {
            GameObject bulletInstance =
                Instantiate(bullet, gun.position, transform.rotation);

            // bulletInstance.transform.localScale = transform.localScale;
            bulletInstance
                .GetComponent<Bullet>()
                .setBulletSpeed(Mathf.Sign(transform.localScale.x) *
                bulletSpeed);
            bulletInstance
                .GetComponent<Bullet>()
                .setSpinSpeed(Mathf.Sign(transform.localScale.x) *
                bulletSpinSpeed);
        }
    }

    void Run()
    {
        Vector2 playerVelocity =
            new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool isPlayerMoveHorizontal =
            Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        bool isPlayerMoveVertical = Mathf.Abs(myRigidbody.velocity.y) > 0.1;

        myAnimator.SetBool("isRunning", isPlayerMoveHorizontal);
        myAnimator
            .SetBool("isJumping", isPlayerMoveVertical && !isPlayerClimbing);
        isPlayerJumping = isPlayerMoveVertical && !isPlayerClimbing;
    }

    void ClimbLadder()
    {
        if (!myCapsuleFeet.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            myAnimator.SetBool("isClimbing", false);
            myRigidbody.gravityScale = gravityScaleAtStart;
            isPlayerClimbing = false;
            return;
        }

        bool onTheLadderAndMoving = false;
        bool onTheLadder = true;

        // isPlayerClimbing = false;
        if (Mathf.Abs(moveInput.y) > Mathf.Epsilon)
        {
            onTheLadderAndMoving = true;
        }

        Vector2 playerClimbingVelocity =
            new Vector2(myRigidbody.velocity.x, moveInput.y * climbingSpeed);

        if (onTheLadderAndMoving)
        {
            myRigidbody.velocity = playerClimbingVelocity;
            myRigidbody.gravityScale = 0;
            isPlayerClimbing = true;
            myAnimator.SetBool("isClimbing", true);
        }
        else if (onTheLadder && !isPlayerJumping)
        {
            myRigidbody.velocity = playerClimbingVelocity;
            isPlayerClimbing = false;
            myAnimator.SetBool("isClimbing", false);
        }
        else
        {
            onTheLadder = false;
            isPlayerClimbing = false;
            myAnimator.SetBool("isClimbing", false);
        }
    }

    void FlipSprite()
    {
        bool isPlayerMoveHorizontal =
            Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (isPlayerMoveHorizontal)
        {
            transform.localScale =
                new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1.0f);
        }
    }

    private void Die()
    {
        // if (myCapsuleBody.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        // {
        isAlive = false;
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpSpeed);
        myAnimator.SetTrigger("Dying");
        // }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isAlive) return;
        if (
            other.gameObject.name == "Hazards Tilemap" ||
            myCapsuleBody.IsTouchingLayers(LayerMask.GetMask("Enemy"))
        )
        {
            Die();
        }
    }
}
