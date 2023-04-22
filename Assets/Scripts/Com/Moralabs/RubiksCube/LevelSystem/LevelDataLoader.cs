using UnityEngine;

namespace Com.Moralabs.RubiksCube.LevelSystem {
    public class LevelDataLoader : MonoBehaviour {
        public static LevelData LoadLevelData(string category, int level) {
            return Resources.Load<LevelData>("LevelData/" + category + "/" + level);
        }
    }
}