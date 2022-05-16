using UnityEngine;

namespace Khynan_Coding
{
    public enum ScoreRelatedActionName
    {
        Unassigned, OnHit, OnDeath, //etc...
    }

    [System.Serializable]
    public class ScoreData
    {
        [SerializeField] private string _name;
        [SerializeField] private ScoreRelatedActionName _scoreRelatedActionName;
        [SerializeField] private int value;

        public ScoreRelatedActionName ScoreRelatedActionName { get => _scoreRelatedActionName; private set => _scoreRelatedActionName = value; }
        public int Value { get => value; private set => this.value = value; }

        public void SetValue(int value)
        {
            Value = value;
        }

        public ScoreData(ScoreRelatedActionName scoreRelatedActionName, int value)
        {
            ScoreRelatedActionName = scoreRelatedActionName;
            Value = value;
        }

        public ScoreData(int value) : this(ScoreRelatedActionName.OnHit, value) { }
    }
}