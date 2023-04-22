using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Moralabs.RubiksCube.Game.Grid {
    [Serializable]
    public class BlockData  {
        public List<Vector2Int> lastPositions;
        public List<Vector2Int> initialPositions;
        public List<List<Vector2Int>> previousMoves = new List<List<Vector2Int>>();
        public List<List<Vector2Int>> nextMoves = new List<List<Vector2Int>>();
        public string category;
        public int level;
        public float time;


        public BlockData() {

        }

        public BlockData(List<Vector2Int> lastPositions, string category, int level, float time, 
            List<List<Vector2Int>> previousMoves, List<Vector2Int> initialPositions, List<List<Vector2Int>> nextMoves) {
            this.lastPositions = lastPositions;
            this.category = category;
            this.level = level;
            this.time = time;
            this.previousMoves = previousMoves;
            this.initialPositions = initialPositions;
            this.nextMoves = nextMoves;
        }
    }
}