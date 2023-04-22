using Com.Moralabs.RubiksCube.Util;
using Morautils.AudioSystem;
using Morautils.PopupSystem;
using UnityEngine;
using Zenject;

namespace Com.Moralabs.RubiksCube.DashboardScene.Ui {

    public class DashboardSettingsButton : MonoBehaviour {

        [Inject]
        private ISoundController2d soundController;
        [Inject]
        private IPopupController popupController;

        public void SettingsButton() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));

            popupController.OpenTop(Constants.SETTINGS_POPUP);
        }

    }
}