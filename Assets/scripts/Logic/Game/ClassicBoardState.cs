#nullable enable

using System.Collections.Generic;
using UnityEngine;

public class ClassicBoardState {
    private readonly ChessGame _game;
    private List<int> _capturedPieces = new();
    private HashSet<int> _movedPieces = new();

    /// <summary>
    /// A dictionary where each key represents a position on the board, and
    /// each value represents the id of the piece in that position.
    /// </summary>
    public Dictionary<Vector2Int, int> Board { get; private set; } = new();

    /// <summary>
    /// A list with the ids of all pieces that have been captured in this board.
    /// </summary>
    public IReadOnlyList<int> CapturedPieces => _capturedPieces;
    /// <summary>
    /// A list with the ids of all pieces that have been moved during this game.
    /// </summary>
    public HashSet<int> MovedPieces => _movedPieces;

    /// <summary>
    /// The amount of identical boards represented by this board. For example,
    /// if a quantum board state had 4 boards, and 2 of them were the exact
    /// same, it could store only 3 boards, with one of them having this
    /// property set to '2' instead of '1' (meaning that it represents 2 
    /// identical boards).
    /// </summary>
    public long Weight { get; set; } = 1; // TODO: this can get arbitrarily big.

    public int this[int x, int y] {
        get => Board[new(x, y)];
        set => Board[new(x, y)] = value;
    }

    public int this[Vector2Int pos] {
        get => Board[pos];
        set => Board[pos] = value;
    }

    public ClassicBoardState (ChessGame game) {
        this._game = game;
    }

    public int? GetPieceAt (Vector2Int pos) {
        if (Board.TryGetValue(pos, out var piece)) {
            return piece;
        }

        return null;
    }

    public bool TryGetPieceAt (Vector2Int pos, out int piece) {
        return Board.TryGetValue(pos, out piece);
    }

    /// <summary>
    /// Creates a new classic board state that is an identical copy of this one.
    /// </summary>
    public ClassicBoardState Clone () {
        ClassicBoardState clone = new(_game);

        foreach (var kv in Board) {
            clone.Board[kv.Key] = kv.Value;
        }
        foreach (var id in _capturedPieces) {
            clone._capturedPieces.Add(id);
        }
        foreach (var id in _movedPieces) {
            clone._movedPieces.Add(id);
        }

        clone.Weight = Weight;

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
    /// <param name="type">The type of the piece moving.</param>
    /// <param name="origin">The cell where the piece supposedly is.</param>
    /// <param name="target">The cell where the piece should move.</param>
    public void MakeMoveIfAble (
        int pieceId, PieceType type, Vector2Int origin, Vector2Int target
    ) {
        // idk how you'd achieve this but, in case you do, the logic assumes
        // the piece at the target cell is not the piece you are moving, and
        // will break otherwise (e.g. the piece will capture itself).
        if (origin == target) return;

        // If there isn't any piece in the specified origin, or if the piece
        // in that cell is not the requested piece, then no move is made in
        // this board.
        if (IsAnyPieceAt(origin) == false || this[origin] != pieceId) {
            return;
        }

        //bool isLegalMove = true; // TODO: Actually check this.
        var legalMoves = Ruleset.GetAvailableMovesInBoard(
            _game, this, _game.GetPieceById(pieceId), origin
        );

        // if the move is illegal in this board,
        // then no move is made in this board.
        if (legalMoves.Contains(target) == false) return;

        // if there's a piece at the target, it'll be replaced by the piece
        // we are moving, so we send it to the list of captures.
        if (IsAnyPieceAt(target)) {
            _capturedPieces.Add(Board[target]);
        }

        // check en passant
        if (type == PieceType.Pawn
            && target.x != origin.x
            && IsAnyPieceAt(target) == false
            && IsAnyPieceAt(new(target.x, origin.y)) // this check should be necessary, since we know
                                                     // this move is available and thus MUST be en passant,
                                                     // but it's here for redundancy.
        ) {
            _capturedPieces.Add(Board[new(target.x, origin.y)]);
            Board.Remove(new(target.x, origin.y));
        }

        // check castling
        if (type == PieceType.King) {
            int cellsMoved = target.x - origin.x; // +n if moving towards the right, -n if moving towards the left.

            // we assume there's a castling if the king is moving more than 1
            // cell towards either direction, and if the leftmost / rightmost
            // cell has is not empty.
            // castling to the left.
            if (cellsMoved < -1 && TryGetPieceAt(new(0, target.y), out int rookId)) {
                Board.Remove(new(0, target.y)); // removed from its original place.
                Board[new(target.x + 1, target.y)] = rookId; // and placed to the king's right.
            }
            // castling to the right.
            int rightmost = _game.Width - 1;
            if (cellsMoved > 1 && TryGetPieceAt(new(rightmost, target.y), out rookId)) {
                Board.Remove(new(rightmost, target.y)); // removed from its original place.
                Board[new(target.x - 1, target.y)] = rookId; // and placed to the king's left.
            }
        }

        Board.Remove(origin);
        Board[target] = pieceId;
    }

    /// <summary>
    /// Returns true if the board given is identical 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsBoardIdentical (ClassicBoardState other) {
        // if they have a different number of pieces on board, then they are
        // not identical.
        if (Board.Keys.Count != other.Board.Keys.Count) return false;
        // if they don't have the same # of pieces captured (although this check
        // is redundant), then they are not identical.
        //if (_capturedPieces.Count != other._capturedPieces.Count) return false;

        // Note: we are only checking ALL pieces in other.Board. We are NOT
        // checking all pieces in this.Board (there could be a piece in this
        // board in a cell that is empty in the other board).
        // The trick is that, at this moment, we have verified that both boards
        // have the same number of pieces. Given this condition, if there is a
        // piece in this board that we are not checking, that means there's
        // a piece in the other board that, when checked, will match an empty
        // cell in this board and thus return false. If all cells in the other
        // board were identical to cells in this board, and this board still
        // had a piece unchecked, then both boards would necessarily have a
        // different amount of pieces and thus it would've failed in the check
        // above.
        // This also applies to captured pieces.

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

        // this check is redundant, it adds extra consistency in the event of
        // a bug that could remove a piece in a board without capturing it.
        //foreach (var pieceId in other._capturedPieces) {
        //    _capturedPieces.Contains(pieceId);
        //}

        return true;
    }
}
