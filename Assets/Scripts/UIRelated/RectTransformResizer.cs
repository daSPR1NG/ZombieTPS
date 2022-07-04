using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Khynan_Coding
{
    public class RectTransformResizer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Vector2 _paddingSize;

        RectTransform _rect;

        private void OnEnable() => MatchTextContentSize();

        void Start() => Init();

        void Init()
        {
            _rect = GetComponent<RectTransform>();
        }

        void MatchTextContentSize()
        {
            if ( !Helper.IsThisGOActive( gameObject ) || !_rect ) return;

            _text.ForceMeshUpdate();

            Vector2 textSize = _text.GetRenderedValues(false);

            _rect.sizeDelta = textSize + _paddingSize;
        }
    }
}