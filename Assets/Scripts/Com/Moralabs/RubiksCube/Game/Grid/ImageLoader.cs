using System;
using UnityEngine;

namespace Com.Moralabs.RubiksCube.Game.Grid {
    public class ImageLoader : MonoBehaviour {
        public static Sprite LoadImage(String imageName, String category) {
            return Resources.Load<Sprite>("LevelImages/" + category + "/"+ imageName);
        }
    }
}