using System;

namespace SerializableData
{
[Serializable]
public class LevelCompletionData
{
    public LevelCompletionStats[] levelStats;
}

[Serializable]
public class LevelCompletionStats
{
    public int deathCount;
    public float timeSeconds;
}
}