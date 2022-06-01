using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    public class HealthBarSeparatorsHandler : MonoBehaviour
    {
        [Header("SEPARATOR SETTINGS")]
        [SerializeField] private bool _useSeparators = true;
        [SerializeField] private int _separatorToSpawn = 0;
        [SerializeField] private GameObject _separatorIconPf;
        [SerializeField, Range(2,30)] private float _width = 2;

        void Start() => Init();

        void Init()
        {
            CreateSeparators();
        }

        private void CreateSeparators()
        {
            if (!_useSeparators || !_separatorIconPf) { return; }

            for (int i = 0; i < _separatorToSpawn; i++)
            {
                GameObject instance = Instantiate(_separatorIconPf, transform);
                //instance.transform.SetParent(transform);
            }

            ChangeSeparatorWidth(_width);
        }

        private void ChangeSeparatorWidth(float width)
        {
            //Debug.Log("ChangeSeparatorWidth");

            if (transform.childCount == 0) { return; }

            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform rectTransform = transform.GetChild(i).GetComponent<RectTransform>();
                Vector2 newSizeDelta = new(width, 0);

                if (rectTransform.sizeDelta == newSizeDelta) { continue; }

                rectTransform.sizeDelta = newSizeDelta;
            }
        }

        private void OnValidate()
        {
            ChangeSeparatorWidth(_width);
        }
    }
}