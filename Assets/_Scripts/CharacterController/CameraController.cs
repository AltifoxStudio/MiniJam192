using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float height = 1f;
    public float yOffset = 0.5f;

    [Tooltip("Approximately the time it will take for the camera to reach the target. A smaller value will feel more responsive.")]
    public float smoothTime = 0.3f; 

    // This variable is used by SmoothDamp to track the camera's current velocity.
    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        // Calculate the desired position for the camera
        Vector3 desiredPosition = player.position;
        desiredPosition.z -= yOffset;
        desiredPosition.y += height;

        // Use SmoothDamp to gradually move the camera towards the desired position.
        // It's like attaching the camera to the target with a spring.
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // Make the camera continue to look at the player
        transform.LookAt(player);
    }
}