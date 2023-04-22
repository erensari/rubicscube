using Com.Moralabs.RubiksCube.Game.Grid;
using Com.Moralabs.RubiksCube.LevelSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using System;
using Random = UnityEngine.Random;
using Com.Moralabs.RubiksCube.Manager;
using System.Collections;
using System.Globalization;
using Com.Moralabs.RubiksCube.AnalyticsSystem;
using Com.Moralabs.RubiksCube.Util;
using Cysharp.Threading.Tasks;

namespace Com.Moralabs.RubiksCube.Game.Manager
{
    public class GameManager : MonoBehaviour {

        [SerializeField] private Block _blockPrefab;
        [SerializeField] private SpriteRenderer _boardPrefab;
        [SerializeField] private RectTransform boardParent;
        [SerializeField] private GameObject gameArea;
        [SerializeField] private RectTransform originalImageArea;
        [SerializeField] private RectTransform canvas;
        [SerializeField] private GameObject confetti;

        private int width;
        private int height;
        private List<Block> _blocks;
        private bool isPlaying;
        private bool isDragging;
        private GridUtility gridUtility;
        private Vector2 firstTouch, finalTouch, distance, prevTouch;
        private IDisposable timer;
        private float saveTimer = 0;
        private int movementCount = 0;
        private readonly ReactiveProperty<float> currentTime = new ReactiveProperty<float>(0);
        public IReadOnlyReactiveProperty<float> CurrentTime => currentTime;
        private Stack<List<Vector2Int>> previousMoves;
        private Stack<List<Vector2Int>> nextMoves;
        private List<Vector2Int> initialPositions;
        private List<List<Vector2Int>> positionListForStack = new List<List<Vector2Int>>();


        [Inject]
        private IDragEvent dragEvent;
        [Inject]
        private ProjectManager projectManager;
        [Inject]
        private LevelController levelController;
        [Inject]
        private IPopupController popupController;
        [Inject]
        private AnalyticsManager analyticsManager;
        [Inject]
        private ISoundController2d soundController;

        public List<Block> Blocks {
            get { return _blocks; }
            set { }
        }
        public Stack<List<Vector2Int>> PreviousMoves {
            get { return previousMoves; }
            set { }
        }
        public Stack<List<Vector2Int>> NextMoves {
            get { return nextMoves; }
            set { }
        }

        private enum LockState {
            Vertical,
            Horizontal,
            NotSet
        }

        private LockState lockState;

        private void Awake() {
            dragEvent.OnDrag
                .TakeUntilDestroy(gameObject)
                .Subscribe(OnDrag);
        }

        void Start() {
            isDragging = false;
            isPlaying = true;
            Time.timeScale = 1f;
            previousMoves = new Stack<List<Vector2Int>>();
            nextMoves = new Stack<List<Vector2Int>>();
            Initialize(projectManager.ActiveLevelData);
            string str = projectManager.ActiveLevelData.category + "/" + projectManager.ActiveLevelData.currentLevel;
            PlayerPrefs.SetString(Constants.LAST_ACTIVE_CATEGORY_LEVEL, str);
            analyticsManager.LogProgressionEvent(AnalyticsManager.ProgressionStatus.Start, null, projectManager.ActiveCategoryData.category, projectManager.ActiveLevelData.currentLevel.ToString());
            PlayerPrefs.SetInt(Constants.IS_ACTIVE_LEVEL_COMPLETED, 0);

        }

        private void Update() {
            saveTimer += Time.deltaTime;
            if (saveTimer >= 1f) {
                saveTimer = 0;
                SaveBlockPositions(projectManager.ActiveCategoryData.category, projectManager.ActiveLevelData.currentLevel);
            }
        }

