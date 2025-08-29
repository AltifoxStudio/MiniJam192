using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController player;
    public float height = 1f;
    public float yOffset = 0.5f;

    private void Update()
    {
        //transform.LookAt(player.gameObject.transform);
        Vector3 CameraPosition = player.transform.position;
        CameraPosition.z -= yOffset;
        CameraPosition.y += height;
        transform.position = CameraPosition;
        transform.LookAt(player.transform);
    }
}