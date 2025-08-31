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
    public Canvas WinScreen;
    public Canvas MainMenu;
    public Canvas InGameUI;

    [Header("In Game UI Elements")]
    public TMP_Text bunnyHealthAmount;

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

    public void ResetUI()
    {
        WinScreen.gameObject.SetActive(false);
        DeathScreen.gameObject.SetActive(false);
    }

    public void OnWinLevel(int LevelIndex)
    {
        WinScreen.gameObject.SetActive(true);
    }

    public void SetBunnyAmount(float Amount)
    {
        float percentage = (Amount * 100f) / gameConfig.bunnyStartDustAmount;
        percentage = Mathf.Clamp(percentage, 0, gameConfig.maxPlayerSize * 100);
        bunnyHealthAmount.text = $"Dusty size : {(int)percentage} %";
    }


    public void OnRestartClick()
    {
        GameManager.Instance.ReloadCurrentScene();
    }
}