        public void Initialize(LevelData levelData) {
            lockState = LockState.NotSet;
            width = levelData.width;
            height = levelData.height;
            var sprite = ImageLoader.LoadImage(levelData.imageName, projectManager.ActiveCategoryData.category);
            originalImageArea.GetComponent<Image>().sprite = sprite;
            gridUtility = new GridUtility(height, width, (RectTransform)boardParent.transform, (RectTransform)gameArea.transform);

            if (projectManager.anySavedPositions) {
                SpawnBlocksFromSavedPositions(sprite, width, height);
                projectManager.anySavedPositions = false;
                StartTimer();
                BlockData blockDatas = LoadBlockPositions();

                if (blockDatas == null) {
                    SpawnBlocks(sprite, width, height);
                    ShuffleBlocks();
                    StartTimer();
                }
                else {
                    previousMoves = new Stack<List<Vector2Int>>(blockDatas.previousMoves.Reverse<List<Vector2Int>>());
                    currentTime.Value = blockDatas.time;
                }

            }
            else {
                SpawnBlocks(sprite, width, height);
                ShuffleBlocks();
                StartTimer();

                List<Vector2Int> positions = new List<Vector2Int>();

                foreach (var block in _blocks) {
                    positions.Add(block.Position);
                }

                initialPositions = positions;
                previousMoves.Clear();
                positionListForStack.Add(initialPositions);


            }
        }
        private void StartTimer() {
            timer?.Dispose();
            timer = Observable.EveryGameObjectUpdate()
                .Subscribe(_ => {
                    currentTime.Value += Time.deltaTime;
                });
        }

        private void OnDrag(PointerData input) {
            if (isPlaying) {
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
        }

        private void OnDragBegin(PointerData input) {
            if (isPlaying) {
                firstTouch = gridUtility.ScreenToUiPosition(input.position);
                prevTouch = firstTouch;
                isDragging = gridUtility.IsBoardParentContainsPoint(firstTouch);
            }
        }
        private void OnDragHold(PointerData input) {
            if (!isDragging) {
                return;
            }

            var moveTouch = gridUtility.ScreenToUiPosition(input.position);
            distance = moveTouch - prevTouch;
            prevTouch = moveTouch;

            if (lockState == LockState.NotSet) {

                if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y)) {
                    lockState = LockState.Horizontal;
                }
                else {
                    lockState = LockState.Vertical;
                }
            }

            if (lockState != LockState.NotSet) {
                var gridPosition = gridUtility.UIToGridPosition(firstTouch);

                if (lockState == LockState.Vertical) {
                    var column = _blocks.Where(b => b.Position.x == gridPosition.x);
                    foreach (var block in column) {
                        var pos = block.GetTempPosition();
                        pos.y += distance.y;
                        block.SetTempPosition(pos);
                    }
                }
                else if (lockState == LockState.Horizontal) {
                    var row = _blocks.Where(b => b.Position.y == gridPosition.y);
                    foreach (var block in row) {

                        var pos = block.GetTempPosition();
                        pos.x += distance.x;

                        block.SetTempPosition(pos);
                    }
                }
            }

        }

        private void OnDragEnd(PointerData input) {
            if (isDragging && lockState != LockState.NotSet) {

                finalTouch = gridUtility.ScreenToUiPosition(input.position);
                distance = finalTouch - firstTouch;

                Vector2Int movement = Vector2Int.zero;

                if (lockState == LockState.Horizontal) {


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

                    movement = new Vector2Int(numberOfBlocks, 0);


                    // horizontal
                }
                else {
                    // vertical

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

                    movement = new Vector2Int(0, numberOfBlocks);
                }

                isDragging = false;
                lockState = LockState.NotSet;

                if (movement != new Vector2Int(0, 0)) {
                    movementCount++;
                }

                SwapBlock(gridUtility.UIToGridPosition(firstTouch), movement);

                CheckGameOver();
            }
        }

        void SpawnBlocks(Sprite mainSprite, int row, int column) {
            _blocks = new List<Block>();
            var splitedSprites = ImageSplitter.SplitImage(mainSprite, row, column);
            for (int x = 0; x < column; x++) {
                for (int y = 0; y < row; y++) {
                    var gridPos = new Vector2Int(x, y);
                    var block = Instantiate(_blockPrefab);
                    block.transform.SetParent(boardParent.transform);
                    block.transform.localScale = new Vector3(1, 1, 1);
                    var rt = (RectTransform)block.transform;
                    rt.sizeDelta = new Vector2(gridUtility.BlockSize, gridUtility.BlockSize);
                    block.name = Constants.BLOCK_NAME + gridPos;
                    block.Initialize(gridUtility, splitedSprites[x, y], gridPos);
                    block.Position = gridPos;
                    block.CorrectPosition = gridPos;
                    _blocks.Add(block);
                }
            }
        }

