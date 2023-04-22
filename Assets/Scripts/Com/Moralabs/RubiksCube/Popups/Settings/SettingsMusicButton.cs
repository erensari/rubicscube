using Com.Moralabs.RubiksCube.Util;
using Morautils.AudioSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Com.Moralabs.RubiksCube.Popups.Settings {
    public class SettingsMusicButton : MonoBehaviour {
        [SerializeField]
        private Image onOffImage;

        [Inject]
        private IMusicController musicController;
        [Inject]
        private ISoundController2d soundController;


        private void Awake() {
            onOffImage.GetComponent<Image>().enabled = !musicController.Enabled;
        }

        public void MusicOffButton() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            musicController.Enabled = !musicController.Enabled;
            onOffImage.GetComponent<Image>().enabled = !onOffImage.GetComponent<Image>().enabled;
        }
    }
}