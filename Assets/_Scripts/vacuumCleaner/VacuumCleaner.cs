using UnityEngine;
using System.Collections.Generic;
using AltifoxStudio.AltifoxAudioManager;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

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

    public AltifoxPersistentSFXPlayer SoundPlayer;
    private bool soundIsPlaying = false;

    // Parameters from GameConfig
    private float angle;
    private float radius;
    private float vacuumHeight; // ADDED: Height of the vacuum cone
    private float moveSpeed;
    public float suckRate = 1f;
    public TMP_Text Status;

    public Image OverloadStatus; 

    public float currentDustAmount = 0f;
    private float dustOverloadThreshold;

    private NavMeshAgent agent;
    private Transform playerTransform;

    private Dictionary<Collider, ParticleFlowController> suckedObjectsVFX = new Dictionary<Collider, ParticleFlowController>();

    private void Awake()
    {
        // Load parameters from the ScriptableObject
        angle = gameConfig.vaccumAngle;
        radius = gameConfig.vaccumRadius;
        vacuumHeight = gameConfig.vacuumHeight; // ADDED: Load the height
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
        if (!soundIsPlaying)
        {
            try
            {
                SoundPlayer.Play();
                if (SoundPlayer.audioSource.isPlaying)
                {
                    soundIsPlaying = true;  
                }
            }
            catch (System.Exception)
            {
                // pass 
            }

        }
    }

    private void FixedUpdate()
    {
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
            SetPercentage();
            if (currentDustAmount > dustOverloadThreshold)
            {
                Debug.Log("Destroying");
                Destroy(gameObject);
            }
            Rigidbody rb = itemCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (transform.position - itemCollider.transform.position).normalized;
                rb.AddForce(direction * suckForce * Time.deltaTime);
            }

            // Update the VFX to flow from the object to the vacuum
            if (vfx != null)
            {
                // Example: Assumes your VFX script has a method to set its target
                vfx.SetTargets(itemCollider.transform, transform);
            }
        }
    }

    private void SetPercentage()
    {
        int percentage = (int)(100 * currentDustAmount / dustOverloadThreshold);
        Status.text = $"{percentage}%";
        OverloadStatus.fillAmount = (float)percentage / 100.0f;
    }

    private List<Collider> FindSuckableObjects()
    {
        Collider[] collidersInSphere = Physics.OverlapSphere(transform.position, radius, suckableLayer);
        List<Collider> objectsInCone = new List<Collider>();
        float coneAngleCos = Mathf.Cos((angle / 2f) * Mathf.Deg2Rad);

        foreach (Collider collider in collidersInSphere)
        {
            // MODIFIED: This check is now more robust and uses the new vacuumHeight variable.
            // It ensures the object is above the vacuum's base but below its max height reach.
            float heightDifference = collider.transform.position.y - transform.position.y;
            if (heightDifference < vacuumHeight)
            {
                Vector3 directionToCollider = (collider.transform.position - transform.position).normalized;
                // We ignore the Y-axis for the angle check to make it a true cone, not a spherical segment.
                directionToCollider.y = 0;

                if (Vector3.Dot(transform.forward, directionToCollider.normalized) >= coneAngleCos)
                {
                    objectsInCone.Add(collider);
                }
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
        try
        {
            SFXManager.Instance.deathOfAVacuum.transform.position = transform.position;
            if (GameManager.Instance.gameState == GameState.Play)
            {
                SFXManager.Instance.deathOfAVacuum.PreloadAndPlay();
            }
            AltifoxAudioManager.Instance.ReleaseAltifoxAudioSource(SoundPlayer.audioSource);
        }
        catch (System.Exception)
        {
            // pass
        }
       
    }

    // MODIFIED: The entire Gizmo drawing function is updated for 3D preview.
    private void OnDrawGizmosSelected()
    {
        // Ensure parameters are loaded from GameConfig for editor preview
        if (gameConfig != null)
        {
            angle = gameConfig.vaccumAngle;
            radius = gameConfig.vaccumRadius;
            vacuumHeight = gameConfig.vacuumHeight;
        }

        Vector3 position = transform.position;
        Vector3 forward = transform.forward;
        Vector3 up = transform.up;

        // Calculate cone edge directions
        Quaternion leftRayRotation = Quaternion.AngleAxis(-angle / 2f, up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(angle / 2f, up);
        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        // Define the top position of the cone
        Vector3 topPosition = position + up * vacuumHeight;

        // --- Draw the Bottom Arc ---
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(position, position + leftRayDirection * radius);
        Gizmos.DrawLine(position, position + rightRayDirection * radius);

        // --- Draw the Top Arc ---
        Gizmos.DrawLine(topPosition, topPosition + leftRayDirection * radius);
        Gizmos.DrawLine(topPosition, topPosition + rightRayDirection * radius);

        // --- Draw Vertical Connecting Lines ---
        Gizmos.DrawLine(position + leftRayDirection * radius, topPosition + leftRayDirection * radius);
        Gizmos.DrawLine(position + rightRayDirection * radius, topPosition + rightRayDirection * radius);

#if UNITY_EDITOR
        // Draw the filled arcs for better visualization
        UnityEditor.Handles.color = new Color(0, 1, 1, 0.1f);
        // Bottom arc
        UnityEditor.Handles.DrawSolidArc(position, up, leftRayDirection, angle, radius);
        // Top arc
        UnityEditor.Handles.DrawSolidArc(topPosition, up, leftRayDirection, angle, radius);
#endif
    }
}