using UnityEngine;

namespace Com.Moralabs.RubiksCube.LevelSystem {
    [CreateAssetMenu(fileName = "CategoryData", menuName = "ScriptableObjects/CategoryInfo")]
    public class CategoryData : ScriptableObject {
        public string category;
        public int numberOfLevels;
        public int categoryId;
    }
}
