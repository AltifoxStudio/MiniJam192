using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))] 
public class VacuumCleaner : MonoBehaviour
{
    [Tooltip("Reference to the game's configuration asset.")]
    public GameConfig gameConfig;
    
    [Tooltip("The layer(s) containing objects that can be vacuumed.")]
    public LayerMask suckableLayer;

    [Tooltip("The force applied to pull objects towards the vacuum.")]
    public float suckForce = 50f;

    public ParticleFlowController dustFlowVFX;

    // Parameters from GameConfig
    private float angle;
    private float radius;
    private float moveSpeed;
    public float suckRate = 1f;

    public float currentDustAmount = 0f;
    private float dustOverloadThreshold;

    // ADDED: Pathfinding variables
    private NavMeshAgent agent;
    private Transform playerTransform;

    private Dictionary<Collider, ParticleFlowController> suckedObjectsVFX = new Dictionary<Collider, ParticleFlowController>();

    private void Awake()
    {
        // Load parameters from the ScriptableObject
        angle = gameConfig.vaccumAngle;
        radius = gameConfig.vaccumRadius;
        moveSpeed = gameConfig.vacuumSpeed;
        dustOverloadThreshold = gameConfig.vacuumMaxDust;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

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
        FollowPlayer();
       
    }

    private void FixedUpdate() {
        SuckObjects();
    }
    
    private void FollowPlayer()
    {
        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);
        }
    }


private void SuckObjects()
{
    // Find all objects currently in range
    List<Collider> detectedColliders = FindSuckableObjects();
    // Using a HashSet provides much faster lookups than a List for checking existence.
    HashSet<Collider> detectedSet = new HashSet<Collider>(detectedColliders);

    // --- 1. REMOVAL: Find objects that are no longer in range ---
    List<Collider> collidersToRemove = new List<Collider>();
    foreach (Collider trackedCollider in suckedObjectsVFX.Keys)
    {
        // If a tracked object is no longer in the detected set, mark it for removal.
        if (!detectedSet.Contains(trackedCollider))
        {
            collidersToRemove.Add(trackedCollider);
        }
    }

    // Now, safely remove the marked objects and destroy their VFX
    foreach (Collider colliderToRemove in collidersToRemove)
    {
        if (suckedObjectsVFX.TryGetValue(colliderToRemove, out ParticleFlowController vfx))
        {
            if (vfx != null) // Always check if the VFX still exists
            {
               Destroy(vfx.gameObject);
            }
            suckedObjectsVFX.Remove(colliderToRemove);
        }
    }

    // --- 2. ADDITION: Find newly detected objects ---
    foreach (Collider detectedCollider in detectedColliders)
    {
        // If a detected object is not already being tracked, start tracking it.
        if (!suckedObjectsVFX.ContainsKey(detectedCollider))
        {
            // Instantiate a new VFX for this object
            ParticleFlowController newVFX = Instantiate(dustFlowVFX, transform.position, Quaternion.identity);
            
            // Add the new object and its VFX to the dictionary
            suckedObjectsVFX.Add(detectedCollider, newVFX);
        }
    }

    // --- 3. UPDATE: Apply logic to all currently tracked objects ---
    foreach (var pair in suckedObjectsVFX)
    {
        Collider itemCollider = pair.Key;
        ParticleFlowController vfx = pair.Value;

        // Skip if the object was somehow destroyed by another script
        if (itemCollider == null) continue; 

        // Apply your suction force logic
        itemCollider.gameObject.GetComponent<HasDust>().GiveDust(suckRate);
        currentDustAmount += suckRate;
        UIGameManager.Instance.SetVacuumAmount(currentDustAmount);
        if (currentDustAmount > dustOverloadThreshold)
        {
            GameManager.Instance.OnWinLevel(0);
            Destroy(gameObject);
        }
        Rigidbody rb = itemCollider.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (transform.position - itemCollider.transform.position).normalized;
            rb.AddForce(direction * suckForce * Time.deltaTime);
        }
        
        // Update the VFX to flow from the object to the vacuum
        // You'll need a method on your 'ParticleFlowController' to handle this.
        if(vfx != null)
        {
            // Example: Assumes your VFX script has a method to set its target
            vfx.SetTargets(itemCollider.transform, transform); 
        }
    }
}

    private List<Collider> FindSuckableObjects()
    {
        Collider[] collidersInSphere = Physics.OverlapSphere(transform.position, radius, suckableLayer);
        List<Collider> objectsInCone = new List<Collider>();
        float coneAngleCos = Mathf.Cos((angle / 2f) * Mathf.Deg2Rad);

        foreach (Collider collider in collidersInSphere)
        {
            Vector3 directionToCollider = (collider.transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, directionToCollider) >= coneAngleCos)
            {
                objectsInCone.Add(collider);
            }
        }
        return objectsInCone;
    }

    private void OnDestroy()
    {
        // Destroy all active VFX when the vacuum itself is destroyed
        foreach (var vfx in suckedObjectsVFX.Values)
        {
            if (vfx != null)
            {
                Destroy(vfx.gameObject);
            }
        }
        // Clear the dictionary to be tidy
        suckedObjectsVFX.Clear();
    }

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