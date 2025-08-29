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

    // Parameters from GameConfig
    private float angle;
    private float radius;
    private float moveSpeed;
    public float suckRate = 1f;

    // ADDED: Pathfinding variables
    private NavMeshAgent agent;
    private Transform playerTransform;

    private void Awake() 
    {
        // Load parameters from the ScriptableObject
        angle = gameConfig.vaccumAngle;
        radius = gameConfig.vaccumRadius;
        moveSpeed = gameConfig.vacuumSpeed;

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
        List<Collider> objectsToSuck = FindSuckableObjects();

        foreach (Collider itemCollider in objectsToSuck)
        {
            itemCollider.gameObject.GetComponent<hasDust>().GetDust(suckRate);
            Rigidbody rb = itemCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (transform.position - itemCollider.transform.position).normalized;
                rb.AddForce(direction * suckForce * Time.deltaTime);
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
        foreach (var objects in objectsInCone)
        {
            Debug.Log(objects);
        }
        return objectsInCone;
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