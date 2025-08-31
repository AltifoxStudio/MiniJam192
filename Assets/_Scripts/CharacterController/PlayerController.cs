using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public enum MoveDirection
{
    Up,
    Down,
    Side,
    Idle,
}

public enum LookDirection
{
    Left,
    Right,
}

public class PlayerController : MonoBehaviour
{
    // Actions
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;

    [Header("Configuration")]
    public GameConfig gameConfig;
    public GameObject lookAtTarget;
    public GameObject playerQuad;
    public GameObject LookAtContainer;
    public bool infiniteJump;
    public LayerMask groundLayer;

    // Gameplay variables
    private float moveSpeed;
    private float jumpForce; // We now use jumpForce instead of jumpSpeed
    private float dashSpeed;
    private bool isGrounded;
    private float dashCooldown;

    // Animation 
    [Header("Animation Textures")]
    public Texture2D[] MoveLeftTextures;
    public Texture2D[] MoveLeftFXTextures;
    public Texture2D[] MoveRightTextures;
    public Texture2D[] MoveRightFXTextures;
    public Texture2D[] MoveUpRightTextures;
    public Texture2D[] MoveUpRightFXTextures;
    public Texture2D[] MoveUpLeftTextures;
    public Texture2D[] MoveUpLeftFXTextures;
    public Texture2D[] MoveDownLeftTextures;
    public Texture2D[] MoveDownLeftFXTextures;
    public Texture2D[] MoveDownRightTextures;
    public Texture2D[] MoveDownRightFXTextures;

    // Ground Check variables
    public Transform groundCheckPoint;
    public float groundCheckDistance = 0.3f;
    public float groundCheckRadius = 0.2f;

    // Components and private variables
    private Rigidbody rb;
    private Renderer rend;
    private float lastDashTime;
    private MaterialPropertyBlock bunnyTexture;
    private float dustSpendMoving;
    private Coroutine dashCoroutine;

    private LookDirection lookDirection;

    private void Awake()
    {
        // Get Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Load config values
        moveSpeed = gameConfig.bunnyMS;
        jumpForce = gameConfig.bunnyJumpSpeed; // Re-purposing this value for jump force
        dashSpeed = gameConfig.bunnyDashSpeed;
        dashCooldown = gameConfig.dashCooldown;
        lastDashTime = -dashCooldown;
        dustSpendMoving = gameConfig.dustSpendMoving;
    }

