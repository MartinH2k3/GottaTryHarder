using UnityEngine;

namespace SerializableData
{
[CreateAssetMenu(fileName = "PlayerLevelUpStats", menuName = "ScriptableObjects/Player Level Up Stats")]
public class PlayerLevelUpStats : ScriptableObject
{
    public LevelUpStats[] levelUpStats;
}

[System.Serializable]
public class LevelUpStats {
    public int addsLifeSteal = 0;
    public int addsDamageTakenOnJump = 0;
    public int addsMultiJump = 0;
    public int damageAdded = 0;
    public bool attackComboUnlocked = false;
    public bool jumpKickUnlocked = false;
    public float jumpStrengthRateChange = 0f;
    public float movementSpeedRateChange = 0f;
    public float dashSpeedRateChange = 0f;
}
}