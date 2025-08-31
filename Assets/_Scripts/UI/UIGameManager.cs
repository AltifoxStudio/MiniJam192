using UnityEngine;
using TMPro;



public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance { get; private set; }

    public GameConfig gameConfig;
    public GameObject DeathScreen;
    public GameObject WinScreen;
    public GameObject InGameUI;

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
        //
    }

    public void ResetUI()
    {
        WinScreen.gameObject.SetActive(false);
        DeathScreen.gameObject.SetActive(false);
    }

    private void Update() {
        switch (GameManager.Instance.gameState)
        {
            case GameState.Play:
                WinScreen.SetActive(false);
                DeathScreen.gameObject.SetActive(false);
                InGameUI.gameObject.SetActive(true);
                break;

            case GameState.win:
                WinScreen.SetActive(true);
                DeathScreen.SetActive(false);
                InGameUI.SetActive(false);
                break;

            case GameState.Death:
                WinScreen.SetActive(false);
                DeathScreen.SetActive(true);
                InGameUI.SetActive(false);
                break;

            default:
                break;
        }
    }

    public void OnWinLevel(int LevelIndex)
    {
       //
    }

    public void OnStartLevel()
    {
        GameManager.Instance.gameState = GameState.Play;
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