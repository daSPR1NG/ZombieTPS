using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Khynan_Coding
{
    [DisallowMultipleComponent]
    public class GridLayoutHelper : MonoBehaviour
    {
        public bool _isFixed = false;
        public float _spacing = 5;

        private void SetChildSize()
        {
            if (transform.childCount == 0 || _isFixed) { return; }

            float xSize = GetRectTransformDeltaSize().x / transform.childCount;
            float ySize = GetRectTransformDeltaSize().y /*/ transform.childCount*/;

            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform rT = transform.GetChild(i).GetComponent<RectTransform>();
                rT.sizeDelta = new Vector2(xSize, ySize);

                Debug.Log(rT.gameObject.name + " " + rT.sizeDelta);
            }

            _isFixed = true;
        }

        private Vector2 GetRectTransformDeltaSize()
        {
            RectTransform contrainerRT = GetComponent<RectTransform>();
            LayoutGroup layoutGroup = GetComponent<LayoutGroup>();

            Vector2 size = contrainerRT.sizeDelta;
            size.x -= (layoutGroup.padding.left + layoutGroup.padding.right);
            size.y -= (layoutGroup.padding.top + layoutGroup.padding.bottom);

            return size;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetChildSize();

            if (Application.isPlaying) { return; }

            UnityEditor.SceneView.RepaintAll();
        }
    }
#endif
}