using Enemies.Stats;

namespace Enemies.Melee.Stats
{
[System.Serializable]
public class MeleeCombatStats: CombatStats
{
    public float attackDelay = 0.1f; // Delay from animation trigger to actual attack
    public float attackRange = 1f;
    public float attackHeight = 0.2f;
    public float attackWidth = 0.5f;
}
}