using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Khynan_Coding
{
    public enum SpawnerState
    {
        Unassigned, WaitingToStart, WaitingForAction, NeedToSpawn, Processing, 
    }

    [System.Serializable]
    public class SpawnDifficultySetting
    {
        // Put here all the data used per difficulty / Easy, Medium, Hard /
        // - unit to add
        // - difficulty
        // - ...
    }

    public class IASpawner : Spawner
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private SpawnerState _spawnerState = SpawnerState.Unassigned;

        [Header("TIMER SETTINGS")]
        [SerializeField] private float _startTimerBeforeSpawning = 1.5f;
        [SerializeField] private float _timerBeforeSpawningOnWaveEnd = 1.5f;
        [SerializeField] private float _delayBetweenSpawns = 1.5f;

        [Space]
        [Header("UNIT SETTINGS")]
        [SerializeField] private int _unitLimit = 5;
        [SerializeField] private int _unitAmountToAddPerWave = 2;

        private int _aliveUnit = 0;
        private int _whatToSpawnIndex = 0;
        private Coroutine _spawnCoroutine;

        public List<DistanceFromPlayerData> DistanceFromPlayerDatas = new();

        #region Public References

        #endregion

        [System.Serializable]
        public class DistanceFromPlayerData
        {
            public Transform PlayerTransform;
            public Transform Transform;
            public float Distance;

            public DistanceFromPlayerData(Transform playerTransform, Transform transform, float distance)
            {
                PlayerTransform = playerTransform;
                Transform = transform;
                Distance = distance;
            }
        }

        #region OnEnable / OnDisable
        private void OnEnable()
        {
            Actions.OnEnemyDeath += RemoveOneAliveUnit;
        }

        private void OnDisable()
        {
            Actions.OnEnemyDeath -= RemoveOneAliveUnit;
        }
        #endregion

        private void Awake() => _spawnerState = SpawnerState.WaitingToStart;

        void Start() => Init();

        void Init()
        {
            CreateDistanceDataReferences();
            StartCoroutine(HandleStartingWave());
        }

        private void LateUpdate() => SpawnSomething();

        protected override void SpawnSomething()
        {
            if (!NeedToSpawnSomething()) { return; }

            CalculateDistanceFromAPlayer();

            // If there is more than one type of object to spawn increment the value, else always return 0
            _whatToSpawnIndex = WhatToSpawn.Count > 1 ? _whatToSpawnIndex++ : 0;

            // Reset the value if this value exceeds the list count, it prevents any error
            if (_whatToSpawnIndex >= WhatToSpawn.Count) { _whatToSpawnIndex = 0; }

            _spawnCoroutine = StartCoroutine(CreateAnEnemyInstance());
        }

        private IEnumerator CreateAnEnemyInstance()
        {
            _spawnerState = SpawnerState.Processing;

            WaitForSeconds delayBetweenSpawn = new (_delayBetweenSpawns);

            // When this method is called it means that a wave has begun
            Actions.OnWaveBegun?.Invoke();

            for (int i = 0; i < _unitLimit; i++)
            {
                // Actually spawn something and add one more alive unit
                Instantiate(
                    WhatToSpawn[_whatToSpawnIndex].Prefab,
                    GetClosestSpawnPointFromPlayer().Transform.position,
                    GetClosestSpawnPointFromPlayer().Transform.rotation);

                AddOneAliveUnit();

                yield return delayBetweenSpawn;
            }

            SetSpawnerStateToWaitingForAction();
        }

        private void AddOneAliveUnit()
        {
            _aliveUnit++;
        }
        
        private void RemoveOneAliveUnit()
        {
            _aliveUnit--;

            if (_aliveUnit <= 0 && _spawnerState == SpawnerState.WaitingForAction) {
                _aliveUnit = 0;
                StartCoroutine(HandleEndOfWave()); 
            }
        }

        private void SetSpawnerStateToWaitingForAction()
        {
            // When every unit is spawned put the spawner into waiting state
            _spawnerState = SpawnerState.WaitingForAction;

            if (_spawnCoroutine is not null) { StopCoroutine(_spawnCoroutine); }

            if (_aliveUnit <= 0 && _spawnerState == SpawnerState.WaitingForAction) { StartCoroutine(HandleEndOfWave()); }
        }

        private IEnumerator HandleEndOfWave()
        {
            // Send the signal that the wave has ended
            Actions.OnWaveEnded?.Invoke();

            // Wait for t
            yield return new WaitForSeconds(_timerBeforeSpawningOnWaveEnd);

            // Add more unit to the limit
            _unitLimit += _unitAmountToAddPerWave;

            // Reset spawner state to make units respawn again
            _spawnerState = SpawnerState.NeedToSpawn;
        }

        private IEnumerator HandleStartingWave()
        {
            // Wait for t
            yield return new WaitForSeconds(_startTimerBeforeSpawning);

            // Set spawner state to make units spawn
            _spawnerState = SpawnerState.NeedToSpawn;
        }

        private bool NeedToSpawnSomething()
        {
            if (_spawnerState != SpawnerState.NeedToSpawn) { return false; }

            return true;
        }

        #region Distance handler
        private DistanceFromPlayerData GetClosestSpawnPointFromPlayer()
        {
            float minDist = DistanceFromPlayerDatas.Min(t => t.Distance);

            for (int i = 0; i < DistanceFromPlayerDatas.Count; i++)
            {
                if (DistanceFromPlayerDatas[i].Distance == minDist)
                {
                    return DistanceFromPlayerDatas[i];
                }
            }

            return DistanceFromPlayerDatas[0];
        }

        private void CreateDistanceDataReferences()
        {
            for (int i = 0; i < SpawnPoints.Count; i++)
            {
                DistanceFromPlayerDatas.Add(new DistanceFromPlayerData(
                    GameManager.Instance.ActivePlayer, 
                    SpawnPoints[i],
                    Vector3.Distance(GameManager.Instance.ActivePlayer.position, SpawnPoints[i].position)));
            }
        }

        private void CalculateDistanceFromAPlayer()
        {
            for (int i = 0; i < SpawnPoints.Count; i++)
            {
                float distance = Vector3.Distance(DistanceFromPlayerDatas[i].Transform.position, DistanceFromPlayerDatas[i].PlayerTransform.position);

                DistanceFromPlayerDatas[i].Distance = distance;
            }
        }
        #endregion
    }
}