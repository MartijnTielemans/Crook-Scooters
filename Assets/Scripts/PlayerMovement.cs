using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    GameManager manager;
    Rigidbody rb;

    [Header("Customization")]
    [SerializeField] Color[] possibleColors;
    public Color playerColor;
    [SerializeField] MeshRenderer hatRenderer;
    [SerializeField] MeshRenderer scooterRenderer;
    public bool canMove;

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
    bool canCheckOnGround;
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
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        InitialisePlayer();
        rb = GetComponent<Rigidbody>();
        onGround = true;
    }

    private void Update()
    {
        // Set onGround with a boxcast
        if (!onGround && canCheckOnGround)
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
        if (canMove)
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

    IEnumerator SetCanCheckOnGround()
    {
        yield return new WaitForSeconds(0.4f);
        canCheckOnGround = true;
    }

    // Sets initial values like location and color
    void InitialisePlayer()
    {
        int playersJoined = PlayerInputManager.instance.playerCount-1;
        manager.players.Add(gameObject);

        switch (playersJoined)
        {
            case 0:
                SetLocation(manager.spawnLocations[0]);
                ChangeColor(possibleColors[0]);
                break;

            case 1:
                SetLocation(manager.spawnLocations[1]);
                ChangeColor(possibleColors[1]);
                break;

            case 2:
                SetLocation(manager.spawnLocations[2]);
                ChangeColor(possibleColors[2]);
                break;

            case 3:
                SetLocation(manager.spawnLocations[3]);
                ChangeColor(possibleColors[3]);
                break;

            default:
                break;
        }
    }

    void SetLocation(Vector3 position)
    {
        transform.position = position;
    }

    void ChangeColor(Color color)
    {
        hatRenderer.material.color = color;
        scooterRenderer.material.color = color;
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump()
    {
        if (onGround)
        {
            canCheckOnGround = false;
            onGround = false;
            rb.AddForce(Vector3.up * jumpForce);

            StartCoroutine(SetCanCheckOnGround());
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // Do stuff
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Obstacle hit");
        }
    }
}
