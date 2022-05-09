using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public class PlayerKillChain : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private int _killCount = 0;
        [SerializeField] private float _killChainDuration = 5f;

        [Header("STEP SETTINGS")]
        [SerializeField] private int _maxStep = 250;
        [SerializeField] private List<KillChainData> _killChainDatas = new();

        private float _currentKillChainTimer;

        [System.Serializable]
        private class KillChainData
        {
            public string Name = "New KillChain Data";
            public int Step = 0;
            public float ScoreMultiplierModifier = 0;
        }

        private void OnEnable()
        {
            Actions.OnEnemyDeath += SetKillChain;
        }

        private void OnDisable()
        {
            Actions.OnEnemyDeath -= SetKillChain;
        }

        void Start() => Init();

        void Update() => ProcessKillChainTimer();

        void Init()
        {
            _currentKillChainTimer = _killChainDuration;
        }

        void ProcessKillChainTimer()
        {
            if(_killCount == 0) { return; }

            _currentKillChainTimer -= Time.deltaTime;

            if (_currentKillChainTimer <= 0)
            {
                _currentKillChainTimer = 0;
                ResetKillChain();
            }
        }

        void SetKillChain()
        {
            AddToKillCount();

            // Update kill count
            UpdateKillCount(_killCount);

            // Refresh timer
            RefreshKillChainTimer();

            HandleKillChainStep();
        }

        void ResetKillChain()
        {
            RefreshKillChainTimer();
            ResetKillCount();
            GameManager.Instance.ResetScoreMultiplierValue();
        }

        void RefreshKillChainTimer()
        {
            _currentKillChainTimer = _killChainDuration;
        }

        void HandleKillChainStep()
        {
            if (_killCount >= _maxStep) { return; }

            for (int i = 0; i < _killChainDatas.Count; i++)
            {
                if (_killCount == _killChainDatas[i].Step)
                {
                    Debug.Log("Kill count reached this step : " + _killChainDatas[i]);

                    GameManager.Instance.UpdateScoreMultiplierValue(_killChainDatas[i].ScoreMultiplierModifier);

                    Actions.OnKillCountStepReached?.Invoke();
                }
            }
        }

        #region Kill count
        void ResetKillCount()
        {
            _killCount = 0;

            // Update the associated UI
            Actions.OnKillCountValueChanged?.Invoke(_killCount);
        }

        void AddToKillCount()
        {
            _killCount++;
        }

        void UpdateKillCount(int value)
        {
            _killCount = value;

            Debug.Log("KILL COUNT = " + _killCount);

            // Update the associated UI
            Actions.OnKillCountValueChanged?.Invoke(_killCount); 
        }
        #endregion
    }
}