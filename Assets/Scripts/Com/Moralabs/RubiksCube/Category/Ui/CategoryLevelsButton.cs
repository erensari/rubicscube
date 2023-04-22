using Com.Moralabs.RubiksCube.Manager;
using Com.Moralabs.RubiksCube.Util;
using Morautils.AudioSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Com.Moralabs.RubiksCube.Category.Ui {
    public class CategoryLevelsButton : MonoBehaviour {
        [SerializeField]
        private string categoryName;

        [Inject]
        private ProjectManager projectManager;
        [Inject]
        private ISoundController2d soundController;

        public string CategoryName => categoryName;

        public void LevelScene() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            projectManager.PlayCategory(categoryName);
        }

        public void SetLocked(bool locked) {
            GetComponent<Button>().interactable = !locked;
        }
    }
}
