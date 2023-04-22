using System.Collections.Generic;
using System.Linq;
using Com.Moralabs.RubiksCube.Util;
using Morautils.JsonHelper;
using UnityEngine;

namespace Com.Moralabs.RubiksCube.LevelSystem {

    public class LevelController {

        private List<LevelCompletionTime> completedLevelList;

        private CategoryData[] categories;
        private readonly Dictionary<(string category, int level), LevelData> leveldatas = new Dictionary<(string category, int level), LevelData>();

        public LevelController() {
            categories = CategoryDataLoader.LoadCategoryData();
        }

        public CategoryData GetCategoriesWithId(int id) {
            return categories.FirstOrDefault(x => x.categoryId == id);
        }

        public CategoryData[] GetCategoryData() {
            return categories;
        }

        public CategoryData GetCategoryData(string category) {
            return categories.FirstOrDefault(x => x.category == category);
        }

        public LevelData GetLevelData(string category, int level) {
            if (!leveldatas.TryGetValue((category, level), out LevelData data)) {
                data = LevelDataLoader.LoadLevelData(category, level);
                leveldatas.Add((category, level), data);
            }
            return data;
        }

        public List<LevelCompletionTime> CompletedLevels {
            get {
                if (completedLevelList == null) {
                    string str = PlayerPrefs.GetString(Constants.COMPLETED_LEVELS, null);
                    if (string.IsNullOrEmpty(str)) {
                        completedLevelList = new List<LevelCompletionTime>();
                        str = completedLevelList.ToJson();
                        PlayerPrefs.SetString(Constants.COMPLETED_LEVELS, str);
                    }
                    else {
                        completedLevelList = str.ParseJson<List<LevelCompletionTime>>();
                    }
                }
                return completedLevelList;
            }
        }

        public void CompleteLevel(int level, string category, float time, string completedTime) {

            if (CompletedLevels.Any(x => x.Level.Equals(level) && x.Category.Equals(category) && x.Time > time)) {
                foreach (var item in completedLevelList.Where(x => x.Level.Equals(level) && x.Category.Equals(category))) {
                    item.Time = time;
                    item.LevelCompletiondTime = completedTime;
                }
                string str2 = completedLevelList.ToJson();
                PlayerPrefs.SetString(Constants.COMPLETED_LEVELS, str2);
                return;
            }
            else if (CompletedLevels.Any(x => x.Level.Equals(level) && x.Category.Equals(category) && x.Time < time)) {
                return;
            }

            LevelCompletionTime levelCompletionTime = new LevelCompletionTime();
            levelCompletionTime.Level = level;
            levelCompletionTime.Category = category;
            levelCompletionTime.Time = time;
            levelCompletionTime.LevelCompletiondTime = completedTime;
            CompletedLevels.Add(levelCompletionTime);
            string str = completedLevelList.ToJson();
            PlayerPrefs.SetString(Constants.COMPLETED_LEVELS, str);
        }

        public string LevelCompletionTime(string category, int level) {
            foreach (var item in completedLevelList.Where(x => x.Level.Equals(level) && x.Category.Equals(category))) {
                return item.LevelCompletiondTime.ToString();
            }
            return null;
        }

        public bool IsLocked(int level, string category) {
            if (level == 0) {
                return true;
            }
            return CompletedLevels.Any(x => x.Level == level - 1 && x.Category.Equals(category));
        }

        public int LastCompletedLevel(string category) {
            return CompletedLevels.Count(x => x.Category.Equals(category));
        }
    }
}
