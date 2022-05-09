using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    [System.Serializable]
    public class Stat
    {
        [Header("SETUP")]
        [SerializeField] private string m_Name = "[TYPE HERE]";
        [SerializeField] private StatType type = StatType.Unassigned;

        [Space][Header("VALUES")]
        [SerializeField] private bool needsToMatchBaseValueAtStart = true;
        [SerializeField] private float baseValue = 0;
        [SerializeField] private float maxValue = 0;
        [SerializeField] private float minMaxValue = 0;
        [SerializeField] private List<StatModifier> statModifiers = new();
        public float currentValue = 0;

        [Space][Header("CONDITIONNAL SETTINGS")]
        [Range(0, 100)] [SerializeField] private float criticalThresholdValue = 30;

        public string Name { get => m_Name; private set => m_Name = value; }
        public StatType Type { get => type; }

        #region Public references
        public float BaseValue { get => baseValue; }
        public float CurrentValue { get => currentValue; set => currentValue = Mathf.Clamp(value, 0, MaxValue); }
        public float MaxValue { get => maxValue; set => maxValue = Mathf.Clamp(value, minMaxValue, value); }
        public float CriticalThresholdValue { get => criticalThresholdValue; }
        #endregion

        public void SetStatName(string stringValue)
        {
            if (m_Name == stringValue) { return; }

            m_Name = stringValue;
        }

        public void MatchCurrentValueWithBaseValue()
        {
            if (CurrentValue == BaseValue || !needsToMatchBaseValueAtStart) { return; }

            MaxValue = BaseValue;
            CurrentValue = BaseValue;
        }

        public float CalculateCurrentValue()
        {
            float value = BaseValue;

            //No modifier at all return the value
            if (statModifiers.Count == 0)
            {
                return value;
            }

            //Modifiers are present, add them then return the correct value
            for (int i = statModifiers.Count - 1; i >= 0; i--)
            {
                StatModifier modifier = statModifiers[i];

                switch (statModifiers[i].ModifierType)
                {
                    case ModifierType.Flat:
                        value += modifier.ModifierValue;
                        break;
                    case ModifierType.Percentage:
                        value += 1 * (modifier.ModifierValue / 100);
                        break;
                }
            }

            if (value <= minMaxValue)
            {
                return minMaxValue;
            }

            //Set the max value corresponding to : Base value (+ modifiers)(if present)
            MaxValue = value;

            return value;
        }

        #region Stat modifiers methods
        public void AddModifier(StatModifier modifier)
        {
            statModifiers.Add(modifier);

            //Recalculate the value after adding a modifier
            CurrentValue = CalculateCurrentValue();
        }

        public void RemoveSourceModifier(object source)
        {
            if (statModifiers.Count == 0)
            {
                Debug.LogError("This stat " + m_Name + " has no stat modifiers.");
            }

            for (int i = 0; i < statModifiers.Count; i++)
            {
                if (statModifiers[i].ModifierSource != source) { continue; }

                Debug.Log("Remove stat modifier " + statModifiers[i].ModifiedStatType.ToString() + " from " + statModifiers[i].ModifierSource);
                statModifiers.RemoveAt(i);

                //Recalculate the value after removing all modifiers from a source
                CurrentValue = CalculateCurrentValue();
                MaxValue = CurrentValue;
            }
        }
        #endregion
    }
}