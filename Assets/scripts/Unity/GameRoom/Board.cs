#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    [Header("Prefabs")]
    [SerializeField] private PieceIcon piecePrefab;
    [Header("Components")]
    [SerializeField] private GameRoom _room;
    [SerializeField] private BoardGrid _grid;

    public void DrawBoard (PieceStyle style, BoardMeaning board) {
        _grid.Clear();

        for (int y = 0; y < _room.Game.Height; y++) {
            for (int x = 0; x < _room.Game.Width; x++) {
                var pieces = board[x, y];
                DrawPieces(style, pieces, new(x, y));
            }
        }
    }

    public void DrawPieces (
        PieceStyle style, List<RealPiece> pieces, Vector2Int cell
    ) {
        if (pieces.Count > 4) throw new System.Exception(
            "There can't be more than 4 pieces in a single cell."
        );

        for (int i = 0; i < pieces.Count; i++) {
            RealPiece piece = pieces[i];
            var icon = Instantiate(piecePrefab);
            icon.Initialize(piece, style);

            _grid.PlaceIntoGrid(icon.transform, cell, pieces.Count != 1, i);
        }
    }
}
