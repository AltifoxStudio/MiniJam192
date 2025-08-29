using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI; // ADDED: Required for NavMeshAgent

[RequireComponent(typeof(NavMeshAgent))] // ADDED: Ensures this object always has a NavMeshAgent
public class VacuumCleaner : MonoBehaviour
{
    [Tooltip("Reference to the game's configuration asset.")]
    public GameConfig gameConfig;
    
    [Tooltip("The layer(s) containing objects that can be vacuumed.")]
    public LayerMask suckableLayer;

    [Tooltip("The force applied to pull objects towards the vacuum.")]
    public float suckForce = 50f;

    // Parameters from GameConfig
    private float angle;
    private float radius;
    private float moveSpeed;

    // ADDED: Pathfinding variables
    private NavMeshAgent agent;
    private Transform playerTransform;

    private void Awake() 
    {
        // Load parameters from the ScriptableObject
        angle = gameConfig.vaccumAngle;
        radius = gameConfig.vaccumRadius;
        moveSpeed = gameConfig.vacuumSpeed;

        // ADDED: Get the NavMeshAgent component attached to this GameObject
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed; // Set the agent's speed from your config
    }

    // ADDED: Use Start to find the player to ensure it exists in the scene
    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure your player GameObject has the 'Player' tag.", this);
        }
    }

    private void Update()
    {
        // ADDED: Tell the agent to follow the player's position
        FollowPlayer();
        
        SuckObjects();
    }
    
    // ADDED: A new method to handle the following logic
    /// <summary>
    /// Sets the NavMeshAgent's destination to the player's current position.
    /// </summary>
    private void FollowPlayer()
    {
        // Only try to follow if we have a valid reference to the player
        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    /// <summary>
    /// Finds and applies a force to all suckable objects within the vacuum's cone.
    /// </summary>
    private void SuckObjects()
    {
        List<Collider> objectsToSuck = FindSuckableObjects();

        foreach (Collider itemCollider in objectsToSuck)
        {
            Rigidbody rb = itemCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (transform.position - itemCollider.transform.position).normalized;
                rb.AddForce(direction * suckForce * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Finds all colliders on the 'suckableLayer' within a cone in front of the vacuum.
    /// </summary>
    /// <returns>A list of colliders found within the cone.</returns>
    private List<Collider> FindSuckableObjects()
    {
        Collider[] collidersInSphere = Physics.OverlapSphere(transform.position, radius, suckableLayer);
        List<Collider> objectsInCone = new List<Collider>();
        float coneAngleCos = Mathf.Cos((angle / 2f) * Mathf.Deg2Rad);

        foreach (Collider collider in collidersInSphere)
        {
            Vector3 directionToCollider = (collider.transform.position - transform.position).normalized;
            if (Vector3.Dot(-transform.forward, directionToCollider) >= coneAngleCos)
            {
                objectsInCone.Add(collider);
            }
        }
        return objectsInCone;
    }

    /// <summary>
    /// Draws a gizmo in the editor to visualize the vacuum's range and angle.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (gameConfig != null && (angle == 0 || radius == 0))
        {
            angle = gameConfig.vaccumAngle;
            radius = gameConfig.vaccumRadius;
        }

        Gizmos.color = Color.cyan;
        Vector3 forward = transform.forward;
        Vector3 position = transform.position;

        Quaternion leftRayRotation = Quaternion.AngleAxis(-angle / 2f, transform.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(angle / 2f, transform.up);
        
        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.DrawLine(position, position + leftRayDirection * radius);
        Gizmos.DrawLine(position, position + rightRayDirection * radius);

#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(0, 1, 1, 0.1f);
        UnityEditor.Handles.DrawSolidArc(position, transform.up, leftRayDirection, angle, radius);
#endif
    }
}