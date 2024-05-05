#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessGame {
    private readonly Dictionary<int, ClassicPiece> _pieces;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public QuantumBoardState CurrentState { get; private set; }
    public BoardMeaning Meaning => BoardMeaning.FromBoardState(
        CurrentState, Width, Height
    );

    public ChessGame (
        Dictionary<int, ClassicPiece> pieces,
        Dictionary<Vector2Int, int> startingCells
    ) {
        Width = Constants.DEFAULT_BOARD_WIDTH;
        Height = Constants.DEFAULT_BOARD_HEIGHT;
        _pieces = pieces;

        ClassicBoardState initialState = new();
        foreach (var kv in startingCells) {
            initialState[kv.Key] = kv.Value;
        }

        CurrentState = new(this, initialState);
    }

    public ClassicPiece GetPieceById (int id) {
        return _pieces[id];
    }
}
