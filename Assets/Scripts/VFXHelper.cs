using UnityEngine;
using System.Collections.Generic;

namespace Khynan_Coding
{
    public class VFXHelper : MonoBehaviour
    {
        [Header("ON DISABLE SETTINGS")]
        [SerializeField] private bool _doesHideObjects = false;
        [SerializeField] private bool _doesDisplayObjects = false;
        [SerializeField] private float _durationBeforeExecution = .5f;
        [SerializeField] private GameObject[] _gameObjectsToUse;

        private float _currentTimer = 0;

        private void Awake()
        {
            HideAllGameObjectsToUse();
        }

        void OnEnable()
        {
            HideAllGameObjectsToUse();
            _currentTimer = 0;
        }

        void Start() => Init();

        private void LateUpdate() => ProcessTimer();

        void Init()
        {
            //Set all datas that need it at the start of the game
        }

        private void ProcessTimer()
        {
            if (_durationBeforeExecution == 0) { return; }

            _currentTimer += Time.deltaTime;

            if (_currentTimer >= _durationBeforeExecution)
            {
                _currentTimer = _durationBeforeExecution;
                DisplayAllGameObjectsToUse();
            }
        }

        private void DisplayAllGameObjectsToUse()
        {
            if (!_doesDisplayObjects || _gameObjectsToUse.Length == 0) { return; }

            for (int i = 0; i < _gameObjectsToUse.Length; i++)
            {
                _gameObjectsToUse[i].gameObject.SetActive(true);
            }
        }

        private void HideAllGameObjectsToUse()
        {
            if (!_doesHideObjects || _gameObjectsToUse.Length == 0) { return; }

            for (int i = 0; i < _gameObjectsToUse.Length; i++)
            {
                _gameObjectsToUse[i].gameObject.SetActive(false);
            }
        }
    }
}