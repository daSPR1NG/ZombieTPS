using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public class DetectionDamageZone : MonoBehaviour
    {
        [SerializeField] private string comparedTag;
        [SerializeField] private List<CharacterType> comparedCharacterTypes = new();

        public List<Transform> transformFound = new();

        #region Public references
        public List<CharacterType> ComparedCharacterTypes { get => comparedCharacterTypes; }
        #endregion

        protected virtual void Start() => InvokeRepeating(nameof(RemoveANonValidTransfromFromTheList), 1, 0.2f);

        #region Trigger events - Enter, Exit
        private void OnTriggerEnter(Collider other)
        {
            AddFoundTransform(other.transform);
        }

        private void OnTriggerExit(Collider other)
        {
            RemoveATransformFromTheList(other.transform);
        }
        #endregion

        #region Add method - OnTriggerEnter
        protected virtual void AddFoundTransform(Transform other)
        {
            GlobalCharacterParameters globalCharacterParameters = other.GetComponent<GlobalCharacterParameters>();

            bool isCorrectType =
                globalCharacterParameters && OtherTypeIsCorrect(globalCharacterParameters.CharacterType) || other.gameObject.CompareTag(comparedTag);

            Debug.Log("Other found : " + other.name + " is type : " + isCorrectType);

            if (transformFound.Contains(other.transform) || !isCorrectType) { return; }

            transformFound.Add(other.transform);
        }
        #endregion

        #region Remove methods - OnTriggerExit & Dead Flag
        public virtual void RemoveATransformFromTheList(Transform other)
        {
            if (!transformFound.Contains(other.transform)) { return; }

            transformFound.Remove(other.transform);
        }

        private void RemoveANonValidTransfromFromTheList()
        {
            if (transformFound.Count == 0) { return; }

            Debug.Log("Trigger stay event in DefaultDetectionZone");

            for (int i = 0; i < transformFound.Count; i++)
            {
                if (transformFound[i].Equals(null))
                {
                    transformFound.RemoveAt(i);
                }
            }
        }
        #endregion

        private bool OtherTypeIsCorrect(CharacterType otherType)
        {
            if (ComparedCharacterTypes.Count == 0 || otherType == CharacterType.Unassigned)
            {
                Debug.LogError("The list ComparedCharacterTypes is empty or other type is incorrect (" + otherType + ").", transform);
                return false;
            }

            for (int i = 0; i < ComparedCharacterTypes.Count; i++)
            {
                if (ComparedCharacterTypes[i] == otherType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}