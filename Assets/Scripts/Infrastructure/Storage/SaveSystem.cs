using System.IO;
using UnityEngine;

namespace Infrastructure.Storage
{
public static class SaveSystem {
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void SaveData(SaveData data) {
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static SaveData LoadData() {
        if (!File.Exists(SavePath)) {
            return new SaveData();
        }

        var json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }
}
}