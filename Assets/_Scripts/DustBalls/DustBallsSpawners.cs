using UnityEngine;

public class DustBallsSpawners : MonoBehaviour {
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.1f, 0.1f, 0.1f));
    }
}