using UnityEngine;

public class DustBalls : MonoBehaviour
{
    private bool collected = false;
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the "Player" tag
        if (other.CompareTag("Player"))
        {
            collected = true;
            Debug.Log("Dust ball collected by the Player!");

            // Optional: Destroy the dust ball after it's collected
            Destroy(gameObject);
        }
    }
}