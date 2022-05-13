using UnityEngine;

namespace Khynan_Coding
{
    public class ScoreManager : MonoBehaviour
    {
        public int GlobalScore;

        private void OnEnable()
        {
            Actions.OnAddingScore += AddValueToGlobalScore;
            Actions.OnRemovingScore += RemoveValueToGlobalScore;
        }

        private void OnDisable()
        {
            Actions.OnAddingScore -= AddValueToGlobalScore;
            Actions.OnRemovingScore -= RemoveValueToGlobalScore;
        }

        void Start() => Init();

        void Init()
        {
            GlobalScore = 0;
            Actions.OnGlobalScoreValueChanged?.Invoke(GlobalScore, false);
        }

        public void AddValueToGlobalScore(int value)
        {
            GlobalScore += value;
            Debug.Log("Score added : " + value);

            Actions.OnGlobalScoreValueChanged?.Invoke(GlobalScore, true);
        }

        public void RemoveValueToGlobalScore(int value)
        {
            GlobalScore -= value;
            Debug.Log("Score removed : " + value);

            Actions.OnGlobalScoreValueChanged?.Invoke(GlobalScore, false);
        }
    }
}