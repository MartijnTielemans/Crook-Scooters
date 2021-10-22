using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;

    [Header("Customization")]
    public Color playerColor;

    [Space]

    [Header("Player Stats")]
    [SerializeField] float moveSpeed = 7;
    [SerializeField] float inputSmoothing = 1.5f;
    [SerializeField] float jumpForce = 900;
    [SerializeField] float bounceForce = 250;
    [SerializeField] float gravity = 23;

    [Space]

    [Header("Collisions")]
    [SerializeField] BoxCollider groundCheck;
    [SerializeField] BoxCollider wallCheckLeft;
    [SerializeField] BoxCollider wallCheckRight;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask wallMask;
    [SerializeField] bool onGround;
    [SerializeField] bool leftWall;
    bool rightWall;
    RaycastHit groundCheckHit;
    RaycastHit wallHit;

    Vector2 moveInput;
    Vector2 currentInputVector;
    Vector2 inputVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        onGround = true;
    }

    private void Update()
    {
        // Set onGround with a boxcast
        if (!onGround)
            onGround = CheckGround();

        // Check for walls
        leftWall = CheckWall(true);
        rightWall = CheckWall(false);

        // Check if a player is hit with the groundcheck and add force
        if (!onGround)
        {
            if (CheckPlayerJump())
            {
                rb.AddForce(Vector3.up * bounceForce);
            }
        }
    }

    void FixedUpdate()
    {
        // Call MovePlayer
        MovePlayer();

        // Apply gravity
        if (!onGround)
        {
            rb.AddForce(Vector3.down * gravity);
        }
    }

    bool CheckWall(bool isLeft)
    {
        if (isLeft)
        {
            bool hit = Physics.BoxCast(wallCheckLeft.bounds.center, wallCheckLeft.bounds.size, Vector3.left, out wallHit, wallCheckLeft.transform.rotation, 0.8f, wallMask);
            return hit;
        }
        else
        {
            bool hit = Physics.BoxCast(wallCheckRight.bounds.center, wallCheckRight.bounds.size, Vector3.right, out wallHit, wallCheckRight.transform.rotation, 0.8f, wallMask);
            return hit;
        }
    }

    bool CheckGround()
    {
        bool hit = Physics.BoxCast(groundCheck.bounds.center, groundCheck.bounds.size, Vector3.down, out groundCheckHit, groundCheck.transform.rotation, 1, groundMask);
        return hit;
    }

    bool CheckPlayerJump()
    {
        bool hit = Physics.BoxCast(groundCheck.bounds.center, groundCheck.bounds.size, Vector3.down, out groundCheckHit, groundCheck.transform.rotation, 1, playerMask);
        return hit;
    }

    void MovePlayer()
    {
        // Can't move if touching a wall
        if (!leftWall && moveInput.x < 0 || !rightWall && moveInput.x > 0 || !leftWall && !rightWall && moveInput.x == 0)
        {
            // Only update currentInputVector when on the ground
            if (onGround)
            {
                currentInputVector = Vector2.SmoothDamp(currentInputVector, moveInput, ref inputVelocity, inputSmoothing);
            }

            Vector3 movement = new Vector3(currentInputVector.x, 0, 0);

            // Neutralize movement in wall direction if touching a wall
            if (leftWall && movement.x < 0 || rightWall && movement.x > 0)
            {
                movement = new Vector3(0, 0, 0);
            }

            transform.Translate(movement * moveSpeed * Time.deltaTime);
        }
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump()
    {
        if (onGround)
        {
            onGround = false;
            rb.AddForce(Vector3.up * jumpForce);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // Do stuff
        }
    }
}
