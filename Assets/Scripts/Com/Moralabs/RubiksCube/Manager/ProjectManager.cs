using Com.Moralabs.RubiksCube.LevelSystem;
using Morautils.LanguageSystem;
using UnityEngine.SceneManagement;
using UniRx;
using Morautils.AudioSystem;
using Com.Moralabs.RubiksCube.Util;

namespace Com.Moralabs.RubiksCube.Manager {
    public class ProjectManager {
        private readonly LevelController levelController;
        private readonly ILanguageController languageController;
        private readonly IMusicController musicController;
        private readonly ISoundController2d soundController;
        private readonly ReactiveProperty<bool> isLoaded = new ReactiveProperty<bool>(false);
        public IReadOnlyReactiveProperty<bool> IsLoaded => isLoaded;

        public string initialCategory = "Paintings";
        public int initialLevel = 1;
        public int totalCategoryNumber = 4;
        public bool anySavedPositions = false;

        public ProjectManager(LevelController levelController, ILanguageController languageController, IMusicController musicController, ISoundController2d soundController) {
            this.levelController = levelController;
            this.languageController = languageController;
            this.musicController = musicController;
            this.soundController = soundController;

            languageController.IsLanguageReady.Subscribe(OnLanguageLoaded);
            musicController.IsLoaded.Subscribe(OnMusicLoaded);
        }
        public LevelData ActiveLevelData {
            get;
            set;
        }
        public CategoryData ActiveCategoryData {
            get;
            set;
        }

        private void OnMusicLoaded(bool loaded) {
            if (loaded) {
                musicController.PlayMusic(musics: new Sound[] { new Sound(Sounds.MUSIC), new Sound(Sounds.MUSIC2), 
                    new Sound(Sounds.MUSIC3), new Sound(Sounds.MUSIC4)});         
            }
            IsAllLoaded();
        }

        private void OnLanguageLoaded(bool loaded) {
            if (loaded) {
            }
            IsAllLoaded();
        }

        private void IsAllLoaded() {
            isLoaded.Value = languageController.IsLanguageReady.Value &&
                 musicController.IsLoaded.Value &&
                 soundController.IsLoaded.Value;
        }

        public void PlayLevel(int level) {
            ActiveLevelData = levelController.GetLevelData(ActiveCategoryData.category, level);
            anySavedPositions = false;
            SceneManager.LoadSceneAsync(Constants.GAME_SCENE);
        }
        public void PlayCategory(string categoryName) {
            ActiveCategoryData = levelController.GetCategoryData(categoryName);
            anySavedPositions = false;
            SceneManager.LoadSceneAsync(Constants.LEVEL_SCENE);
        }
        public void PlayCategoryAndLevel(string category, int level) {
            ActiveCategoryData = levelController.GetCategoryData(category);
            ActiveLevelData = levelController.GetLevelData(category, level);
            anySavedPositions = false;
            SceneManager.LoadSceneAsync(Constants.GAME_SCENE);
        }
        public void PlayCategoryAndLevel(string category, int level, bool anySavedPositions) {
            ActiveCategoryData = levelController.GetCategoryData(category);
            ActiveLevelData = levelController.GetLevelData(category, level);
            this.anySavedPositions = anySavedPositions;
            SceneManager.LoadSceneAsync(Constants.GAME_SCENE);
        }
    }
}