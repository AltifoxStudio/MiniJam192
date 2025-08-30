using UnityEngine;

public class HasDust : MonoBehaviour
{
    public GameConfig gameConfig;
    public float dustAmount;
    public float initDustAmount = 100f;
    public float minSize = 0.5f;
    public float maxSize = 3f;
    private float scaleFactor = 1f;
    public GameObject RenderQuad;

    private void Start()
    {
        dustAmount = initDustAmount;
        if (gameObject.tag == "Player")
        {
            dustAmount = gameConfig.bunnyStartDustAmount;
        }
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
        if (gameObject.tag == "Player")
        {
            UIGameManager.Instance.SetBunnyAmount(dustAmount);
        }
    }

    public void GiveDust(float amount)
    {
        dustAmount -= amount;
        if (gameObject.tag == "Player")
        {
            if (dustAmount < 0)
            {
                gameObject.GetComponent<PlayerController>().killPlayer();
                Destroy(gameObject);
            }
            else
            {
                UIGameManager.Instance.SetBunnyAmount(dustAmount);
            }
        }
    }
   
}