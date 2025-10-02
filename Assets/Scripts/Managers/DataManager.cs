using Infrastructure.Storage;
using UnityEngine;

namespace Managers
{
public class DataManager: MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public SaveData saveData;

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveData = SaveSystem.LoadData();
    }

    private void OnApplicationQuit()
    {
        SaveSystem.SaveData(saveData);
    }
}
}