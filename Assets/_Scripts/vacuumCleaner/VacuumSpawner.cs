using UnityEngine;

public class VacuumSpawner : MonoBehaviour {
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.1f, 0.1f, 0.1f));
    }
}