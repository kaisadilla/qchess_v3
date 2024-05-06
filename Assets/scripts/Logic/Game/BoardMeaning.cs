#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// An object that describes the pieces (classic and quantum) that exist in
/// each cell of the board. This is the representation of the quantum board
/// that the player sees.
/// </summary>
public class BoardMeaning {
    private readonly List<RealPiece>[] _board;
    private readonly List<RealPiece> _capturedPieces;

    private readonly int _boardWidth, _boardHeight;

    /// <summary>
    /// A list with all the pieces that have been captured in this board.
    /// </summary>
    public IReadOnlyList<RealPiece> CapturedPieces => _capturedPieces;

    /// <summary>
    /// Returns a list of pieces at the cell given.
    /// </summary>
    public List<RealPiece> this[int x, int y] {
        get => _board[IndexFromPos(x, y)];
    }

    /// <summary>
    /// Returns a list of pieces at the cell given.
    /// </summary>
    public List<RealPiece> this[Vector2Int pos] {
        get => this[pos.x, pos.y];
    }

    public BoardMeaning (int boardWidth, int boardHeight) {
        _boardWidth = boardWidth;
        _boardHeight = boardHeight;
        _board = new List<RealPiece>[boardWidth * boardHeight];
        _capturedPieces = new();
    }

    /// <summary>
    /// Obtains the meaning of the board state given.
    /// </summary>
    /// <param name="state">The quantum board state to represent.</param>
    public static BoardMeaning FromBoardState (
        QuantumBoardState state, int width, int height
    ) {
        BoardMeaning meaning = new(width, height);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                Vector2Int pos = new(x, y);
                int index = meaning.IndexFromPos(pos);
                meaning._board[index] = state.GetPiecesAtPos(pos);
            }
        }

        meaning._capturedPieces.AddRange(
            state.GetCapturedPieces()
        );

        return meaning;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexFromPos (Vector2Int pos) {
        return IndexFromPos(pos.x, pos.y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexFromPos (int x, int y) {
        return (y * _boardWidth) + x;
    }
}
