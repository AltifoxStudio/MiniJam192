using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    public float width;
    public float height;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
    }

}