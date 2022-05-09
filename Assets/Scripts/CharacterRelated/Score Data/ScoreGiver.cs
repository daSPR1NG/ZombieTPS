using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public class ScoreGiver : MonoBehaviour
    {
        [SerializeField] private List<ScoreData> scoreDatas = new();

        #region Public References
        public List<ScoreData> ScoreDatas { get => scoreDatas; }
        #endregion

        public void GiveScoreToTarget(Transform target, ScoreData scoreData)
        {
            int scoreValue = 0;

            ScoreManager scoreManager = target.GetComponent<ScoreManager>();

            scoreValue = Mathf.CeilToInt(scoreData.Value * GameManager.Instance.ScoreMultiplier);
            scoreManager.AddValueToGlobalScore(scoreValue);

            Debug.Log("Score added : " + scoreValue);

            // Call event that sends message to UI handling the score
            Actions.OnAddingScore?.Invoke(scoreValue);
        }

        public ScoreData GetScoreData(ScoreDataType scoreDataType)
        {
            if (scoreDatas.Count == 0)
            {
                Debug.LogError("Score data list is empty.");
                return null;
            }

            for (int i = 0; i < scoreDatas.Count; i++)
            {
                if (scoreDatas[i].Type != scoreDataType) { continue; }

                return scoreDatas[i];
            }

            return null;
        }
    }
}