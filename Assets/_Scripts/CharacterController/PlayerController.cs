using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Actions
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;

    public GameConfig gameConfig;

    // Gameplay variables
    private float moveSpeed;
    private bool isDashing;
    private bool isGrounded;

    private float verticalSpeed = 0f;
    public float gravityConstant = 9.81f;
    public float groundRaycastDistance = 0.2f;
    public float groundSphereCastRadius = 0.2f;

    private Rigidbody rigidbody;

    private void Awake()
    {
        moveSpeed = gameConfig.bunnyMS;
        rigidbody = GetComponent<Rigidbody>();
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
        
        if (isDashing) return;

        if (jumpAction.IsPressed() && isGrounded)
        {
            verticalSpeed = 20f;
        }

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 move3D = new Vector3(moveValue.x, verticalSpeed, moveValue.y);
        rigidbody.linearVelocity = Vector3.Normalize(move3D) * moveSpeed;
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


}