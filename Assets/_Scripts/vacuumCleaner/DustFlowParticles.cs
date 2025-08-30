using UnityEngine;

public class ParticleFlowController : MonoBehaviour
{
    // These will be set by the VacuumCleaner script
    private Transform _source;
    private Transform _destination;

    // You can still tweak this in the Inspector for the prefab
    public float particleSpeed = 10f;

    private ParticleSystem _particleSystem;

    void Awake()
    {
        // Get the Particle System component. Awake is better than Start for this.
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    /// <summary>
    /// This is the new public method that the VacuumCleaner will call.
    /// </summary>
    /// <param name="sourceTransform">The object the particles should come FROM.</param>
    /// <param name="destinationTransform">The object the particles should go TO.</param>
    public void SetTargets(Transform sourceTransform, Transform destinationTransform)
    {
        _source = sourceTransform;
        _destination = destinationTransform;
    }

    void Update()
    {
        // Don't do anything until the targets have been assigned.
        if (_source == null || _destination == null || _particleSystem == null)
        {
            return;
        }

        // 1. Make this VFX controller follow the source object
        transform.position = _source.position;

        // 2. Aim the particle system towards the destination (the vacuum)
        transform.LookAt(_destination);

        // 3. Calculate the distance to the destination
        float distance = Vector3.Distance(transform.position, _destination.position);

        // 4. Calculate the required lifetime for particles to reach the destination
        float lifetime = distance / particleSpeed;

        // 5. Apply the calculated speed and lifetime to the particle system
        var mainModule = _particleSystem.main;
        mainModule.startSpeed = particleSpeed;
        mainModule.startLifetime = lifetime;
    }
}