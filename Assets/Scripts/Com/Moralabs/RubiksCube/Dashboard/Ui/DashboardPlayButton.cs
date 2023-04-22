using Com.Moralabs.RubiksCube.LevelSystem;
using Com.Moralabs.RubiksCube.Manager;
using Com.Moralabs.RubiksCube.Util;
using Morautils.AudioSystem;
using Morautils.LanguageSystem;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Com.Moralabs.RubiksCube.DashboardScene.Ui {
    public class DashboardPlayButton : MonoBehaviour {
        [SerializeField]
        private TMP_Text playContinueText;
        [SerializeField]
        private LanguageTMP languageTMP;

        [Inject]
        private ProjectManager projectManager;
        [Inject]
        private LevelController levelController;
        [Inject]
        private ISoundController2d soundController;


        private void OnEnable() {
            if (levelController.CompletedLevels.Count > 0) {
                playContinueText.fontSize = 60;
                languageTMP.SetTag("Continue");


            }
        }
        private void Start() {
            projectManager.IsLoaded.Subscribe(IsAllLoaded);

        }

        private void IsAllLoaded(bool loaded) {
            if (loaded) {
                Debug.Log("ALL LOADED");
            }
        }


        public void PlayButton() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            PlayButtonAux();
        }

        private void PlayButtonAux() {
            string lastActiveCategoryLevel = PlayerPrefs.GetString(Constants.LAST_ACTIVE_CATEGORY_LEVEL);

            if (string.IsNullOrEmpty(lastActiveCategoryLevel)) {
                projectManager.PlayCategoryAndLevel(projectManager.initialCategory, projectManager.initialLevel);
                return;
            }

            // Kaydedilmis son aktif kategori ve seviye bilgisi ayristirilir
            string savedCategory = GetCategoryFromSavedString(lastActiveCategoryLevel);
            int savedLevel = GetLevelFromSavedString(lastActiveCategoryLevel);

            // Onceden tamamlanmis seviye tekrardan oynandiginda tamamlanip tamamlanmadigini kontrol etmek icin
            bool isActiveLevelCompleted = PlayerPrefs.GetInt(Constants.IS_ACTIVE_LEVEL_COMPLETED) == 1 ? true : false;
            //bool isLastSavedLevelCompleted = isActiveLevelCompleted && levelController.LevelCompletionTime(savedCategory, savedLevel) != null;

            if (isActiveLevelCompleted) {
                bool isLastLevelOfLastCategory = savedCategory == levelController.GetCategoriesWithId(projectManager.totalCategoryNumber).category
                    && savedLevel == levelController.GetCategoriesWithId(projectManager.totalCategoryNumber).numberOfLevels;

                if (isLastLevelOfLastCategory) {
                    // Son kategorinin son seviyesi tamamlandiysa, oyun baslangicina dönülür
                    projectManager.PlayCategoryAndLevel(projectManager.initialCategory, projectManager.initialLevel);
                }
                else {
                    // Sonraki kategoriye gecer
                    if (savedLevel == levelController.GetCategoriesWithId(projectManager.totalCategoryNumber).numberOfLevels) {
                        projectManager.PlayCategoryAndLevel(levelController.GetCategoriesWithId(projectManager.ActiveCategoryData.categoryId + 1).category, 1);
                    }
                    else {
                        // Ayni kategorinin bir sonraki seviyesine geçilir
                        projectManager.PlayCategoryAndLevel(savedCategory, savedLevel + 1);
                    }

                }
            }
            else {
                //Tutorial olan seviyelerde kayit yok, her zaman bastan baslar.
                if (savedCategory.Equals(levelController.GetCategoriesWithId(1).category) && (savedLevel == 1 || savedLevel == 4) ) {
                    projectManager.PlayCategoryAndLevel(savedCategory, savedLevel);
                }
                else {
                    // Kaydedilmis son seviyede oyun devam ettirilir
                    projectManager.PlayCategoryAndLevel(savedCategory, savedLevel, true);
                }
 
            }

        }

        // Oyun son durumundan kategori ve seviye bilgisini ayrýþtýran yardýmcý metotlar
        private string GetCategoryFromSavedString(string savedString) {
            int index = savedString.IndexOf('/');
            return savedString.Substring(0, index);
        }

        private int GetLevelFromSavedString(string savedString) {
            int index = savedString.IndexOf('/') + 1;
            return int.Parse(savedString.Substring(index));
        }

    }
}
