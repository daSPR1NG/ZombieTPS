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
            int scoreValue;

            scoreValue = Mathf.CeilToInt(scoreData.Value * GameManager.Instance.ScoreMultiplier);

            //Debug.Log("Score added : " + scoreValue);

            // Call event that sends message to UI handling the score
            Actions.OnAddingScore?.Invoke(scoreValue);
            Actions.OnScoreNotificationNeed?.Invoke(scoreData);
        }

        public ScoreData GetScoreData(ScoreRelatedActionName scoreDataType)
        {
            if (scoreDatas.Count == 0)
            {
                Debug.LogError("Score data list is empty.");
                return null;
            }

            for (int i = 0; i < scoreDatas.Count; i++)
            {
                if (scoreDatas[i].ScoreRelatedActionName != scoreDataType) { continue; }

                return scoreDatas[i];
            }

            return null;
        }
    }
}