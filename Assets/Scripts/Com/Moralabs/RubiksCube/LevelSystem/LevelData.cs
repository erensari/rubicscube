using UnityEngine;

namespace Com.Moralabs.RubiksCube.LevelSystem {
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelInfo")]
    public class LevelData : ScriptableObject {
        public int currentLevel;
        public string imageName;
        public int width;
        public int height;
        public string category;
    }
}
