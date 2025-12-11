using System;
using Player;
using UnityEngine;
using SerializableData;
using UnityEngine.SceneManagement; // added

namespace Managers
{
public class GameManager: MonoBehaviour
{
    [Header("Player Prefabs")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private PlayerController skinnyPlayerPrefab;
    [SerializeField] private PlayerController midPlayerPrefab;
    [SerializeField] private PlayerController buffPlayerPrefab;
    private PlayerController player;

    [Header("Level Data")]
    [SerializeField] private PlayerLevelUpStats levelUpStatsData;
    private LevelCompletionData levelCompletionData;
    private string SaveFilePath => Application.persistentDataPath + "/levelData.json";

    private int _currentLevelIndex = 0;
    private float _spawnTimestamp;
    private float _prevLevelsTime;
    public int DeathCount { get; private set; }


    private void Start() {
        LoadLevelData();

        _currentLevelIndex = GetCurrentLevelIndex();
        _spawnTimestamp = Time.time;
        _prevLevelsTime = GetSpentTime(_currentLevelIndex);

        DeathCount = GetDeaths(_currentLevelIndex);

        var playerPrefab = ChoosePlayerPrefab(DeathCount);
        player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        LevelUpPlayer(DeathCount);
    }

    /// <summary>
    /// Levels up the player based on their death count. Adjusts the base stat class values.
    /// </summary>
    /// <param name="level">Death count of the player</param>
    public void LevelUpPlayer(int level) {
        var statsArray = levelUpStatsData.levelUpStats;
        for (var i = 0; i < Mathf.Min(level, statsArray.Length); i++) {
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

    public int GetDeathCount(int level) {
        var statsArray = levelCompletionData.levelStats;
        int deaths = 0;
        for (var i = 0; i < statsArray.Length; i++) {
            var stats = statsArray[i];
            deaths += stats.deathCount;
        }
        return deaths;
    }

    public float GetTimeSoFar() {
        return _prevLevelsTime + (Time.time - _spawnTimestamp);
    }

    private int GetDeaths(int levelIndex = -1) {
        var statsArray = levelCompletionData.levelStats;

        if (levelIndex < 0) {
            levelIndex = statsArray.Length;
        }

        int deaths = 0;
        for (var i = 0; i < levelIndex; i++) {
            var stats = statsArray[i];
            deaths += stats.deathCount;
        }
        return deaths;
    }

    public float GetSpentTime(int levelIndex = -1) {
        var statsArray = levelCompletionData.levelStats;

        if (levelIndex < 0) {
            levelIndex = statsArray.Length;
        }

        float time = 0f;
        for (var i = 0; i < levelIndex; i++) {
            var stats = statsArray[i];
            time += stats.timeSeconds;
        }
        return time;
    }

    private int GetCurrentLevelIndex() {
        return SceneManager.GetActiveScene().buildIndex - 1; // Scene 0 is the main menu
    }

    private PlayerController ChoosePlayerPrefab(int deathCount) {
        if (deathCount < 3) {
            return skinnyPlayerPrefab;
        }
        if (deathCount < 6) {
            return midPlayerPrefab;
        }
        return buffPlayerPrefab;

    }

    private void LoadLevelData()
    {
        if (System.IO.File.Exists(SaveFilePath))
        {
            string json = System.IO.File.ReadAllText(SaveFilePath);
            levelCompletionData = JsonUtility.FromJson<LevelCompletionData>(json);
        }
        else
        {
            // Initialize with default data if no save file exists
            levelCompletionData = new LevelCompletionData
            {
                levelStats = new LevelCompletionStats[4] // We have just 1, but intended to be 4
            };
            for (int i = 0; i < levelCompletionData.levelStats.Length; i++)
            {
                levelCompletionData.levelStats[i] = new LevelCompletionStats();
            }
        }
    }

    private void SaveLevelData()
    {
        string json = JsonUtility.ToJson(levelCompletionData, true);
        System.IO.File.WriteAllText(SaveFilePath, json);
    }

} }