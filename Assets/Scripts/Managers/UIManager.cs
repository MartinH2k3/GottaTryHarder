using System;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Slider slider;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject loadingScreen;
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
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void DisplayHealth(int health) {
        slider.value = health;
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

}



}