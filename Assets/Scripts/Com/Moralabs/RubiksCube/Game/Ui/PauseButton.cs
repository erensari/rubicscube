using Com.Moralabs.RubiksCube.Game.Manager;
using Com.Moralabs.RubiksCube.Util;
using Morautils.AudioSystem;
using UnityEngine;
using Zenject;

namespace Com.Moralabs.RubiksCube.Game.Ui {
    public class PauseButton : MonoBehaviour {
        [Inject]
        GameManager gameManager;
        [Inject]
        private ISoundController2d soundController;
        public void Pause() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));

            gameManager.PauseGame();
          
        }
    }
}

