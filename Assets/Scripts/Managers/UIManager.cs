using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Slider slider;

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


}



}