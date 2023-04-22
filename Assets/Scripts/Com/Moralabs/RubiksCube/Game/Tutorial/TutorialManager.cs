using Com.Moralabs.RubiksCube.Game.Grid;
using Com.Moralabs.RubiksCube.Game.Manager;
using Com.Moralabs.RubiksCube.Manager;
using Morautils.InputSystem.Pointer;
using System.Reflection;
using UniRx;
using UnityEngine;
using Zenject;

namespace Com.Moralabs.RubiksCube.Tutorial {
    public class TutorialManager : MonoBehaviour {

        [SerializeField]
        Hand hand;

        [Inject]
        GameManager gameManager;
        [Inject]
        ProjectManager projectManager;
        [Inject]
        private IDragEvent dragEvent;

        private int level;
        private int step;
        private Vector2 firstTouch, finalTouch, distance;
        private GridUtility gridUtility;
        private Vector2Int gridPosition;
        private static int stepNumber;
        private int previousMovesCount;

        private void Start() {
            stepNumber = 1;
            if (HasTutorial()) {
                InitializeTutorial();
                
            }
            
        }

        private void OnDrag(PointerData input) {
            if (input.state == PointerState.Begin) {
                OnDragBegin(input);
            }
            else if (input.state == PointerState.Hold) {
                OnDragHold(input);
            }
            else if (input.state == PointerState.End) {
                OnDragEnd(input);
            }
        }

        private void OnDragBegin(PointerData input) {

            firstTouch = gridUtility.ScreenToUiPosition(input.position);
            gridPosition = gridUtility.UIToGridPosition(firstTouch);

        }
        
        private void OnDragHold(PointerData input) {

            var moveTouch = gridUtility.ScreenToUiPosition(input.position);
            var currentDistance = moveTouch - firstTouch;
            int numberOfBlocksX = NumberOfBlocksX(currentDistance);
            int numberOfBlocksY = NumberOfBlocksY(currentDistance);
            var movement = new Vector2Int(numberOfBlocksX, numberOfBlocksY);
            previousMovesCount = gameManager.PreviousMoves.Count;

            if (!DragControl(stepNumber, gridPosition, projectManager.ActiveLevelData.currentLevel, movement)) {
                foreach (var block in gameManager.Blocks) {
                    block.SetPosition(block.Position);
                }
            }
        }


        private void OnDragEnd(PointerData input) {
            finalTouch = gridUtility.ScreenToUiPosition(input.position);
            distance = finalTouch - firstTouch;
            int numberOfBlocksX = NumberOfBlocksX(distance);
            int numberOfBlocksY = NumberOfBlocksY(distance);
            var movement = new Vector2Int(numberOfBlocksX, numberOfBlocksY);
            
            if (previousMovesCount != gameManager.PreviousMoves.Count) {
                if (!DragControl(stepNumber, gridPosition, projectManager.ActiveLevelData.currentLevel, movement)) {
                    gameManager.PreviousPosition();
                }
            }

            if (Mathf.Abs(numberOfBlocksX) >= 1 || Mathf.Abs(numberOfBlocksY) >= 1) {
                SetStep();
            }
            gameManager.NextMoves.Clear();

        }
        private int NumberOfBlocksX(Vector2 distance) {

            float blocksFromDistanceX = distance.x / gridUtility.BlockSize;
            int numberOfBlocks = (int)blocksFromDistanceX;

            if (Mathf.Abs(blocksFromDistanceX % 1) > 0.35f) {
                int negOrPosNumber = blocksFromDistanceX >= 0 ? 1 : -1;

                if (negOrPosNumber >= 1) {
                    numberOfBlocks = Mathf.CeilToInt(blocksFromDistanceX);
                }
                else {
                    numberOfBlocks = Mathf.FloorToInt(blocksFromDistanceX);
                }
            }
            return numberOfBlocks;
        }
        private int NumberOfBlocksY(Vector2 distance) {

            float blocksFromDistanceY = distance.y / gridUtility.BlockSize;
            int numberOfBlocks = (int)blocksFromDistanceY;

            if (Mathf.Abs(blocksFromDistanceY % 1) > 0.35f) {
                int negOrPosNumber = blocksFromDistanceY >= 0 ? 1 : -1;

                if (negOrPosNumber >= 1) {
                    numberOfBlocks = Mathf.CeilToInt(blocksFromDistanceY);
                }
                else {
                    numberOfBlocks = Mathf.FloorToInt(blocksFromDistanceY);
                }
            }
            return numberOfBlocks;
        }

