using UnityEngine;

public class hasDust : MonoBehaviour
{
    private float dustAmount = 100f;
    public float initDustAmount = 100f;
    public float minSize = 0.5f;
    public float maxSize = 3f;
    private float scaleFactor = 1f;
    public GameObject RenderQuad;

    private void ApplyScale()
    {
        scaleFactor = dustAmount / initDustAmount;
        float scale = Mathf.Lerp(maxSize, minSize, scaleFactor);
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
    }
   
}