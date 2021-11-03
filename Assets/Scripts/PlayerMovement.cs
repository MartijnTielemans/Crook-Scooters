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

    [Space]
    [Header("Player Stats")]
    [SerializeField] float moveSpeed = 7;
    [SerializeField] float inputSmoothing = 1.5f;
    [SerializeField] float jumpForce = 900;
    [SerializeField] float bounceForce = 250;
    [SerializeField] float gravity = 23;
    [SerializeField] float stunTime = 1.2f;
    public bool active = true;
    public bool canMove;
    [SerializeField] bool stunned;

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
    BoxCollider boxCollider;
    RaycastHit groundCheckHit;
    RaycastHit wallHit;

    [Space]
    [Header("Animations")]
    [SerializeField] GameObject model;
    [SerializeField] string deathAnimationName;
    [SerializeField] float tiltAmount = 10;
    [SerializeField] float tiltSpeed = 5;
    Vector3 currentTilt;
    Animator anim;

    [SerializeField] ParticleSystem hitParticle;
    [SerializeField] ParticleSystem stunParticle;

    Vector2 moveInput;
    Vector2 currentInputVector;
    Vector2 inputVelocity;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        InitialisePlayer();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        onGround = true;
    }

    private void Update()
    {
        if (active)
        {
            // Set onGround with a boxcast
            if (!onGround && canCheckOnGround)
            {
                onGround = CheckGround();
            }

            // Check for walls
            leftWall = CheckWall(true);
            rightWall = CheckWall(false);

            // For setting tilt animations
            if (onGround && !stunned)
            {
                // Change jump lilt animation back to 0
                currentTilt.x = 0;

                // For tilting player animation
                if (moveInput.x < 0)
                {
                    currentTilt.z = -tiltAmount;
                }
                else if (moveInput.x > 0)
                {
                    currentTilt.z = tiltAmount;
                }
                else
                {
                    currentTilt.z = 0;
                }
            }
            else if  (onGround && stunned)
            {
                currentTilt.x = tiltAmount * 2;
            }

            // Applies current tilt to model
            Quaternion tilt = Quaternion.Euler(currentTilt);
            model.transform.localRotation = Quaternion.Lerp(model.transform.localRotation, tilt, tiltSpeed * Time.deltaTime);

            // Check if a player is hit with the groundcheck and add force
            if (!onGround)
            {
                if (CheckPlayerJump())
                {
                    rb.velocity = Vector3.zero;
                    rb.AddForce(Vector3.up * bounceForce);

                    // Cause stun on other player
                    groundCheckHit.collider.gameObject.GetComponent<PlayerMovement>().GetStunned(stunTime);
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Call MovePlayer
        if (canMove && active)
            MovePlayer();

        // Apply gravity
        if (!onGround)
        {
            rb.AddForce(Vector3.down * gravity);
        }
    }

    // Checks for walls to the left and right
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

    // Check for ground collision
    bool CheckGround()
    {
        bool hit = Physics.BoxCast(groundCheck.bounds.center, groundCheck.bounds.size, Vector3.down, out groundCheckHit, groundCheck.transform.rotation, 1, groundMask);
        return hit;
    }

    // Check if there is another player below the players
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
        manager.players.Add(gameObject.GetComponent<PlayerMovement>());

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
        playerColor = color;
        hatRenderer.material.color = playerColor;
        scooterRenderer.material.color = playerColor;
    }

    // Called when a player jumps on this player
    // Calls a coroutine which handles the actual stun and timer
    public void GetStunned(float stunTime)
    {
        StartCoroutine(StunTimer(stunTime));
    }

    // Sets canmove before and after timer
    IEnumerator StunTimer(float stunTime)
    {
        canMove = false;
        stunParticle.Play();

        yield return new WaitForSeconds(stunTime);

        canMove = true;
        stunParticle.Stop();
    }

    void PlayerDeath()
    {
        Debug.Log("Player Died");

        // Play death animation and particle
        anim.Play(deathAnimationName);
        hitParticle.Play();

        // Set Player active bool
        active = false;

        // Stop player from inputting
        canMove = false;

        // Call method to turn off collider and set offset
        StartCoroutine(SetPlayerDeathLocation(.5f));

        // Check if only 1 player is still alive (Depending on single or multiplayer mode)
        if (manager.singleplayerMode)
        {
            // Start endgame sequence
            manager.GameEnd(false);
        }
        else
        {
            int actives = 0;

            // Got through every player, check if they are still active
            for (int i = 0; i < manager.players.Count; i++)
            {
                if (manager.players[i].active)
                {
                    actives++;
                }
            }

            // If there is only 1 active player
            if (actives == 1)
            {
                // Start game end sequence
                manager.GameEnd(false);
            }
        }
    }

    // Sets the players hitbox and gravity to false on death
    IEnumerator SetPlayerDeathLocation(float timer)
    {
        yield return new WaitForSeconds(timer);

        transform.Translate(0, 0, -40);
        boxCollider.enabled = false;
        rb.useGravity = false;

    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump()
    {
        if (onGround && active)
        {
            canCheckOnGround = false;
            onGround = false;
            rb.AddForce(Vector3.up * jumpForce);

            // For jump tilt animation
            currentTilt.x = tiltAmount * 2;

            StartCoroutine(SetCanCheckOnGround());
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Obstacle hit");

            if (!manager.godMode)
            {
                PlayerDeath();
            }
        }
    }
}
