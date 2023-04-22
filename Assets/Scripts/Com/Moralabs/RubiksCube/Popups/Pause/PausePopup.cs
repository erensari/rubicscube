using Com.Moralabs.RubiksCube.Game.Manager;
using Com.Moralabs.RubiksCube.Manager;
using Com.Moralabs.RubiksCube.Util;
using Morautils.AudioSystem;
using Morautils.PopupSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using Zenject;

namespace Com.Moralabs.RubiksCube.Popups.Pause {
    public class PausePopup : BasePopup {
        static GameManager gameManager;

        [Inject]
        private ISoundController2d soundController;
        [Inject]
        private ProjectManager projectManager;


        public override void OnOpened(Hashtable args) {
            base.OnOpened(args);

            gameManager = (GameManager)args[Constants.GAME_MANAGER];
        }


        public void ContinueButton() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            gameManager.ResumeGame();
            ClosePopup();

        }

        public void HomeButton() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            SceneManager.LoadSceneAsync(Constants.DASHBOARD_SCENE);
        }

        public void CategoryButton() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            SceneManager.LoadSceneAsync(Constants.CATEGORY_SCENE);
        }
        public void RestartButton() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            projectManager.PlayCategoryAndLevel(projectManager.ActiveCategoryData.category, projectManager.ActiveLevelData.currentLevel);
        }

    }
}
