using Com.Moralabs.RubiksCube.Manager;
using Com.Moralabs.RubiksCube.Util;
using Morautils.AudioSystem;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Com.Moralabs.RubiksCube.DashboardScene.Ui {
    public class Dashboard : MonoBehaviour {

        [Inject]
        private ProjectManager projectManager;

        [Inject]
        private ISoundController2d soundController;
        

        private void Start() {
            projectManager.IsLoaded.Subscribe(IsAllLoaded);
            
        }

        private void IsAllLoaded(bool loaded) {
            if (loaded) {
                Debug.Log("ALL LOADED");
            }
        }

        public void CategoriesScene() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            SceneManager.LoadSceneAsync(Constants.CATEGORY_SCENE);
        }

    
    }
}