    private void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogError("Renderer component not found on this GameObject.");
            return;
        }
        bunnyTexture = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        // Find actions if they are not already found
        if (jumpAction == null)
        {
            moveAction = InputSystem.actions.FindAction("Move");
            jumpAction = InputSystem.actions.FindAction("Jump");
            dashAction = InputSystem.actions.FindAction("Dash");
        }

        // Subscribe the Jump method directly to the 'performed' event
        jumpAction.performed += Jump;
    }

    private void OnDisable()
    {
        // Unsubscribe the Jump method directly to prevent memory leaks
        jumpAction.performed -= Jump;
    }

    private void FixedUpdate()
    {
        // We always check for the ground
        isGrounded = CheckGround();
        HandleMovement();
        HandleDash();
    }

    private void Update() {
        HandleRotationAndAnimation();
    }

    private void HandleMovement()
    {
        // Read input value
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        // Calculate horizontal movement, preserving the Rigidbody's current vertical velocity
        Vector3 horizontalMove = new Vector3(moveValue.x, 0, moveValue.y) * moveSpeed;
        rb.linearVelocity = new Vector3(horizontalMove.x, rb.linearVelocity.y, horizontalMove.z);

        // Handle dust emission
        GetComponent<HasDust>().GiveDust(Vector3.Magnitude(horizontalMove) * dustSpendMoving);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        // Only allow jumping if the player is on the ground
        if (isGrounded)
        {
            // Apply an instant upward force to the Rigidbody
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            SFXManager.Instance.jumpSFX.PreloadAndPlay();
        }
    }

    private void HandleDash()
    {
        if (Time.time - lastDashTime > dashCooldown)
        {
            if (dashAction.IsPressed())
            {
                lastDashTime = Time.time;
                if (dashCoroutine != null)
                {
                    StopCoroutine(dashCoroutine);
                }
                dashCoroutine = StartCoroutine(CR_Dash(0.05f));
            }
        }

    }

    private void HandleRotationAndAnimation()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        MoveDirection moveDirection = evalMoveDirection(moveValue);


        // Flip the player sprite
        if (moveValue.x < -0.1f)
        {
            lookDirection = LookDirection.Left;
        }
        else if (moveValue.x > 0.1f)
        {
            lookDirection = LookDirection.Right;
        }

        // Set texture based on movement direction
        switch (moveDirection)
        {
            case MoveDirection.Up:
                if (lookDirection == LookDirection.Left)
                {
                    bunnyTexture.SetTexture("_playerSprite", MoveUpLeftTextures[0]);
                    bunnyTexture.SetTexture("_playerFX0", MoveUpLeftFXTextures[0]);
                    bunnyTexture.SetTexture("_playerFX1", MoveUpLeftFXTextures[1]);
                }
                else
                {
                    bunnyTexture.SetTexture("_playerSprite", MoveUpRightTextures[0]);
                    bunnyTexture.SetTexture("_playerFX0", MoveUpRightFXTextures[0]);
                    bunnyTexture.SetTexture("_playerFX1", MoveUpRightFXTextures[1]);
                }
                break;

           case MoveDirection.Down:
                if (lookDirection == LookDirection.Left)
                {
                    bunnyTexture.SetTexture("_playerSprite", MoveDownLeftTextures[0]);
                    bunnyTexture.SetTexture("_playerFX0", MoveDownLeftFXTextures[0]);
                    bunnyTexture.SetTexture("_playerFX1", MoveDownLeftFXTextures[1]);
                }
                else
                {
                    bunnyTexture.SetTexture("_playerSprite", MoveDownRightTextures[0]);
                    bunnyTexture.SetTexture("_playerFX0", MoveDownRightFXTextures[0]);
                    bunnyTexture.SetTexture("_playerFX1", MoveDownRightFXTextures[1]);
                }
                break;

           case MoveDirection.Side:
                if (lookDirection == LookDirection.Left)
                {
                    bunnyTexture.SetTexture("_playerSprite", MoveLeftTextures[0]);
                    bunnyTexture.SetTexture("_playerFX0", MoveLeftFXTextures[0]);
                    bunnyTexture.SetTexture("_playerFX1", MoveLeftFXTextures[1]);
                }
                else
                {
                    bunnyTexture.SetTexture("_playerSprite", MoveRightTextures[0]);
                    bunnyTexture.SetTexture("_playerFX0", MoveRightFXTextures[0]);
                    bunnyTexture.SetTexture("_playerFX1", MoveRightFXTextures[1]);
                }
                break;
        }
        rend.SetPropertyBlock(bunnyTexture);


        // Make the parent object look at the target
        //LookAtContainer.transform.LookAt(lookAtTarget.transform);
    }

    private bool CheckGround()
    {
        if (infiniteJump)
        {
            return true;
        }
        // CheckSphere returns true if any colliders are within the sphere
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
        return isGrounded;
    }

    // This function remains the same
    private MoveDirection evalMoveDirection(Vector2 directions)
    {
        if (directions.sqrMagnitude < 0.01f) return MoveDirection.Idle;
        if (Mathf.Abs(directions.x) > Mathf.Abs(directions.y)) return MoveDirection.Side;
        return directions.y > 0 ? MoveDirection.Up : MoveDirection.Down;
    }

    public void killPlayer()
    {
        GameManager.Instance.OnDeath();
    }

    private IEnumerator CR_Dash(float dashTime)
    {
        float originalSpeed = gameConfig.bunnyMS;
        moveSpeed = dashSpeed;
        yield return new WaitForSeconds(dashTime);
        moveSpeed = originalSpeed;
    }
}