        void SpawnBlocksFromSavedPositions(Sprite mainSprite, int row, int column) {
            _blocks = new List<Block>();

            BlockData blockDatas = LoadBlockPositions();
            previousMoves = new Stack<List<Vector2Int>>(blockDatas.previousMoves.Reverse<List<Vector2Int>>());

            if (blockDatas == null) {
                SpawnBlocks(mainSprite, row, column);
            }

            var splitedSprites = ImageSplitter.SplitImage(mainSprite, row, column);

            int index = 0;
            for (int x = 0; x < column; x++) {
                for (int y = 0; y < row; y++) {
                    var gridPos = blockDatas.lastPositions[index++];
                    var block = Instantiate(_blockPrefab);
                    block.transform.SetParent(boardParent.transform);
                    block.transform.localScale = new Vector3(1, 1, 1);
                    var rt = (RectTransform)block.transform;
                    rt.sizeDelta = new Vector2(gridUtility.BlockSize, gridUtility.BlockSize);
                    block.name = Constants.BLOCK_NAME + gridPos;
                    block.Initialize(gridUtility, splitedSprites[x, y], gridPos);
                    block.Position = gridPos;
                    block.CorrectPosition = new Vector2Int(x, y);
                    _blocks.Add(block);
                }
            }
        }

        public void NextPositions() {
            if (nextMoves.Count > 0) {
                List<Vector2Int> positions = nextMoves.Pop();
                var temp = positions;

                List<Vector2Int> currentPosition = new List<Vector2Int>();
                foreach (var block in _blocks) {
                    currentPosition.Add(block.Position);
                }
                previousMoves.Push(currentPosition);

                for (int j = 0; j < _blocks.Count; j++) {
                    _blocks[j].SetPosition(positions[j]);
                }

                SaveBlockPositions(projectManager.ActiveCategoryData.category, projectManager.ActiveLevelData.currentLevel);

            }

        }

        public void PreviousPosition() {
            if (previousMoves.Count > 0) {
                List<Vector2Int> positions = previousMoves.Pop();

                List<Vector2Int> currentPosition = new List<Vector2Int>();

                foreach (var block in _blocks) {
                    currentPosition.Add(block.Position);
                }
                nextMoves.Push(currentPosition);


                for (int j = 0; j < _blocks.Count; j++) {
                    _blocks[j].SetPosition(positions[j]);
                }

                SaveBlockPositions(projectManager.ActiveCategoryData.category, projectManager.ActiveLevelData.currentLevel);

            }
        }


        void SaveBlockPositions(string category, int level) {
            BlockData blockDatas;
            List<Vector2Int> positions = new List<Vector2Int>();

            foreach (var block in _blocks) {
                positions.Add(block.Position);
            }

            blockDatas = new BlockData(positions, category, level, currentTime.Value, previousMoves.ToList(), initialPositions, nextMoves.ToList());
            string str = blockDatas.ToJson();
            PlayerPrefs.SetString(Constants.SAVED_BLOCK_POSITIONS, str);
        }

        private BlockData LoadBlockPositions() {

            BlockData blockDatas;
            string str = PlayerPrefs.GetString(Constants.SAVED_BLOCK_POSITIONS);
            if (string.IsNullOrEmpty(str)) {
                blockDatas = null;
            }
            blockDatas = str.ParseJson<BlockData>();
            return blockDatas;
        }

        void SwapBlock(Vector2Int gridPosition, Vector2Int movement) {

            if (!isPlaying)
                return;

            // Go Back
            if (movement == Vector2Int.zero) {
                var rowColumn = _blocks.Where(b => b.Position.x == gridPosition.x || b.Position.y == gridPosition.y);
                foreach (var block in rowColumn) {
                    block.MoveTo(movement);
                }
            }
            // vertical
            else if (movement.x == 0) {
                var column = _blocks.Where(b => b.Position.x == gridPosition.x);


                List<Vector2Int> currentPosition = new List<Vector2Int>();
                foreach (var block in _blocks) {
                    currentPosition.Add(block.Position);
                }

                foreach (var block in column) {
                    block.MoveTo(movement);
                }
                previousMoves.Push(currentPosition);
                SaveBlockPositions(projectManager.ActiveCategoryData.category, projectManager.ActiveLevelData.currentLevel);
                soundController.PlaySound(new Sound(Sounds.SLIDING));
            }
            // Horizontal
            else if (movement.y == 0) {
                var row = _blocks.Where(b => b.Position.y == gridPosition.y);


                List<Vector2Int> currentPosition = new List<Vector2Int>();
                foreach (var block in _blocks) {
                    currentPosition.Add(block.Position);
                }


                foreach (var block in row) {
                    block.MoveTo(movement);
                }
                previousMoves.Push(currentPosition);
                SaveBlockPositions(projectManager.ActiveCategoryData.category, projectManager.ActiveLevelData.currentLevel);
                soundController.PlaySound(new Sound(Sounds.SLIDING));
            }
            nextMoves.Clear();
        }

