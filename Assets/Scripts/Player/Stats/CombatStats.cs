namespace Player.Stats
{
[System.Serializable]
public class CombatStats
{
    public int maxHealthPoints = 100;

    public float attackComboTime = 1f; // time window after attack to do another one
    public float jumpKickTime = 0.25f; // time window after jumping to do a jump kick
    public float jumpKickDamageMultiplier = 1.25f; // attack damage * this
    public int attackDamage = 10;
    public float attackRange = 1f;
    public float attackHeight = 0.2f;
    public float attackWidth = 0.5f;
    public float attackKnockback = 2f;
    public float attackRate = 2f; // attacks per second

    public float attackDelay = 0.1f; // Delay from animation trigger to actual attack

    public bool jumpKickUnlocked = false;
    public bool comboUnlocked = false;
}
}