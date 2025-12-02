using Player;
using UnityEngine;
using ScriptableObjects; // added

namespace Managers
{
public class LevelUpManager: MonoBehaviour
{
    [SerializeField] private PlayerLevelUpStats levelUpStatsData; // replaced raw array with ScriptableObject
    [SerializeField] private PlayerController player;

    /// <summary>
    /// Levels up the player based on their death count. Adjusts the base stat class values.
    /// </summary>
    /// <param name="level">Death count of the player</param>
    public void LevelUpPlayer(int level) {
        var statsArray = levelUpStatsData.levelUpStats; // access ScriptableObject data
        for (var i = 0; i < Mathf.Min(level, statsArray.Length); i++) { // added bounds safeguard
            var stats = statsArray[i];
            player.movementStats.walkSpeed *= stats.movementSpeedRateChange;
            player.jumpStats.extraAirJumps += stats.addsMultiJump;
            player.jumpStats.jumpStrength *= stats.jumpStrengthRateChange;
            player.jumpStats.damageTakenOnJump += stats.addsDamageTakenOnJump;
            player.dashStats.dashSpeed *= stats.dashSpeedRateChange;
            if (stats.attackComboUnlocked) {
                player.combatStats.comboUnlocked = true;
            }
            if (stats.jumpKickUnlocked) {
                player.combatStats.jumpKickUnlocked = true;
            }
            player.combatStats.attackDamage += stats.damageAdded;
            player.combatStats.lifeSteal += stats.addsLifeSteal;
        }
    }
} }