        private bool HasTutorial() {
            return (projectManager.ActiveLevelData.currentLevel == 1 && projectManager.ActiveCategoryData.categoryId == 1) ||
                (projectManager.ActiveLevelData.currentLevel == 4 && projectManager.ActiveCategoryData.categoryId == 1);
        }

        private void InitializeTutorial() {
            level = projectManager.ActiveLevelData.currentLevel;
            step = -1;

            var field = typeof(GameManager).GetField("gridUtility", BindingFlags.NonPublic | BindingFlags.Instance);
            gridUtility = (GridUtility)field.GetValue(gameManager);

            dragEvent.OnDrag
                 .TakeUntilDestroy(gameObject)
                 .Subscribe(OnDrag);

            SetStep();
        }

        private void SetStep() {

            if (level == 1) {
                SetStepLevel1();
            }
            else if (level == 4) {
                SetStepLevel4();
            }
        }


        private void SetStepLevel1() {
            step++;

            if (step == 0) {
                StepLevel1Step0();
            }
            else if (step == 1) {
                if (ControlStep(("Block(1, 1)"), new Vector2Int(0, 1)) ) {
                    gameManager.PreviousMoves.Pop();
                    stepNumber++;
                    StepLevel1Step1();
                }
                else {

                    gameManager.PreviousPosition();
                    StepLevel1Step0();
                    step--;
                }
            }
            else if (step == 2) {
                if (ControlStep(("Block(0, 1)"), new Vector2Int(0, 1)) ) {
                    gameManager.PreviousMoves.Pop();
                    stepNumber++;
                    StepLevel1Step2();
                }
                else {

                    gameManager.PreviousPosition();
                    StepLevel1Step1();
                    step--;
                }
            }
        }

        public void StepLevel1Step0() {
            if (projectManager.ActiveCategoryData.categoryId == 1 && projectManager.ActiveLevelData.currentLevel == 1) {
                hand.SetHand(new Vector3(-100, -400, 0), new Vector3(-100, 0, 0));
            }
        }

        public void StepLevel1Step1() {
            hand.SetHand(new Vector3(200, -50, 0), new Vector3(-200, -50, 0));
        }
        public void StepLevel1Step2() {
            hand.StopHand();
        }

        private void SetStepLevel4() {
            step++;

            if (step == 0) {
                StepLevel4Step0();
            }
            else if (step == 1) {
                
                if (ControlStep(("Block(1, 2)"), new Vector2Int(1, 1)) ) {
                    gameManager.PreviousMoves.Pop();
                    stepNumber++;
                    StepLevel4Step1();
                }
                else {
                    gameManager.PreviousPosition();
                    step--;
                    StepLevel4Step0();
                }

            }
            else if (step == 2) {
                
                if (ControlStep(("Block(1, 1)"), new Vector2Int(1, 0)) ) {
                    gameManager.PreviousMoves.Pop();
                    stepNumber++;
                    StepLevel4Step2();
                }
                else {
                    gameManager.PreviousPosition();
                    step--;
                    StepLevel4Step1();
                }

            }
            else if (step == 3) {
                
                if (ControlStep(("Block(1, 1)"), new Vector2Int(1, 1)) ) {
                    gameManager.PreviousMoves.Pop();
                    stepNumber++;
                    StepLevel4Step3();
                }
                else {
                    gameManager.PreviousPosition();
                    step--;
                    StepLevel4Step2();
                }
            }
        }


