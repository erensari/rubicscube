using Com.Moralabs.RubiksCube.Category.Ui;
using Com.Moralabs.RubiksCube.LevelSystem;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Com.Moralabs.RubiksCube.Category {
    public class CategoryManager : MonoBehaviour {
        [SerializeField]
        private List<CategoryLevelsButton> buttons;
        [Inject]
        private LevelController levelController;

        void Awake() {
            for (int i = 1; i < buttons.Count; i++) {
                int numberOfLevels = levelController.GetCategoryData()[i - 1].numberOfLevels;
                bool isLocked = levelController.LastCompletedLevel(buttons[i - 1].CategoryName) / (float)numberOfLevels > 0.1;
                buttons[i].SetLocked(!isLocked);
            }
        }
    }
}