using UnityEngine;

namespace Com.Moralabs.RubiksCube.LevelSystem {

    public class CategoryDataLoader : MonoBehaviour {
        public static CategoryData[] LoadCategoryData() {
            return Resources.LoadAll<CategoryData>("CategoryData/");
        }
    }
}