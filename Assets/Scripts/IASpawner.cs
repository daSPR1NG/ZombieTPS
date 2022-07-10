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

    public class IASpawner : Spawner
    {
        [Header( "DEPENDENCIES" )]
        public bool IsAbleToSpawn = true;
        public float HealthModifier = 15;
        public float MovementSpeedModifier = .5f;
        public float AttackSpeedModifier = .25f;
        public float AttackDamageModifier = 5;
        [SerializeField] private SpawnerState _spawnerState = SpawnerState.Unassigned;

        [Header("TIMER SETTINGS")]
        [SerializeField] private float _startTimerBeforeSpawning = 1.5f;
        [SerializeField] private float _timerBeforeSpawningOnWaveEnd = 1.5f;
        [SerializeField] private float _delayBetweenSpawns = 1.5f;

        [Space]
        [Header("UNIT SETTINGS")]
        [SerializeField] private int _unitLimit = 5;
        [SerializeField] private int _unitAmountToAddPerWave = 2;
        [SerializeField] private int _spawnUnitTypeChangeRate = 3;
        private int _currentSpawnUnitTypeChangeRate = 0;
        public int ActiveAudios = 0;
        public List<Transform> SpawnedEnemies = new();

        private int _aliveUnit = 0;
        private int _whatToSpawnIndex = 0;
        private Coroutine _spawnCoroutine;

        [Space( 5 )]
        [Header( "SPAWNER PLACES SETTINGS" )]
        public bool IsSpawnBasedOnDistanceFromPlayer = true;
        public int SpawnerIndex = 0;
        public List<DistanceFromPlayerData> DistanceFromPlayerDatas = new();

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
            Actions.OnPlayerDeath += KillEveryAliveUnits;
            Actions.OnEnemyDeath += RemoveOneAliveUnit;
        }

        private void OnDisable()
        {
            Actions.OnPlayerDeath -= KillEveryAliveUnits;
            Actions.OnEnemyDeath -= RemoveOneAliveUnit;
        }
        #endregion

        private void Awake() => _spawnerState = SpawnerState.WaitingToStart;

        void Start() => Init();

        void Init()
        {
            _currentSpawnUnitTypeChangeRate = 1;

            CreateDistanceDataReferences();
            StartCoroutine(HandleStartingWave());
        }

        private void LateUpdate() => SpawnSomething();

        protected override void SpawnSomething()
        {
            if (!NeedToSpawnSomething()) return;

            CalculateDistanceFromAPlayer();

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
                Transform spawnLocation = IsSpawnBasedOnDistanceFromPlayer ? 
                    GetClosestSpawnPointFromPlayer().Transform : SpawnPoints [ Random.Range( 0, SpawnPoints.Count ) ];

                // Actually spawn something and add one more alive unit
                GameObject enemyInstance = Instantiate(
                    WhatToSpawn[_whatToSpawnIndex].Prefab,
                    spawnLocation.position,
                    spawnLocation.rotation);

                if ( GameManager.Instance.WaveCount > 1 ) AugmentUnitPower( enemyInstance.transform );

                AddOneAliveUnit( enemyInstance.transform );

                // Reset the value if this value exceeds the list count, it prevents any error
                if ( _whatToSpawnIndex >= 1 ) _whatToSpawnIndex = 0;

                AudioSource audioSource = enemyInstance.GetComponent<AudioSource>();
                if ( ActiveAudios < 3 )
                {
                    audioSource.enabled = true;
                    ActiveAudios++;
                }

                _currentSpawnUnitTypeChangeRate++;

                // If there is more than one type of object to spawn increment the value, else always return 0
                if ( WhatToSpawn.Count > 1 && _currentSpawnUnitTypeChangeRate >= _spawnUnitTypeChangeRate )
                {
                    _currentSpawnUnitTypeChangeRate = 0;
                    _whatToSpawnIndex++;
                }

                StatsManager playerStats = GameManager.Instance.ActivePlayer.GetComponent<StatsManager>();
                if ( playerStats.IsCharacterDead() ) Actions.OnPlayerDeath?.Invoke();

                yield return delayBetweenSpawn;
            }

            SetSpawnerStateToWaitingForAction();
        }

        private void AddOneAliveUnit( Transform unit )
        {
            _aliveUnit++;
            SpawnedEnemies.Add( unit );

            //SpawnerIndex++;
            //if ( SpawnerIndex >= SpawnPoints.Count ) SpawnerIndex = 0;
        }
        
        private void RemoveOneAliveUnit( Transform unit )
        {
            _aliveUnit--;

            if ( unit.GetComponent<AudioSource>().enabled ) ActiveAudios--;

            SpawnedEnemies.Remove( unit );

            ManageUnitsAudio();

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

            GameManager.Instance.IncrementeWaveCount();
        }

        private bool NeedToSpawnSomething()
        {
            if (_spawnerState != SpawnerState.NeedToSpawn || !IsAbleToSpawn ) { return false; }

            return true;
        }

        private void ManageUnitsAudio()
        {
            if ( SpawnedEnemies.Count == 0 || _aliveUnit == 0 || ActiveAudios >= 3 ) { return; }

            for ( int i = SpawnedEnemies.Count - 1; i >= 0; i-- )
            {
                if ( ActiveAudios >= 3 ) { return; }

                AudioSource audioSource = SpawnedEnemies [ i ].GetComponent<AudioSource>();

                if ( audioSource.enabled ) { continue; }

                audioSource.enabled = true;
                ActiveAudios++;
            }
        }

        private void KillEveryAliveUnits()
        {
            IsAbleToSpawn = false;

            if ( SpawnedEnemies.Count == 0 ) { return; }

            for ( int i = SpawnedEnemies.Count - 1; i >= 0; i-- )
            {
                AudioSource audioSource = SpawnedEnemies [ i ].GetComponent<AudioSource>();

                if ( !audioSource.enabled ) { continue; }

                audioSource.enabled = false;
            }
        }

        private void AugmentUnitPower( Transform unit )
        {
            StatsManager unitStats = unit.GetComponent<StatsManager>();
            CombatSystem unitCombatSystem = unit.GetComponent<CombatSystem>();

            float localHealthModifier = unitCombatSystem.IsRanged ? HealthModifier / 2 : HealthModifier;
            float localMovementSpeedModifier = unitCombatSystem.IsRanged ? MovementSpeedModifier / 2 : MovementSpeedModifier;
            float localAttackSpeedModifier = unitCombatSystem.IsRanged ? AttackSpeedModifier / 2 : AttackSpeedModifier;
            float localAttackDamageModifier = unitCombatSystem.IsRanged ? AttackDamageModifier / 2 : AttackDamageModifier;

            StatModifier healthStatsModifier = new ( ModifierType.Flat, localHealthModifier, this, StatAttribute.Health );
            StatModifier movementSpeedModifier = new ( ModifierType.Flat, localMovementSpeedModifier, this, StatAttribute.MovementSpeed );
            StatModifier attackSpeedModifier = new ( ModifierType.Flat, localAttackSpeedModifier, this, StatAttribute.AttackSpeed );
            StatModifier attackDamageModifier = new ( ModifierType.Flat, localAttackDamageModifier, this, StatAttribute.AttackDamage );

            for ( int i = 0; i < Mathf.CeilToInt(GameManager.Instance.WaveCount / 2); i++ )
            {
                // Health
                unitStats.GetStat( StatAttribute.Health ).AddModifier( healthStatsModifier );

                // MoveSpeed
                unitStats.AugmentMovementSpeed( movementSpeedModifier );

                // AttackSpeed
                unitStats.GetStat( StatAttribute.AttackSpeed ).AddModifier( attackSpeedModifier );

                // AttackDamage
                unitStats.GetStat( StatAttribute.AttackDamage ).AddModifier( attackDamageModifier );
            }
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
            if ( !IsSpawnBasedOnDistanceFromPlayer ) { return; }

            for (int i = 0; i < SpawnPoints.Count; i++)
            {
                float distance = Vector3.Distance(DistanceFromPlayerDatas[i].Transform.position, DistanceFromPlayerDatas[i].PlayerTransform.position);

                DistanceFromPlayerDatas[i].Distance = distance;
            }
        }
        #endregion
    }
}