using UnityEngine;

/// <summary>
/// A C# script that makes an object always face the main camera.
/// It includes an option to adjust its scale to maintain a consistent size on the screen
/// regardless of the distance from the camera.
/// </summary>
public class Billboard : MonoBehaviour
{
    [Tooltip("Toggle this to enable or disable scaling with distance.")]
    public bool scaleWithDistance = true;

    private Transform mainCameraTransform;
    private float initialScale;

    /// <summary>
    /// Start is called before the first frame update.
    /// We find the main camera's transform and store the object's initial scale.
    /// </summary>
    void Start()
    {
        // Find the main camera and store its transform for efficiency.
        GameObject mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCameraObject != null)
        {
            mainCameraTransform = mainCameraObject.transform;
        }
        else
        {
            // Log an error if the camera is not found to help with debugging.
            Debug.LogError("Billboard script requires a camera tagged 'MainCamera' to function.");
        }

        // Store the initial scale of the object.
        initialScale = transform.localScale.x;
    }

    /// <summary>
    /// Update is called once per frame. We use it to ensure the object is
    /// always facing the camera.
    /// </summary>
    void Update()
    {
        // Safety check to ensure the camera was found in the Start method.
        if (mainCameraTransform == null)
        {
            return;
        }

        // Make the object face the camera. We use LookAt with the negated
        // forward vector and a corrected up vector to prevent flipping.
        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                         mainCameraTransform.rotation * Vector3.up);

        // Adjust the object's scale if the toggle is enabled.
        if (scaleWithDistance)
        {
            float distance = Vector3.Distance(transform.position, mainCameraTransform.position);
            transform.localScale = Vector3.one * (initialScale * distance);
        }
    }
}
