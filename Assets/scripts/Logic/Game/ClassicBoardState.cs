#nullable enable

using System.Collections.Generic;
using UnityEngine;

public class ClassicBoardState {
    // key represents position, value represents piece id.
    public Dictionary<Vector2Int, int> Board { get; private set; } = new();
    /// <summary>
    /// The amount of identical boards represented by this board. For example,
    /// if a quantum board state had 4 boards, and 2 of them were the exact
    /// same, it could store only 3 boards, with one of them having this
    /// property set to '2' instead of '1' (meaning that it represents 2 
    /// identical boards).
    /// </summary>
    public long Multiplier { get; set; } = 1; // TODO: this can get arbitrarily big.

    public int this[int x, int y] {
        get => Board[new(x, y)];
        set => Board[new(x, y)] = value;
    }

    public int this[Vector2Int pos] {
        get => Board[pos];
        set => Board[pos] = value;
    }

    /// <summary>
    /// Creates a new classic board state that is an identical copy of this one.
    /// </summary>
    public ClassicBoardState Clone () {
        ClassicBoardState clone = new();

        foreach (var kv in Board) {
            clone.Board[kv.Key] = kv.Value;
        }

        clone.Multiplier = Multiplier;

        return clone;
    }

    /// <summary>
    /// Returns true if there's a piece at the position given, or false otherwise.
    /// </summary>
    /// <param name="pos">The position to check.</param>
    /// <returns></returns>
    public bool IsAnyPieceAt (Vector2Int pos) {
        return Board.ContainsKey(pos);
    }

    /// <summary>
    /// Makes the move described (moving a specific piece from one specific
    /// place to its origin) if such move is posible. The move will be possible
    /// if the requested piece is in the cell determined to be its origin,
    /// and if such move would be legal in this specific board.
    /// </summary>
    /// <param name="pieceId">The id of the piece to move.</param>
    /// <param name="origin">The cell where the piece supposedly is.</param>
    /// <param name="target">The cell where the piece should move.</param>
    public void MakeMoveIfAble (int pieceId, Vector2Int origin, Vector2Int target) {
        // If there isn't any piece in the specified origin, or if the piece
        // in that cell is not the requested piece, then no move is made in
        // this board.
        if (IsAnyPieceAt(origin) == false || this[origin] != pieceId) {
            return;
        }

        bool isLegalMove = true; // TODO: Actually check this.

        // if the move is illegal, then no move is made in this board.
        if (isLegalMove == false) return;

        Board.Remove(origin);
        Board[target] = pieceId;
    }

    /// <summary>
    /// Returns true if the board given is identical 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsBoardIdentical (ClassicBoardState other) {
        // for each piece in the other board.
        foreach (var otherKv in other.Board) {
            // if one board has a piece in a cell that is empty in the other
            // board, then they are not identical.
            if (IsAnyPieceAt(otherKv.Key) == false) {
                return false;
            }
            // if the piece at this position is different in the two boards,
            // then they're not identical.
            if (Board[otherKv.Key] != otherKv.Value) {
                return false;
            }
        }

        return true;
    }
}
