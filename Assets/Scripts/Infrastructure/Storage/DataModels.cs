using System;
using UnityEngine;

namespace Infrastructure.Storage
{

/// <summary> Per-level stats </summary>
public class LevelStats
{
    public int LowestDeaths = 0;
    public float BestTimeSeconds = float.MaxValue;
    public float TotalTimeSeconds = 0f;
    public bool Unlocked = false;
    public bool Completed = false;

}
}