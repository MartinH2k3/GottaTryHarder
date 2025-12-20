using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI deathCountText;
    [SerializeField] private TextMeshProUGUI timeSpentText;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject endCutscene;
    [SerializeField] private GameObject hud;
    [SerializeField] private float loadingScreenSeconds = 4f;

    private float _loadingScreenEndTime;
    public bool LoadingScreenFinished => _loadingScreenEndTime < Time.time;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void InitiateHUD(int maxHealth, int deathCount = 0, float timeSpent = 0) {
        hud.SetActive(true);
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        deathCountText.text = deathCount.ToString();
        timeSpentText.text = FormatTime(timeSpent);
    }

    public void HideHUD() {
        hud.SetActive(false);
    }

    private string FormatTime(float seconds) {
        string output = "";
        int hours = Mathf.FloorToInt(seconds / 3600);
        int minutes = Mathf.FloorToInt((seconds % 3600) / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        if (hours > 0)
            output += hours.ToString() + "h";
        if (minutes > 0)
            output += minutes.ToString() + "m";
        if (secs > 0)
            output += secs.ToString() + "s";
        return output == "" ? "0s" : output;
    }

    public void DisplayHealth(int health) {
        healthBar.value = health;
    }

    public void DisplayDeathCount(int deathCount) {
        deathCountText.text = deathCount.ToString();
    }

    public void DisplayTimeSpent(float timeSpent) {
        timeSpentText.text = FormatTime(timeSpent);
    }

    public void ShowPauseMenu() {
        pauseMenu.SetActive(true);
    }

    public void HidePauseMenu() {
        pauseMenu.SetActive(false);
    }

    public void ShowLoadingScreen() {
        _loadingScreenEndTime = Time.time + loadingScreenSeconds;
        loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen() {
        loadingScreen.SetActive(false);
    }

    public void ShowEndCutscene() {
        endCutscene.SetActive(true);
    }

    public void HideEndCutscene() {
        endCutscene.SetActive(false);
    }

}



}