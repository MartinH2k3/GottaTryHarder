using System.Collections.Generic;
using OtherObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
public class HintManager: MonoBehaviour {
    private static HintManager Instance { get; set; }

    private readonly HashSet<string> _shown = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var hints = FindObjectsByType<Hint>(FindObjectsSortMode.None);

        foreach (var hint in hints)
        {
            if (hint == null) continue;

            // If no ID is set, we can't track it reliably
            if (string.IsNullOrWhiteSpace(hint.HintID))
            {
                Debug.LogWarning($"Hint '{hint.name}' has empty HintId. It won't be tracked.");
                continue;
            }

            string key = MakeKey(scene.name, hint.HintID);

            // If it was shown before, remove it immediately
            if (_shown.Contains(key))
            {
                hint.RemoveHint(); // or: hint.gameObject.SetActive(false);
                continue;
            }

            // Subscribe (closure captures the hint instance)
            hint.HintDisplayed -= () => OnHintDisplayed(scene.name, hint); // no-op (can't remove lambdas reliably)
            hint.HintDisplayed += () => OnHintDisplayed(scene.name, hint);
        }

    }

    private void OnHintDisplayed(string sceneName, Hint hint)
    {
        if (hint == null) return;
        if (string.IsNullOrWhiteSpace(hint.HintID)) return;

        string key = MakeKey(sceneName, hint.HintID);
        _shown.Add(key);
    }

    private static string MakeKey(string sceneName, string hintId)
        => $"{sceneName}:{hintId}";
}
}