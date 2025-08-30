using UnityEngine;

public class HasDust : MonoBehaviour
{
    public float dustAmount;
    public float initDustAmount = 100f;
    public float minSize = 0.5f;
    public float maxSize = 3f;
    private float scaleFactor = 1f;
    public GameObject RenderQuad;

    private void Start() {
        dustAmount = initDustAmount;
    }

    private void ApplyScale()
    {
        scaleFactor = dustAmount / initDustAmount;
        float scale = Mathf.Lerp(minSize, maxSize, scaleFactor);
        RenderQuad.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void Update() {
        ApplyScale();
    }

    public void GetDust(float amount)
    {
        dustAmount += amount;
    }

    public void GiveDust(float amount)
    {
        dustAmount -= amount;
        if (dustAmount < 0)
        {
            if (gameObject.tag == "Player")
            {
                gameObject.GetComponent<PlayerController>().killPlayer();
            }
            Destroy(gameObject);
        }
    }
   
}