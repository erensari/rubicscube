using Com.Moralabs.RubiksCube.LevelSystem;
using Com.Moralabs.RubiksCube.Manager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Com.Moralabs.RubiksCube.Level.Ui {
    public class LevelButtonGenerator : MonoBehaviour {

        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private Transform levelParent;
        [SerializeField] private GameObject categoryImage;

        [Inject]
        ProjectManager projectManager;
        [Inject]
        private IFactory<GameObject, Transform, GameObject> prefabFactoryWithParent;
        [Inject]
        private LevelController levelController;

        private void OnEnable() {
            int levelCount = projectManager.ActiveCategoryData.numberOfLevels;

            List<LevelButton> levelList = new List<LevelButton>();

            for (int i = 1; i <= levelCount; i++) {
                LevelButton level = prefabFactoryWithParent.Create(levelPrefab, levelParent.transform).GetComponent<LevelButton>();
                bool isUnlocked = levelController.IsLocked(i, projectManager.ActiveCategoryData.category);

                string time = levelController.LevelCompletionTime(projectManager.ActiveCategoryData.category, i);

                bool isCompleted = true;

                if (string.IsNullOrEmpty(time)) {
                    isCompleted = false;
                    time = "00:00.0";
                }

                if (i == 1) {
                    isUnlocked = true;
                }
                level.Initialze(i, time, isUnlocked, isCompleted);
                levelList.Add(level);

            }
            
            var sprite = Resources.Load<Sprite>("CategoryImage/" + projectManager.ActiveCategoryData.category);
            categoryImage.GetComponent<Image>().sprite = sprite;

        }
    }
}