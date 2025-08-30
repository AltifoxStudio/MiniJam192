using UnityEngine;

enum UIManager
{
    Death,
    MainMenu,
    Pause
}

public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance { get; private set; }
    public Canvas DeathScreen;
    public Canvas MainMenu;

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
    }

    public void OnDeath()
    {
        DeathScreen.gameObject.SetActive(true);
    }

    public void OnRestartClick()
    {
        GameManager.Instance.ReloadCurrentScene();
    }
}