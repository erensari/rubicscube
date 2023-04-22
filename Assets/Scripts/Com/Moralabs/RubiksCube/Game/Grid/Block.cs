using UnityEngine;
using UnityEngine.UI;
namespace Com.Moralabs.RubiksCube.Game.Grid {
    public class Block : MonoBehaviour {
        [SerializeField]
        private Image image;

        public Vector2Int Position;
        public Vector2Int CorrectPosition;

        private GridUtility gridUtility;
        private bool isTeleported;
        private GameObject dummyBlock;

        public void SetPosition(Vector2Int position) {

            Position = position;
            ((RectTransform)transform).anchoredPosition = gridUtility.GridToUIPosition(position);
                     
            isTeleported = false;
            if (dummyBlock != null) {
                Destroy(dummyBlock);
                dummyBlock = null;
            }
        }

        public void SetTempPosition(Vector2 position) {
            var rt = (RectTransform)transform;

            rt.anchoredPosition = position;

            Vector2 bottomLeftPoint = rt.localPosition;
            bottomLeftPoint.x -= rt.rect.width / 2;
            bottomLeftPoint.y -= rt.rect.height / 2;

            Vector2 topRightPoint = rt.localPosition;
            topRightPoint.x += rt.rect.width / 2;
            topRightPoint.y += rt.rect.height / 2;

            var bottomLeft = gridUtility.IsPointOutOfBoardParent(bottomLeftPoint);
            var topRight = gridUtility.IsPointOutOfBoardParent(topRightPoint);

            // bottom left
            if (!isTeleported && bottomLeft != OutOfPoint.Inside) {
                isTeleported = true;
                if (bottomLeft == OutOfPoint.Down) {
                    position.y += gridUtility.BoardParent.rect.height;
                }
                else if (bottomLeft == OutOfPoint.Left) {
                    position.x += gridUtility.BoardParent.rect.width;
                }

                dummyBlock = Instantiate(this, rt.parent).gameObject;
                dummyBlock.transform.position = rt.position;
                Destroy(dummyBlock.GetComponent<Block>());
                rt.anchoredPosition = position;
                dummyBlock.transform.SetParent(rt);
            }
            // top right
            else if (!isTeleported && topRight != OutOfPoint.Inside) {
                isTeleported = true;
                if (topRight == OutOfPoint.Up) {
                    position.y -= gridUtility.BoardParent.rect.height;
                }
                else if (topRight == OutOfPoint.Right) {
                    position.x -= gridUtility.BoardParent.rect.width;
                }

                dummyBlock = Instantiate(this, rt.parent).gameObject;
                dummyBlock.transform.position = rt.position;
                Destroy(dummyBlock.GetComponent<Block>());
                rt.anchoredPosition = position;
                dummyBlock.transform.SetParent(rt);
            }
            else if (isTeleported && topRight == OutOfPoint.Inside && bottomLeft == OutOfPoint.Inside) {
                isTeleported = false;
                if (dummyBlock != null) {
                    Destroy(dummyBlock);
                    dummyBlock = null;
                }
            }
            else if (isTeleported && topRight != OutOfPoint.Inside && bottomLeft != OutOfPoint.Inside) {
                isTeleported = false;
                if (dummyBlock != null) {
                    transform.position = dummyBlock.transform.position;
                    Destroy(dummyBlock);
                    dummyBlock = null;
                }
            }
 


        }
       
        public Vector2 GetTempPosition() {
            return ((RectTransform)transform).anchoredPosition;
        }

        public void Initialize(GridUtility gridUtility, Sprite sprite, Vector2Int position) {
            this.gridUtility = gridUtility;
            image.sprite = sprite;
            SetPosition(position);
        }
        public void MoveTo(Vector2Int movement) {
            var newPosition = Position + (movement);
            if (newPosition.x < 0) {
                newPosition.x = ((newPosition.x % gridUtility.Columns) + gridUtility.Columns) % gridUtility.Columns;
            }
            else if (newPosition.x >= gridUtility.Columns) {
                newPosition.x %= gridUtility.Columns;
            }
            if (newPosition.y < 0) {
                newPosition.y = ((newPosition.y % gridUtility.Rows) + gridUtility.Rows) % gridUtility.Rows;
            }
            else if (newPosition.y >= gridUtility.Rows) {
                newPosition.y %= gridUtility.Rows;
            }
            SetPosition(newPosition);
        }
        public bool CheckPosition() {
            return Position == CorrectPosition;
        }
    }
}
