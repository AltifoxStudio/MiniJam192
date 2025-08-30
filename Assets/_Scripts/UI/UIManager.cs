using UnityEngine;
using TMPro;

enum UIManager
{
    Death,
    MainMenu,
    Pause
}

public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance { get; private set; }

    public GameConfig gameConfig;
    public Canvas DeathScreen;
    public Canvas MainMenu;
    public Canvas InGameUI;

    [Header("In Game UI Elements")]
    public TMP_Text bunnyHealthAmount;
    public TMP_Text vacuumState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        SetBunnyAmount(gameConfig.bunnyStartDustAmount);
    }

    public void OnDeath()
    {
        DeathScreen.gameObject.SetActive(true);
    }

    public void SetBunnyAmount(float Amount)
    {
        bunnyHealthAmount.text = $"Bunny Dust Amount : {Amount}";
    }

    public void SetVacuumAmount(float Amount)
    {
        float vacuumStateMax = gameConfig.vacuumMaxDust;
        int percentage = (int)(100 * Amount / vacuumStateMax);
        vacuumState.text = $"Vacuum Status: {percentage}%";
    }

    public void OnRestartClick()
    {
        GameManager.Instance.ReloadCurrentScene();
    }
}