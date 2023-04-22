using Com.Moralabs.RubiksCube.Util;
using Morautils.AudioSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Com.Moralabs.RubiksCube.Category.Ui {
    public class CategoryReturnButton : MonoBehaviour {
        [Inject]
        private ISoundController2d soundController;

        public void DashboardScene() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            SceneManager.LoadSceneAsync(Constants.DASHBOARD_SCENE);
        }
        public void CategoryScene() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            SceneManager.LoadSceneAsync(Constants.DASHBOARD_SCENE);
        }
    }
}