using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Khynan_Coding
{
    public enum GameState
    {
        Play, Pause,
    }

    public class GameManager : MonoBehaviour
    {
        [Header("GENERAL SETTINGS")]
        public GameState GameState = GameState.Play;
        public Transform ActivePlayer = null;

        [Header("WAVES")]
        public int WaveCount;

        [Header("SCORE")]
        public float ScoreMultiplier = 1;

        private PlayerInput _playerInputReference;

        private InputAction _pause;

        #region References
        //
        #endregion

        #region Singleton
        public static GameManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                if (!ActivePlayer)
                {
                    ActivePlayer = GameObject.FindGameObjectWithTag("Player").transform;
                }

                _playerInputReference = ActivePlayer.GetComponent<PlayerInput>();
                _pause = _playerInputReference.actions["Pause"];
            }
        }
        #endregion

        #region OnEnable / OnDisable
        private void OnEnable()
        {
            _pause.performed += context => ToggleGameState();

            Actions.OnWaveEnded += IncrementeWaveCount;
        }

        private void OnDisable()
        {
            _pause.performed -= context => ToggleGameState();

            Actions.OnWaveEnded -= IncrementeWaveCount;
        }
        #endregion

        void Start() => Init();

        private void Init()
        {
            SetGameStateToPlayingMod();
        }

        private void Update()
        {
            // DEBUG
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("DEBUG ADD SCORE");
                _playerInputReference.GetComponent<ScoreManager>().AddValueToGlobalScore(500);
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _playerInputReference.GetComponent<ScoreManager>().RemoveValueToGlobalScore(500);
            }
        }

        #region Game states methods
        private void ToggleGameState()
        {
            if (GameIsPlaying())
            {
                SetGameStateToPause();
            }
            else
            {
                SetGameStateToPlayingMod();
            }

            Actions.OnGameStateChanged?.Invoke();
        }

        public void SetGameStateToPause()
        {
            if (GameState == GameState.Pause) { return; }

            GameState = GameState.Pause;

            Actions.OnGamePaused?.Invoke();

            SetTimeScaleTo(0);
        }

        public void SetGameStateToPlayingMod()
        {
            if (GameState == GameState.Play) { return;  }

            GameState = GameState.Play;
            SetTimeScaleTo(1);
        }

        public bool PlayerCanUseActions()
        {
            if (GameIsPaused())
            {
                return false;
            }

            return true;
        }

        public void QuitTheGame()
        {
            Helper.DebugMessage("QUIT THE GAME !");
            Application.Quit();
        }

        public bool GameIsPaused()
        {
            if (GameState == GameState.Pause)
            {
                return true;
            }

            return false;
        }

        public bool GameIsPlaying()
        {
            if (GameState == GameState.Play)
            {
                return true;
            }

            return false;
        }
        #endregion

        public void SetTimeScaleTo(float newValue)
        {
            Time.timeScale = newValue;
        }

        public void IncrementeWaveCount()
        {
            WaveCount++;

            Actions.OnWaveCountValueChanged?.Invoke(WaveCount);
        }

        #region Score Multiplier
        public void ResetScoreMultiplierValue()
        {
            ScoreMultiplier = 1;
            Actions.OnScoreMultiplierValueChanged?.Invoke(ScoreMultiplier);
        }

        public void UpdateScoreMultiplierValue(float value)
        {
            ScoreMultiplier = value;
            Actions.OnScoreMultiplierValueChanged?.Invoke(ScoreMultiplier);
        }
        #endregion

        //private void OnGUI()
        //{
        //    if (!Application.isEditor) { return; }

        //    GUI.Label(new Rect(5, 15, 75, 25), GameState.ToString());
        //}
    }
}