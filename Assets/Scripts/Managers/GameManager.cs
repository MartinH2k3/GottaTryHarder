using Player;
using UnityEngine;
using SerializableData;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion; // added

namespace Managers
{
public class GameManager: MonoBehaviour
{
    [Header("Player Prefabs")]
    [SerializeField] private Vector3[] playerSpawnPoints;
    [SerializeField] private PlayerController skinnyPlayerPrefab;
    [SerializeField] private PlayerController midPlayerPrefab;
    [SerializeField] private PlayerController buffPlayerPrefab;
    private PlayerController _player;

    [Header("Level Data")]
    [SerializeField] private PlayerLevelUpStats levelUpStatsData;
    private LevelCompletionData _levelCompletionData;
    private string SaveFilePath => Application.persistentDataPath + "/levelData.json";

    private int _currentLevelIndex = 0;
    private Vector3 PlayerSpawnPoint => _currentLevelIndex >= 0 ? playerSpawnPoints[_currentLevelIndex] : Vector3.negativeInfinity; // If it's in the main menu, and anything fucks up so that it tries to spawn the player, it will spawn at negative infinity so nobody sees it

    private float _spawnTimestamp;
    private float _prevLevelsTime;
    public int DeathCount { get; private set; }

    public static GameManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        InitializeLevel();
    }

    private void InitializeLevel() {

        _currentLevelIndex = GetCurrentLevelIndex();

        // Skip Main Menu
        if (_currentLevelIndex < 0) {
            return;
        }

        LoadLevelData();

        _spawnTimestamp = Time.time;
        _prevLevelsTime = GetSpentTime(_currentLevelIndex);

        DeathCount = GetDeathCount(_currentLevelIndex);

        var playerPrefab = ChoosePlayerPrefab(DeathCount);
        _player = Instantiate(playerPrefab, PlayerSpawnPoint, Quaternion.identity);
        LevelUpPlayer(DeathCount);

        _player.OnDeath += HandlePlayerDeath;

        UpdateCameraTarget();
    }

    /// <summary>
    /// Levels up the player based on their death count. Adjusts the base stat class values.
    /// </summary>
    /// <param name="level">Death count of the player</param>
    public void LevelUpPlayer(int level) {
        var statsArray = levelUpStatsData.levelUpStats;
        for (var i = 0; i < Mathf.Min(level, statsArray.Length); i++) {
            var stats = statsArray[i];
            _player.movementStats.walkSpeed *= stats.movementSpeedRateChange;
            _player.jumpStats.extraAirJumps += stats.addsMultiJump;
            _player.jumpStats.jumpStrength *= stats.jumpStrengthRateChange;
            _player.jumpStats.damageTakenOnJump += stats.addsDamageTakenOnJump;
            _player.dashStats.dashSpeed *= stats.dashSpeedRateChange;
            if (stats.attackComboUnlocked) {
                _player.combatStats.comboUnlocked = true;
            }
            if (stats.jumpKickUnlocked) {
                _player.combatStats.jumpKickUnlocked = true;
            }
            _player.combatStats.attackDamage += stats.damageAdded;
            _player.combatStats.lifeSteal += stats.addsLifeSteal;
        }
    }

    /// <summary> Returns death for a chosen level </summary>
    public int GetLevelDeathCount(int level) {
        return _levelCompletionData.levelStats[level].deathCount;
    }

    public float GetTimeSoFar() {
        return _prevLevelsTime + (Time.time - _spawnTimestamp);
    }

    /// <summary> Counts all deaths until chosen level. </summary>
    private int GetDeathCount(int levelIndex = -1) {
        var statsArray = _levelCompletionData.levelStats;

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
        var statsArray = _levelCompletionData.levelStats;

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

    private void LoadLevelData() {
        if (System.IO.File.Exists(SaveFilePath))
        {
            string json = System.IO.File.ReadAllText(SaveFilePath);
            _levelCompletionData = JsonUtility.FromJson<LevelCompletionData>(json);
        }
        else
        {
            // Initialize with default data if no save file exists
            _levelCompletionData = new LevelCompletionData
            {
                levelStats = new LevelCompletionStats[4] // We have just 1, but intended to be 4
            };
            for (int i = 0; i < _levelCompletionData.levelStats.Length; i++)
            {
                _levelCompletionData.levelStats[i] = new LevelCompletionStats();
            }
        }
    }

    private void SaveLevelData() {
        string json = JsonUtility.ToJson(_levelCompletionData, true);
        System.IO.File.WriteAllText(SaveFilePath, json);
    }

    private void HandlePlayerDeath() {
        _player.OnDeath -= HandlePlayerDeath;

        DeathCount++;

        if (_currentLevelIndex >= 0 && _currentLevelIndex < _levelCompletionData.levelStats.Length)
        {
            _levelCompletionData.levelStats[_currentLevelIndex].deathCount++;
            _levelCompletionData.levelStats[_currentLevelIndex].timeSeconds = GetTimeSoFar();
        }

        SaveLevelData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateCameraTarget()
    {
        var followCamera = FindFirstObjectByType<CinemachineCamera>();
        if (followCamera != null)
        {
            followCamera.Follow = _player.transform;
        }
    }
}

}