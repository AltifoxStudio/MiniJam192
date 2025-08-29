using UnityEngine;
using System.Collections.Generic;

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

    private void Awake() 
    {
        // Load parameters from the ScriptableObject
        angle = gameConfig.vaccumAngle;
        radius = gameConfig.vaccumRadius;
        moveSpeed = gameConfig.vacuumSpeed;
    }

    private void Update()
    {
        // Example: Move the vacuum forward (optional, based on your game logic)
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        
        // Continuously check for and suck objects in the cone
        SuckObjects();
    }

    /// <summary>
    /// Finds and applies a force to all suckable objects within the vacuum's cone.
    /// </summary>
    private void SuckObjects()
    {
        List<Collider> objectsToSuck = FindSuckableObjects();

        foreach (Collider itemCollider in objectsToSuck)
        {
            // Check if the object has a Rigidbody to apply force to
            Rigidbody rb = itemCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calculate direction from the object to the vacuum cleaner
                Vector3 direction = (transform.position - itemCollider.transform.position).normalized;
                
                // Apply a force to pull the object in
                rb.AddForce(direction * suckForce * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Finds all colliders on the 'suckableLayer' within a cone in front of the vacuum.
    /// This uses the more performant dot product method.
    /// </summary>
    /// <returns>A list of colliders found within the cone.</returns>
    private List<Collider> FindSuckableObjects()
    {
        // Get all colliders within a sphere that encompasses the cone
        Collider[] collidersInSphere = Physics.OverlapSphere(transform.position, radius, suckableLayer);

        List<Collider> objectsInCone = new List<Collider>();

        // Pre-calculate the cosine of half the cone angle for the dot product comparison
        float coneAngleCos = Mathf.Cos((angle / 2f) * Mathf.Deg2Rad);

        foreach (Collider collider in collidersInSphere)
        {
            // Get the direction vector from the vacuum to the collider
            Vector3 directionToCollider = (collider.transform.position - transform.position).normalized;

            // The dot product of two normalized vectors is the cosine of the angle between them.
            // If the dot product is greater than our pre-calculated cosine, it's within the angle.
            if (Vector3.Dot(transform.forward, directionToCollider) >= coneAngleCos)
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
        // Ensure parameters are loaded even when not in play mode for visualization
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

        // Draw the arc for the cone
#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(0, 1, 1, 0.1f);
        UnityEditor.Handles.DrawSolidArc(position, transform.up, leftRayDirection, angle, radius);
#endif
    }
}