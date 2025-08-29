using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // Actions
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;

    public GameConfig gameConfig;
    public GameObject lookAtTarget;

    // Gameplay variables
    private float moveSpeed;
    private float jumpSpeed;
    private float dashSpeed;
    private bool isDashing;
    private bool isGrounded;

    private float verticalSpeed = 0f;
    public float gravityConstant = 9.81f;
    public float groundRaycastDistance = 0.2f;
    public float groundSphereCastRadius = 0.2f;
    private Rigidbody rb;

    private Coroutine dashCoroutine;

    private void Awake()
    {
        moveSpeed = gameConfig.bunnyMS;
        jumpSpeed = gameConfig.bunnyJumpSpeed;
        dashSpeed = gameConfig.bunnyDashSpeed;

        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Find the references to the "Move" and "Jump" actions
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");


    }

    private void FixedUpdate()
    {
        isGrounded = CheckGround();
        Debug.Log(isGrounded);
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
        Vector3 move3D = new Vector3(moveValue.x, verticalSpeed, moveValue.y);
        rb.linearVelocity = move3D * moveSpeed;
        transform.LookAt(lookAtTarget.transform);
    }

    private bool CheckGround()
    {
        Vector3 start = transform.position;
        float radius = groundSphereCastRadius;
        float distance = groundRaycastDistance;

        return Physics.SphereCast(start, radius, Vector3.down, out RaycastHit hit, distance);

    }


    private void EvalFallSpeed()
    {
        verticalSpeed -= gravityConstant * Time.deltaTime;
    }

    private IEnumerator CR_Dash(float dashTime)
    {
        isDashing = true;
        moveSpeed = dashSpeed;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        moveSpeed = gameConfig.bunnyMS;
    }

}