        private void StepLevel4Step0() {
            if (projectManager.ActiveCategoryData.categoryId == 1 && projectManager.ActiveLevelData.currentLevel == 4) {
                hand.SetHand(new Vector3(30, 40, 0), new Vector3(30, -180, 0));
            }
        }
        private void StepLevel4Step1() {
            hand.SetHand(new Vector3(-230, -450, 0), new Vector3(0, -450, 0));
        }
        private void StepLevel4Step2() {
            hand.SetHand(new Vector3(40, -450, 0), new Vector3(40, -180, 0));
        }
        private void StepLevel4Step3() {
            hand.StopHand();
        }

        private bool ControlStep(string blockName, Vector2Int position) {
            foreach (var block in gameManager.Blocks) {
                if (block.name.Equals(blockName)) {
                    if (block.Position == position) {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool DragControl(int stepNumber, Vector2Int gridPosition, int level, Vector2Int movement, Vector2 distance) {
            if (stepNumber == 1 && (gridPosition == new Vector2Int(0, 0) || gridPosition == new Vector2Int(0, 1)) 
                && level == 1 && (movement.x) == 0 && distance.y > 0) {
                return true;
            }
            else if (stepNumber == 2 && (gridPosition == new Vector2Int(1, 1) || gridPosition == new Vector2Int(0, 1)) 
                && level == 1 && (movement.y) == 0 && distance.x < 0) {
                return true;
            }
            else if (stepNumber == 1 && (gridPosition == new Vector2Int(1, 2) || gridPosition == new Vector2Int(1, 1) || gridPosition == new Vector2Int(1, 0))
                && level == 4 && (movement.x) == 0 && distance.y < 0 ) {
                return true;
            }
            else if (stepNumber == 2 && (gridPosition == new Vector2Int(0, 0) || gridPosition == new Vector2Int(1, 0) || gridPosition == new Vector2Int(2, 0)) 
                && level == 4 && (movement.y) == 0 && distance.x > 0) {
                return true;
            }
            else if (stepNumber == 3 && (gridPosition == new Vector2Int(1, 0) || gridPosition == new Vector2Int(1, 1) || gridPosition == new Vector2Int(1, 2))
                && level == 4 && (movement.x) == 0 && distance.y > 0) {
                return true;
            }
            else {
                return false;
            }
        }
        public bool DragControl(int stepNumber, Vector2Int gridPosition, int level, Vector2Int movement) {
            if (stepNumber == 1 && (gridPosition == new Vector2Int(0, 0) || gridPosition == new Vector2Int(0, 1))
                && level == 1 && (movement.x) == 0 ) {
                return true;
            }
            else if (stepNumber == 2 && (gridPosition == new Vector2Int(1, 1) || gridPosition == new Vector2Int(0, 1))
                && level == 1 && (movement.y) == 0 ) {
                return true;
            }
            else if (stepNumber == 1 && (gridPosition == new Vector2Int(1, 2) || gridPosition == new Vector2Int(1, 1) || gridPosition == new Vector2Int(1, 0))
                && level == 4 && (movement.x) == 0 ) {
                return true;
            }
            else if (stepNumber == 2 && (gridPosition == new Vector2Int(0, 0) || gridPosition == new Vector2Int(1, 0) || gridPosition == new Vector2Int(2, 0))
                && level == 4 && (movement.y) == 0) {
                return true;
            }
            else if (stepNumber == 3 && (gridPosition == new Vector2Int(1, 0) || gridPosition == new Vector2Int(1, 1) || gridPosition == new Vector2Int(1, 2))
                && level == 4 && (movement.x) == 0 ) {
                return true;
            }
            else {
                return false;
            }
        }

    }
}