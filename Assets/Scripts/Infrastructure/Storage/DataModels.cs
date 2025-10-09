using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Storage
{

/// <summary> Per-level stats </summary>
[Serializable]
public class LevelStats
{
    public int lowestDeaths = 0;
    public float bestTimeSeconds = float.MaxValue;
    public float totalTimeSeconds = 0f;
    public bool unlocked = false;
    public bool completed = false;
}

[Serializable]
public class SaveData
{
    public int version = 1;

    public List<LevelStats> levels = new List<LevelStats>();
    private int _currentLevelIndex = 0;

    /// <summary>
    /// Get stats for a given level index. If it doesn’t exist yet, expand the list.
    /// </summary>
    public LevelStats GetLevel(int index)
    {
        // Expand list until it has this index
        while (levels.Count <= index)
            levels.Add(new LevelStats());

        return levels[index];
    }

    public int TotalDeaths() {
        int total = 0;
        foreach (var level in levels) {
            total += level.lowestDeaths;
        }
        return total;
    }

    public float BestRunTime() {
        float total = 0f;
        foreach (var level in levels) {
            total += level.bestTimeSeconds;
        }
        return total;
    }
}
}