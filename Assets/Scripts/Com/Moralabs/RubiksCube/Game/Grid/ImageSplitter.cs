using UnityEngine;
namespace Com.Moralabs.RubiksCube.Game.Grid {
    public static class ImageSplitter {
        public static Sprite[,] SplitImage(Sprite mainSprite, int rows, int columns) {
            Sprite[,] spritePieces;
            Texture2D texture = mainSprite.texture;
            float pieceWidth = texture.width / columns;
            float pieceHeight = texture.height / rows;
            spritePieces = new Sprite[columns, rows];
            for (int y = 0; y < rows; y++) {
                for (int x = 0; x < columns; x++) {
                    Rect rect = new Rect(x * pieceWidth, y * pieceHeight, pieceWidth, pieceHeight);
                    Vector2 pivot = new Vector2(0.5f, 0.5f);
                    spritePieces[x, y] = Sprite.Create(texture, rect, pivot);
                }
            }
            return spritePieces;
        }
    }
}
