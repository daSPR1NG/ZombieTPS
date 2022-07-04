using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public enum StatAttribute
    {
        Unassigned,
        MovementSpeed,
        Health,
        AttackSpeed, AttackDamage,
        ElementalDamage,
        ReloadSpeed, FireRate, MaxAmmoBonus, MaxAmmoPerMag, AmmoFiredPerShotBonus,
        CriticalChance, CriticalDamage,
    }

    [System.Serializable]
    public class Stat
    {
        //[Header("SETUP")]
        [SerializeField] private string _name = "[TYPE HERE]";
        [SerializeField] private StatAttribute _attribute = StatAttribute.Unassigned;

        //[Space(2), Header("VALUES")]
        [SerializeField] private bool _needsToMatchBaseValueAtStart = true;
        [SerializeField, Min(0)] private float _baseValue = 0;
        [SerializeField, Min(0)] private float _maxValue = 0;
        public float _currentValue = 0;

        //[Space(2), Header("LIMITS")]
        [SerializeField, Min(0)] private float _minLimit = 0;
        [SerializeField, Min(0)] private float _maxLimit = 0;

        //[Space(2), Header("STATS")]
        [SerializeField] private List<StatModifier> _statModifiers = new();

        //[Space(2), Header("CONDITIONNAL SETTINGS")]
        [SerializeField, Range(0, 100)] private float _criticalThresholdValue = 30;

        public void SetStatName(string stringValue)
        {
            if (_name == stringValue) { return; }

            _name = stringValue;
        }

        public void MatchCurrentValueWithBaseValue()
        {
            if (_currentValue == _baseValue || !_needsToMatchBaseValueAtStart) { return; }

            _currentValue = _baseValue >= _maxLimit && _maxLimit > 0 ? _currentValue = _maxLimit : _currentValue = _baseValue;
            _maxValue = _baseValue >= _maxLimit && _maxLimit > 0 ? _maxValue = _maxLimit : _maxValue = _baseValue;
        }

        public float CalculateCurrentValue()
        {
            float calculatedValue = _baseValue;

            // No modifier at all, return the value here.
            if (_statModifiers.Count == 0)
            {
                return calculatedValue;
            }

            // Modifiers are present, add them then return the correct value.
            for (int i = _statModifiers.Count - 1; i >= 0; i--)
            {
                StatModifier modifier = _statModifiers[i];

                switch (_statModifiers[i].ModifierType)
                {
                    case ModifierType.Flat:
                        calculatedValue += modifier.ModifierValue;
                        break;
                    case ModifierType.Percentage:
                        // Stat + Stat * Bonus
                        calculatedValue += calculatedValue * (modifier.ModifierValue / 100);
                        break;
                }
            }

            if (calculatedValue <= _minLimit)
            {
                return _minLimit;
            }

            //Set the max value corresponding to : Base value (+ modifiers)(if present)
            _maxValue = calculatedValue;

            return calculatedValue;
        }

        #region Stat modifiers methods
        public void AddModifier(StatModifier modifier)
        {
            _statModifiers.Add(modifier);

            //Recalculate the value after adding a modifier
            _currentValue = CalculateCurrentValue();
        }

        public void RemoveSourceModifier(object source)
        {
            if (_statModifiers.Count == 0)
            {
                Debug.LogError("This stat " + _name + " has no stat modifiers.");
            }

            for (int i = 0; i < _statModifiers.Count; i++)
            {
                if (_statModifiers[i].ModifierSource != source) { continue; }

                Debug.Log("Remove stat modifier " + _statModifiers[i].ModifiedStatType.ToString() + " from " + _statModifiers[i].ModifierSource);
                _statModifiers.RemoveAt(i);

                //Recalculate the value after removing all modifiers from a source
                _currentValue = CalculateCurrentValue();
                _maxValue = CalculateCurrentValue();
            }
        }
        #endregion

        #region Getter
        public StatAttribute GetAttribute() { return _attribute; }
        public float GetBaseValue() { return _baseValue; }
        public float GetCurrentValue() 
        {
            _currentValue = Mathf.Clamp(_currentValue, 0, _maxValue);

            return _currentValue; 
        }
        public float GetMaxValue()
        {
            _maxValue = _maxLimit > 0 ?
                _maxValue = Mathf.Clamp(_maxValue, _minLimit, _maxLimit) : _maxValue = Mathf.Clamp(_maxValue, _minLimit, _maxValue);

            return _maxValue;
        }
        public float GetCriticalThreshold() { return _criticalThresholdValue; }
        #endregion

        #region Setter
        public void SetCurrentValue(float value)
        {
            _currentValue = value;
            _currentValue = Mathf.Clamp(_currentValue, 0, _maxValue);
        }
        public void SetMaxValue(float value)
        {
            _maxValue = value;
            _maxValue = _maxLimit > 0 ?
                _maxValue = Mathf.Clamp(_maxValue, _minLimit, _maxLimit) : _maxValue = Mathf.Clamp(_maxValue, _minLimit, _maxValue);
        }
        #endregion
    }
}