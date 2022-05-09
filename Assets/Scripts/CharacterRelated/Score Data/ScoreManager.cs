using UnityEngine;

namespace Khynan_Coding
{
    public class ScoreManager : MonoBehaviour
    {
        public int GlobalScore;

        private void OnEnable()
        {
            Actions.OnShootingEnemyAddScore += AddValueToGlobalScore;
            Actions.OnKillingEnnemyAddScore += AddValueToGlobalScore;
        }

        private void OnDisable()
        {
            Actions.OnShootingEnemyAddScore -= AddValueToGlobalScore;
            Actions.OnKillingEnnemyAddScore -= AddValueToGlobalScore;
        }

        void Start() => Init();

        void Init()
        {
            GlobalScore = 0;
            Actions.OnGlobalScoreValueChanged?.Invoke(GlobalScore);
        }

        public void AddValueToGlobalScore(int value)
        {
            GlobalScore += value;
            Debug.Log("Score added : " + value);

            Actions.OnGlobalScoreValueChanged?.Invoke(GlobalScore);
        }

        public void RemoveValueToGlobalScore(int value)
        {
            GlobalScore -= value;
            Debug.Log("Score removed : " + value);

            Actions.OnGlobalScoreValueChanged?.Invoke(GlobalScore);
        }
    }
}