using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController player;
    public float height = 1;

    private void Update()
    {
        //transform.LookAt(player.gameObject.transform);
        Vector3 CameraPosition = player.transform.position;
        CameraPosition.z -= 2;
        CameraPosition.y += 1;
        transform.position = CameraPosition;
        transform.LookAt(player.transform);
    }
}