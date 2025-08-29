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
        float vecticalSpeed = 0f;
        isGrounded = CheckGround();
        if (!isGrounded)
        {
            vecticalSpeed = EvalFallSpeed();
            Debug.Log(vecticalSpeed);
        }
        
        if (isDashing) return;

        if (jumpAction.IsPressed())
        {
            vecticalSpeed = 1f;
            Debug.Log("Jump !");
        }
        Debug.Log(isGrounded);

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 move3D = new Vector3(moveValue.x, vecticalSpeed, moveValue.y);
        rigidbody.linearVelocity = Vector3.Normalize(move3D) * moveSpeed;
    }

    private bool CheckGround()
    {
        Vector3 start = transform.position;
        float radius = groundSphereCastRadius;
        float distance = groundRaycastDistance;

        return Physics.SphereCast(start, radius, Vector3.down, out RaycastHit hit, distance);

    }


    private float EvalFallSpeed()
    {
        float currentSpeed = rigidbody.linearVelocity.y;
        return currentSpeed - gravityConstant * Time.deltaTime;
    }


}