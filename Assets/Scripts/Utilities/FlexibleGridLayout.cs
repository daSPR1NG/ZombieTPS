using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    // NOTES : Lack of row/column constraint, starting anchor parameter is not supported yet as the child alignement that is not affecting children.

    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            Unassigned, Uniform, Width, Height, 
        }

        [Space(5), Header("SETTINGS")]
        [SerializeField] private FitType _fitType = FitType.Unassigned;
        [SerializeField, Min(0)] private Vector2 _spacing;

        [Min(1)] private int _rows;
        [Min(1)] private int _columns;
        private Vector2 _cellSize;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            SetCellsSize();
            PositionChildrenByAxis();
        }

        public override void CalculateLayoutInputVertical() { }

        public override void SetLayoutHorizontal() { }

        public override void SetLayoutVertical() { }

        private void SetCellsSize()
        {
            float cellWidth = (GetContainerSpace().x / (float)GetColumns(transform.childCount))

                // SPACING OFFSET
                - (_spacing.x / (float)GetColumns(transform.childCount) * ((float)GetColumns(transform.childCount) - 1))

                // PADDING OFFSET
                - (padding.left / (float)GetColumns(transform.childCount))
                - (padding.right / (float)GetColumns(transform.childCount));

            float cellHeight = (GetContainerSpace().y / (float)GetRows(transform.childCount))

                // SPACING OFFSET
                - (_spacing.y / (float)GetRows(transform.childCount) * ((float)GetRows(transform.childCount) - 1))

                // PADDING OFFSET
                - (padding.top / (float)GetRows(transform.childCount)) 
                - (padding.bottom / (float)GetRows(transform.childCount));

            _cellSize.x = cellWidth;
            _cellSize.y = cellHeight;
        }

        private void PositionChildrenByAxis()
        {
            int rowCount = 0;
            int columnCount = 0;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                rowCount = i / GetColumns(rectChildren.Count);
                columnCount = i % GetColumns(rectChildren.Count);

                RectTransform rectTransform = rectChildren[i];

                float xPos = (_cellSize.x * columnCount) + (_spacing.x * columnCount) + padding.left;
                float yPos = (_cellSize.y * rowCount) + (_spacing.y * rowCount) + padding.top;

                SetChildAlongAxis(rectTransform, 0, xPos, _cellSize.x);
                SetChildAlongAxis(rectTransform, 1, yPos, _cellSize.y);
            }
        }

        #region Get datas - row/column count, container space
        private int GetRows(int childCount)
        {
            float sqrt = Mathf.Sqrt(childCount);
            _rows = Mathf.CeilToInt(sqrt);

            if (_fitType == FitType.Width) { _rows = Mathf.CeilToInt(childCount / (float)GetColumns(childCount)); }

            return _rows;
        }

        private int GetColumns(int childCount)
        {
            float sqrt = Mathf.Sqrt(childCount);
            _columns = Mathf.CeilToInt(sqrt);

            if (_fitType == FitType.Height) { _columns = Mathf.CeilToInt(childCount / (float)GetRows(childCount)); }

            return _columns;
        }

        private Vector2 GetContainerSpace()
        {
            Vector2 space = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

            return space;
        }
        #endregion
    }
}