using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance { get; private set; }

    public GameConfig gameConfig;
    public GameObject DeathScreen;
    public GameObject[] WinScreens;
    public Image bunnyDustAmountImage;
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
        for (int i = 0; i < WinScreens.Length; i++)
        {
            WinScreens[i].SetActive(false);
        }
        DeathScreen.gameObject.SetActive(false);
    }

    private void Update() {
        switch (GameManager.Instance.gameState)
        {
            case GameState.Play:
                for (int i = 0; i < WinScreens.Length; i++)
                {
                    WinScreens[i].SetActive(false);
                }
                DeathScreen.gameObject.SetActive(false);
                InGameUI.gameObject.SetActive(true);
                break;

            case GameState.win:
                Debug.Log("current level "+GameManager.Instance.currentLevelIndex);
                for (int i = 0; i < WinScreens.Length; i++)
                {
                    if (i == GameManager.Instance.currentLevelIndex)
                    {
                        WinScreens[i].SetActive(true);
                    }
                    else
                    {
                        WinScreens[i].SetActive(false);
                    }

                }
                DeathScreen.SetActive(false);
                InGameUI.SetActive(false);
                break;

            case GameState.Death:
                for (int i = 0; i < WinScreens.Length; i++)
                {
                    WinScreens[i].SetActive(false);
                }
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
        float percentage = (Amount * 100f) / gameConfig.bunnyMaxDustAmount;
        percentage = Mathf.Clamp(percentage, 0, gameConfig.maxPlayerSize * 100);
        bunnyDustAmountImage.fillAmount = percentage / 100f;
        bunnyHealthAmount.text = $"Dusty size : {(int)percentage} %";
    }


    public void OnRestartClick()
    {
        GameManager.Instance.ReloadCurrentScene();
    }
}