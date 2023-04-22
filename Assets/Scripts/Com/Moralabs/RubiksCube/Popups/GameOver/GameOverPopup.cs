using Com.Moralabs.RubiksCube.LevelSystem;
using Com.Moralabs.RubiksCube.Manager;
using Com.Moralabs.RubiksCube.Util;
using Morautils.AdSystem;
using Morautils.AudioSystem;
using Morautils.PopupSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Com.Moralabs.RubiksCube.Popups.GameOver {
    public class GameOverPopup : BasePopup {

        [SerializeField]
        private TMP_Text timerText;

        private static int nextLevel;

        [Inject]
        private ProjectManager projectManager;
        [Inject]
        private LevelController levelController;
        [Inject]
        private ISoundController2d soundController;
        [Inject]
        private IAdController adController;

        private IFxSource fireworkSource;

        public override void OnOpened(Hashtable args) {
            base.OnOpened(args);
            if (args != null) {
                if (args.ContainsKey("level")) {
                    nextLevel = (int)args["level"] + 1;
                }

                if (args.ContainsKey("time")) {
                    timerText.text = args["time"].ToString();
                }
            }
            PlayFireworkSound();


        }

        public void NextButton() {
            fireworkSource.Stop();
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            if (projectManager.ActiveLevelData.currentLevel > 3) {
                adController.ShowInterstitial();
            }
            if (nextLevel > projectManager.ActiveCategoryData.numberOfLevels) {
                if (projectManager.ActiveCategoryData.categoryId == levelController.GetCategoryData().Length) {
                    SceneManager.LoadSceneAsync(Constants.CATEGORY_SCENE);
                }
                else {
                    projectManager.PlayCategoryAndLevel(levelController.GetCategoriesWithId(projectManager.ActiveCategoryData.categoryId + 1).category, 1);
                }
            }
            else {
                projectManager.PlayLevel(nextLevel);
            }
        }

        public void HomeButton() {
            fireworkSource.Stop();
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            if (projectManager.ActiveLevelData.currentLevel > 3) {
                adController.ShowInterstitial();
            }
            SceneManager.LoadSceneAsync(Constants.CATEGORY_SCENE);
           
        }

        private async void PlayFireworkSound() {
            fireworkSource = await soundController.PlayAndGetSource(new Sound(Sounds.FIREWORKS, autoDespawn: false));
        }
    }
}

