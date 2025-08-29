using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public enum MoveDirection
{
    Up,
    Down,
    Side
}

public class PlayerController : MonoBehaviour
{
    // Actions
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;

    public GameConfig gameConfig;
    public GameObject lookAtTarget;
    public GameObject playerQuad;

    // Gameplay variables
    private float moveSpeed;
    private float jumpSpeed;
    private float dashSpeed;
    private bool isGrounded;

    // Animation 
    public Texture2D[] MoveSideTextures;
    public Texture2D[] MoveUpTextures;
    public Texture2D[] MoveDownTextures;

    private float verticalSpeed = 0f;
    public float gravityConstant = 9.81f;
    public float groundRaycastDistance = 0.2f;
    public float groundSphereCastRadius = 0.2f;
    private Rigidbody rb;
    private Renderer rend;
    private MaterialPropertyBlock bunnyTexture;
    private float dustAmount = 100f;
    private float dustSpendMoving;

    private Coroutine dashCoroutine;

    private void Awake()
    {
        moveSpeed = gameConfig.bunnyMS;
        jumpSpeed = gameConfig.bunnyJumpSpeed;
        dashSpeed = gameConfig.bunnyDashSpeed;
        dustSpendMoving = gameConfig.dustSpendMoving;

        rb = GetComponent<Rigidbody>();
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
        // Find the references to the "Move" and "Jump" actions
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");


    }

    private void Update()
    {
        ApplyPlayerScale();
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGround();
        if (!isGrounded)
        {
            EvalFallSpeed();
        }
        else
        {
            verticalSpeed = 0f;
        }
        if (dashAction.IsPressed())
        {
            if (dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
            }
            dashCoroutine = StartCoroutine(CR_Dash(0.05f));
        }
        //if (isDashing) return;

        if (jumpAction.IsPressed() && isGrounded)
        {
            verticalSpeed = 2f;
        }

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        MoveDirection moveDirection = evalMoveDirection(moveValue);
        switch (moveDirection)
        {
            case MoveDirection.Up:
                bunnyTexture.SetTexture("_playerSprite", MoveUpTextures[0]);
                break;
            case MoveDirection.Down:
                bunnyTexture.SetTexture("_playerSprite", MoveDownTextures[0]);
                break;
            case MoveDirection.Side:
                bunnyTexture.SetTexture("_playerSprite", MoveSideTextures[0]);
                break;
            default:
                break;
        }
        rend.SetPropertyBlock(bunnyTexture);
        Vector3 move3D = new Vector3(moveValue.x, verticalSpeed, moveValue.y);
        rb.linearVelocity = move3D * moveSpeed;

        Quaternion lookLeft = Quaternion.Euler(0f, 0f, 0f);
        Quaternion lookRight = Quaternion.Euler(0f, 180f, 0f);

        if (moveValue.x > 0)
        {
            playerQuad.transform.rotation = lookLeft;
        }
        else if (moveValue.x < 0)
        {
            playerQuad.transform.rotation = lookRight;
        }
        transform.LookAt(lookAtTarget.transform);
    }

    private void ApplyPlayerScale()
    {
        float ScaleFactor = dustAmount / 100;
        playerQuad.transform.localScale = new Vector3(ScaleFactor, ScaleFactor, ScaleFactor);
    }

    private bool CheckGround()
    {
        Vector3 start = transform.position;
        float radius = groundSphereCastRadius;
        float distance = groundRaycastDistance;

        return Physics.SphereCast(start, radius, Vector3.down, out RaycastHit hit, distance);

    }

    private MoveDirection evalMoveDirection(Vector2 directions)
    {
        if (Mathf.Abs(directions.x) > Mathf.Abs(directions.y))
        {
            return MoveDirection.Side;
        }
        else if (directions.y > 0)
        {
            return MoveDirection.Up;
        }

        return MoveDirection.Down;
    }

    public void GiveDust(float Amout)
    {
        dustAmount += Amout;
    }


    private void EvalFallSpeed()
    {
        verticalSpeed -= gravityConstant * Time.deltaTime;
    }

    private IEnumerator CR_Dash(float dashTime)
    {
        moveSpeed = dashSpeed;
        yield return new WaitForSeconds(dashTime);
        moveSpeed = gameConfig.bunnyMS;
    }

}