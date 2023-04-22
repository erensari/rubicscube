using Com.Moralabs.RubiksCube.Util;
using Morautils.AudioSystem;
using Morautils.LanguageSystem;
using Morautils.PopupSystem;
using TMPro;
using UnityEngine.SceneManagement;
using Zenject;

namespace Com.Moralabs.RubiksCube.Popups.Settings {
    public class SettingsPopup : BasePopup {

        public TMP_Text languageLabel;
        private int selectedLanguage;

        [Inject]
        private ILanguageController languageController;
        [Inject]
        private ISoundController2d soundController;

       
        public void LanguageLeftButton() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            selectedLanguage--;
            var languages = languageController.LanguageConfig.Languages;
            if (selectedLanguage < 0) {
                selectedLanguage = languages.Count - 1;
            }
            UpdateLanguage();
        }
        public void LanguageRightButton() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            selectedLanguage++;

            var languages = languageController.LanguageConfig.Languages;

            if (selectedLanguage > languages.Count - 1) {
                selectedLanguage = 0;
            }
            UpdateLanguage();
        }
        public void UpdateLanguage() {
            var languages = languageController.LanguageConfig.Languages;
            languageLabel.text = languageController.GetText(languages[selectedLanguage].ToIsoString());
            languageController.SetLanguage(languages[selectedLanguage]);
        }
        public void BackButton() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            SceneManager.LoadSceneAsync(Constants.DASHBOARD_SCENE);
        }
    }
}
