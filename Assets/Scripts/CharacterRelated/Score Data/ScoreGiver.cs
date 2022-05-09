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