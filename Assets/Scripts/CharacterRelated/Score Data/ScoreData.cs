using UnityEngine;

namespace Khynan_Coding
{
    public enum ScoreDataType
    {
        Unassigned, OnHit, OnDeath, //etc...
    }

    [System.Serializable]
    public class ScoreData
    {
        [SerializeField] private ScoreDataType type;
        [SerializeField] private int value;

        public ScoreDataType Type { get => type; }
        public int Value { get => value; private set => this.value = value; }

        public void SetValue(int value)
        {
            Value = value;
        }
    }
}