        void SwapBlockForShuffle(Vector2Int gridPosition, Vector2Int movement) {
            if (!isPlaying)
                return;

            // Go Back
            if (movement == Vector2Int.zero) {
                var rowColumn = _blocks.Where(b => b.Position.x == gridPosition.x || b.Position.y == gridPosition.y);
                foreach (var block in rowColumn) {
                    block.MoveTo(movement);
                }
            }
            // vertical
            else if (movement.x == 0) {
                var column = _blocks.Where(b => b.Position.x == gridPosition.x);
                foreach (var block in column) {
                    block.MoveTo(movement);
                }

            }
            // Horizontal
            else if (movement.y == 0) {
                var row = _blocks.Where(b => b.Position.y == gridPosition.y);
                foreach (var block in row) {
                    block.MoveTo(movement);
                }
            }
        }

        void ShuffleBlocks() {
            if (projectManager.ActiveCategoryData.categoryId == 1 && projectManager.ActiveLevelData.currentLevel == 1) {
                SwapBlockForShuffle(_blocks[1].Position, new Vector2Int(1, 0));
                SwapBlockForShuffle(_blocks[0].Position, new Vector2Int(0, 1));
            }
            else if (projectManager.ActiveCategoryData.categoryId == 1 && projectManager.ActiveLevelData.currentLevel == 4) {
                SwapBlockForShuffle(_blocks[5].Position, new Vector2Int(0, 2));
                SwapBlockForShuffle(_blocks[6].Position, new Vector2Int(2, 0));
                SwapBlockForShuffle(_blocks[5].Position, new Vector2Int(0, 1));

            }
            else {
                int correctBlockCount = 0;
                foreach (var block in _blocks) {
                    SwapBlockForShuffle(block.Position, new Vector2Int(0, Random.Range(0, height)));
                    SwapBlockForShuffle(block.Position, new Vector2Int(Random.Range(0, width), 0));
                }
                foreach (var block in _blocks) {
                    if (block.Position == block.CorrectPosition)
                        correctBlockCount++;
                }
                //To ensure that the blocks are not all in their correct position after shuffle
                if (correctBlockCount == _blocks.Count) {
                    ShuffleBlocks();
                }
                SaveBlockPositions(projectManager.ActiveCategoryData.category, projectManager.ActiveLevelData.currentLevel);
            }
        }



        void CheckGameOver() {
            bool gameOver = _blocks.All(b => b.CheckPosition());
            if (gameOver) {
                isPlaying = false;
                timer?.Dispose();

                var seconds = (CurrentTime.Value % 60).ToString("00.0", CultureInfo.InvariantCulture);
                var minutes = Mathf.Floor(CurrentTime.Value / 60).ToString("00", CultureInfo.InvariantCulture);
                string completedTime = minutes + ":" + seconds;

                levelController.CompleteLevel(projectManager.ActiveLevelData.currentLevel,
                    projectManager.ActiveLevelData.category, (float)Math.Round(currentTime.Value, 1), completedTime);

                analyticsManager.LogProgressionEvent(AnalyticsManager.ProgressionStatus.Complete, currentTime.Value, projectManager.ActiveCategoryData.category, projectManager.ActiveLevelData.currentLevel.ToString(), movementCount);



                PlayerPrefs.SetInt(Constants.IS_ACTIVE_LEVEL_COMPLETED, 1);

                Hashtable param = new Hashtable();
                param["level"] = projectManager.ActiveLevelData.currentLevel;
                param["time"] = completedTime;

                OpenGameOverPopup(param);
            }
        }
        public void PauseGame() {
            isPlaying = false;
            Time.timeScale = 0f;
            Hashtable param = new Hashtable();
            param[Constants.GAME_MANAGER] = this;
            popupController.OpenTop(Constants.PAUSE_POPUP, param);
        }
        public void ResumeGame() {
            isPlaying = true;
            Time.timeScale = 1f;
        }

        private async void OpenGameOverPopup(Hashtable param) {
            await UniTask.Delay(1000);
            popupController.OpenTop(Constants.GAME_OVER_POPUP, param);
            confetti.SetActive(true);
        }
    }
}
