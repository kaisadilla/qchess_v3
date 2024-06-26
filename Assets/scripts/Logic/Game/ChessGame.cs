#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChessGame {
    private readonly Dictionary<int, ClassicPiece> _pieces;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public QuantumBoardState CurrentState { get; private set; }

    /// <summary>
    /// A list of moves effectuated in this game, in order.
    /// </summary>
    public List<IMove> Moves { get; private set; } = new();

    public BoardMeaning Meaning => BoardMeaning.FromBoardState(
        CurrentState, Width, Height
    ); // TODO Optimize this so it's not calculated every time it's accessed, since we know exactly when it changes.

    public IMove LastMove => Moves[^1];

    #region Events
    public delegate void MoveEvent (object sender, MoveEventArgs evt);
    public event MoveEvent OnMove;
    #endregion

    public ChessGame (
        Dictionary<int, ClassicPiece> pieces,
        Dictionary<Vector2Int, int> startingCells
    ) {
        Width = Constants.DEFAULT_BOARD_WIDTH;
        Height = Constants.DEFAULT_BOARD_HEIGHT;
        _pieces = pieces;

        ClassicBoardState initialState = new(this);
        foreach (var kv in startingCells) {
            initialState[kv.Key] = kv.Value;
        }

        CurrentState = new(this, initialState);
    }

    /// <summary>
    /// Returns true if the position given exists inside the board for this
    /// game.
    /// </summary>
    /// <param name="pos">The position to check.</param>
    public bool DoesPositionExist (Vector2Int pos) {
        return pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
    }

    public ClassicPiece GetPieceById (int id) {
        return _pieces[id];
    }

    /// <summary>
    /// Tries to execute the classic move given, if that move is determined
    /// to be a legal one. Returns true if the move is successfully executed.
    /// Note that a piece may be in more than one position at the same time,
    /// so this method requires the caller to specify which origin the piece
    /// is being moved from.
    /// </summary>
    /// <param name="pieceId">The id of the piece to move.</param>
    /// <param name="origin">The position the piece is currently in.</param>
    /// <param name="target">The position the piece will be moved to.</param>
    public bool TryClassicMove (
        RealPiece piece, Vector2Int target
    ) {
        var availableMoves = Ruleset.GetAvailableMoves(
            this, piece, false
        );

        // this move is not legal.
        if (availableMoves.Any(pos => pos == target) == false) {
            Debug.LogWarning("Illegal move attempted.");
            return false;
        }

        bool isCastling = false;

        ClassicMove move = new(
            piece.ClassicPiece.PlayerId, piece.ClassicId, piece,
            piece.Position, target, isCastling, Meaning[target]
        );
        CurrentState.MakeClassicMove(move);
        OnMove(this, new(move));

        Moves.Add(move);
        return true;
    }

    public bool TryQuantumMove (
        RealPiece piece, List<Vector2Int> targets
    ) {
        var availableMoves = Ruleset.GetAvailableMoves(
            this, piece, true
        );

        // this move is not legal. Should not happen because we are only offering
        // the player legal moves to choose.
        if (availableMoves.Any(pos => targets.Contains(pos)) == false) {
            Debug.LogWarning("Illegal move attempted.");
            return false;
        }

        QuantumMove move = new(
            piece.ClassicPiece.PlayerId, piece.ClassicId, piece,
            piece.Position, targets
        );
        CurrentState.MakeQuantumMove(move);
        OnMove(this, new(move));

        Moves.Add(move);
        return true;
    }
}

public class MoveEventArgs : EventArgs {
    public IMove Move { get; private set; }

    public MoveEventArgs (IMove move) {
        Move = move;
    }
}