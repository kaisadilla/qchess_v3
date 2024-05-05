#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessGame {
    private readonly Dictionary<int, ClassicPiece> _pieces;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public QuantumBoardState CurrentState { get; private set; }
    public BoardMeaning Meaning => BoardMeaning.FromBoardState(
        CurrentState, Width, Height
    );

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

        ClassicBoardState initialState = new();
        foreach (var kv in startingCells) {
            initialState[kv.Key] = kv.Value;
        }

        CurrentState = new(this, initialState);
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
        int pieceId, Vector2Int origin, Vector2Int target
    ) {
        var piece = _pieces[pieceId];
        var availableMoves = Ruleset.GetAvailableMoves(
            this, origin, piece.Type, false
        );

        // this move is not legal.
        if (availableMoves.Any(pos => pos == target) == false) {
            Debug.LogWarning("Illegal move attempted.");
            return false;
        }

        bool isCastling = false;

        var move = new ClassicMove(
            piece.PlayerId, piece.Id, origin, target, isCastling
        );
        CurrentState.MakeClassicMove(move);
        OnMove(this, new(move));

        return true;
    }
}

public class MoveEventArgs : EventArgs {
    public IMove Move { get; private set; }

    public MoveEventArgs (IMove move) {
        Move = move;
    }
}