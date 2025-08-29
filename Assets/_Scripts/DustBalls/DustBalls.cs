using UnityEngine;

public class DustBalls : MonoBehaviour
{
    public float dustAmount = 100;
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the "Player" tag
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().GiveDust(dustAmount);
            Debug.Log("Dust ball collected by the Player!");
            GameManager.Instance.OnDustBallCollected();
            // Optional: Destroy the dust ball after it's collected
            Destroy(gameObject);
        }
    }
}