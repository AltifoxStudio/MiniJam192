using UnityEngine;

public class DustBalls : MonoBehaviour
{
    private HasDust hasDust;
    private void Awake()
    {
        hasDust = GetComponent<HasDust>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the "Player" tag
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<HasDust>().GetDust(hasDust.dustAmount);
            GameManager.Instance.OnDustBallCollected();
            // Optional: Destroy the dust ball after it's collected
            Destroy(gameObject);
        }
    }
}