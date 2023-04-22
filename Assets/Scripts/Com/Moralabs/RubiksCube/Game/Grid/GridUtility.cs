using UnityEngine;
namespace Com.Moralabs.RubiksCube.Game.Grid {
    public enum OutOfPoint {
        Inside,
        Up,
        Down,
        Left,
        Right
    }

    public class GridUtility {
        private int rows;
        private int columns;
        private float blockSize;
        private float width;
        private float height;
        private RectTransform boardParent;
        private RectTransform gameArea;
        public int Rows => rows;
        public int Columns => columns;
        public float BlockSize => blockSize;
        public RectTransform BoardParent => boardParent;
        public GridUtility(int columns, int rows, RectTransform boardParent, RectTransform gameArea) {
            this.boardParent = boardParent;
            this.columns = columns;
            this.rows = rows;
            width = gameArea.rect.width;
            height = gameArea.rect.height;
            float ratio = columns / (float)rows;
            //square
            if (ratio == 1) {
                height = Mathf.Min(height, width);
                width = height;
            }
            //rectangle
            else if (ratio > 1) {
                float tmpRatio = width / height;
                if (ratio > tmpRatio) {
                    height = width / ratio;
                }
                else {
                    width = height * ratio;
                }
            }
            else {
                float tmpRatio = width / height;
                if (ratio > tmpRatio) {
                    height = width / ratio;
                }
                else {
                    width = height * ratio;
                }
            }
            this.boardParent.sizeDelta = new Vector2(width, height);
            blockSize = width / columns;
        }
        public Vector2 GridToUIPosition(Vector2Int gridPos) {
            return new Vector2((blockSize / 2 + blockSize * gridPos.x),
                (blockSize / 2 + blockSize * gridPos.y));
        }
        public Vector2Int UIToGridPosition(Vector2 uiPos) {
            return new Vector2Int(Mathf.RoundToInt((uiPos.x - blockSize / 2) / blockSize),
                Mathf.RoundToInt((uiPos.y - blockSize / 2) / blockSize));
        }
        public Vector2 ScreenToUiPosition(Vector3 screenPosition) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(boardParent, screenPosition, null, out var point);
            point += new Vector2(width / 2, height / 2);
            return point;
        }
        public bool IsBoardParentContainsPoint(Vector2 uiPos) {
            return uiPos.x >= 0 && uiPos.x <= boardParent.rect.width &&
                uiPos.y >= 0 && uiPos.y <= boardParent.rect.height;
        }

        public OutOfPoint IsPointOutOfBoardParent(Vector2 uiPos) {

            if (uiPos.x < boardParent.rect.min.x - 1f) {
                return OutOfPoint.Left;
            }
            else if (uiPos.x > boardParent.rect.max.x + 1f) {
                return OutOfPoint.Right;
            }
            else if (uiPos.y < boardParent.rect.min.y - 1f) {
                return OutOfPoint.Down;
            }
            else if (uiPos.y > boardParent.rect.max.y + 1f) {
                return OutOfPoint.Up;
            }

            return OutOfPoint.Inside;
        }
    }
}