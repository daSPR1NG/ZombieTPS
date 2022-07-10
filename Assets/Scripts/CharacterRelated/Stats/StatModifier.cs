using UnityEngine;

namespace Khynan_Coding
{
    public enum ModifierType
    {
        Unassigned, Flat, Percentage,
    }

    [System.Serializable]
    public class StatModifier
    {
        [SerializeField] private float modifierValue;
        [SerializeField] private StatAttribute modifiedStatType;
        [SerializeField] private ModifierType modifierType;
        public object modifierSource;

        public StatModifier(ModifierType modifierType, float modifierValue, object modifierSource, StatAttribute modifiedStatType)
        {
            ModifierValue = modifierValue;
            ModifierType = modifierType;
            ModifierSource = modifierSource;
            ModifiedStatType = modifiedStatType;
        }

        public object ModifierSource { get => modifierSource; private set => modifierSource = value; }
        public ModifierType ModifierType { get => modifierType; private set => modifierType = value; }
        public float ModifierValue { get => modifierValue; set => modifierValue = value; }
        public StatAttribute ModifiedStatType { get => modifiedStatType; private set => modifiedStatType = value; }
    }
}