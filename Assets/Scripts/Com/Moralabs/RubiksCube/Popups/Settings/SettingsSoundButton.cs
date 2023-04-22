using Com.Moralabs.RubiksCube.Util;
using Morautils.AudioSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Com.Moralabs.RubiksCube.Popups.Settings {
    public class SettingsSoundButton : MonoBehaviour {

        [SerializeField]
        private Image onOffImage;

        [Inject]
        private ISoundController2d soundController;


        private void Awake() {
            onOffImage.GetComponent<Image>().enabled = !soundController.Enabled;
        }

        public void GameSoundOffButton() {
            soundController.Enabled = !soundController.Enabled;
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            onOffImage.GetComponent<Image>().enabled = !onOffImage.GetComponent<Image>().enabled;
        }
    }

}
