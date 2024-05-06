#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ruleset {
    public static bool CanPieceQuantumMove (PieceType type) {
        return type != PieceType.Pawn;
    }

    /// <summary>
    /// Calculates all the cells to which a piece of a certain type, in a
    /// certain cell, can move.
    /// </summary>
    /// <param name="game">The game this move would be effected in.</param>
    /// <param name="origin">The cell in which the piece exists.</param>
    /// <param name="type">The type of the piece to move.</param>
    /// <param name="isQuantumMove">True if this is a quantum move.</param>
    /// <returns></returns>
    public static List<Vector2Int> GetAvailableMoves (
        ChessGame game,
        RealPiece piece,
        bool isQuantumMove
    ) {
        if (piece.ClassicPiece.Type == PieceType.Pawn) {
            return new(GetAvailablePawnMoves(game, piece, isQuantumMove));
        }
        if (piece.ClassicPiece.Type == PieceType.Rook) {
            return new(GetAvailableRookMoves(game, piece, isQuantumMove));
        }
        if (piece.ClassicPiece.Type == PieceType.Bishop) {
            return new(GetAvailableBishopMoves(game, piece, isQuantumMove));
        }
        if (piece.ClassicPiece.Type == PieceType.Knight) {
            return new(GetAvailableKnightMoves(game, piece, isQuantumMove));
        }
        if (piece.ClassicPiece.Type == PieceType.Queen) {
            var rookMoves = GetAvailableRookMoves(game, piece, isQuantumMove);
            var bishopMoves = GetAvailableBishopMoves(game, piece, isQuantumMove);

            rookMoves.UnionWith(bishopMoves);
            return new(rookMoves);
        }

        return new();
    }

    /// <summary>
    /// Calculates all the cells a piece can move, in a specific classical board.
    /// </summary>
    /// <param name="board">The classical board in which the move occurs.</param>
    /// <param name="piece">The piece to move.</param>
    public static List<Vector2Int> GetAvailableMovesInBoard (
        ChessGame game, ClassicBoardState board, ClassicPiece piece, Vector2Int origin
    ) {
        if (piece.Type == PieceType.Pawn) {
            return new(GetAvailablePawnMovesInBoard(
                game, board, piece.PlayerId, origin, false
            ));
        }
        if (piece.Type == PieceType.Rook) {
            return new(GetAvailableRookMovesInBoard(
                game, board, piece.PlayerId, origin, false
            ));
        }
        if (piece.Type == PieceType.Bishop) {
            return new(GetAvailableBishopMovesInBoard(
                game, board, piece.PlayerId, origin, false
            ));
        }
        if (piece.Type == PieceType.Knight) {
            return new(GetAvailableKnightMovesInBoard(
                game, board, piece.PlayerId, origin, false
            ));
        }
        if (piece.Type == PieceType.Queen) {
            var rookMoves = GetAvailableRookMovesInBoard(
                game, board, piece.PlayerId, origin, false
            );
            var bishopMoves = GetAvailableBishopMovesInBoard(
                game, board, piece.PlayerId, origin, false
            );

            rookMoves.UnionWith(bishopMoves);
            return new(rookMoves);
        }

        return new();
    }

    private static HashSet<Vector2Int> GetAvailablePawnMoves (
        ChessGame game,
        RealPiece piece,
        bool isQuantumMove
    ) {
        HashSet<Vector2Int> availableMoves = new();

        foreach (var state in game.CurrentState.ClassicStates) {
            var moves = GetAvailablePawnMovesInBoard(
                game, state, piece.ClassicPiece.PlayerId, piece.Position, isQuantumMove
            );
            availableMoves.UnionWith(moves); // TODO: Check this is the most efficient way to merge both sets.
        }

        return availableMoves;
    }

    private static HashSet<Vector2Int> GetAvailableRookMoves (
        ChessGame game,
        RealPiece piece,
        bool isQuantumMove
    ) {
        HashSet<Vector2Int> availableMoves = new();

        foreach (var state in game.CurrentState.ClassicStates) {
            var moves = GetAvailableRookMovesInBoard(
                game, state, piece.ClassicPiece.PlayerId, piece.Position, isQuantumMove
            );
            availableMoves.UnionWith(moves);
        }

        return availableMoves;
    }

    private static HashSet<Vector2Int> GetAvailableBishopMoves (
        ChessGame game,
        RealPiece piece,
        bool isQuantumMove
    ) {
        HashSet<Vector2Int> availableMoves = new();

        foreach (var state in game.CurrentState.ClassicStates) {
            var moves = GetAvailableBishopMovesInBoard(
                game, state, piece.ClassicPiece.PlayerId, piece.Position, isQuantumMove
            );
            availableMoves.UnionWith(moves);
        }

        return availableMoves;
    }

    private static HashSet<Vector2Int> GetAvailableKnightMoves (
        ChessGame game,
        RealPiece piece,
        bool isQuantumMove
    ) {
        HashSet<Vector2Int> availableMoves = new();

        foreach (var state in game.CurrentState.ClassicStates) {
            var moves = GetAvailableKnightMovesInBoard(
                game, state, piece.ClassicPiece.PlayerId, piece.Position, isQuantumMove
            );
            availableMoves.UnionWith(moves);
        }

        return availableMoves;
    }

    private static HashSet<Vector2Int> GetAvailablePawnMovesInBoard (
        ChessGame game,
        ClassicBoardState board,
        int playerId,
        Vector2Int pos,
        bool isQuantumMove
    ) {
        HashSet<Vector2Int> availableMoves = new();

        int forwardVal = playerId == 0 ? 1 : -1;

        // the cell ahead.
        Vector2Int potentialMove = pos + new Vector2Int(0, forwardVal);
        bool isPieceInFront = board.IsAnyPieceAt(potentialMove);
        if (game.DoesPositionExist(potentialMove) && isPieceInFront == false) {
            availableMoves.Add(potentialMove);
        }

        // double move, only available when the pawn is in its starting row.
        if (pos.y == (playerId == 0 ? 1 : 6)) {
            var potentialSuperMove = pos + new Vector2Int(0, forwardVal + forwardVal);

            // for the jump forward to be possible, the cell between the origin
            // and the destination must also be empty.
            if (game.DoesPositionExist(potentialSuperMove)
                && isPieceInFront == false
                && board.IsAnyPieceAt(potentialSuperMove) == false
            ) {
                availableMoves.Add(potentialSuperMove);
            }

        }

        // the cell ahead to the left, available when the pawn can capture a piece there.
        potentialMove = pos + new Vector2Int(-1, forwardVal);
        if (game.DoesPositionExist(potentialMove) && board.IsAnyPieceAt(potentialMove)) {
            // can only capture enemy pieces.
            if (IsPieceAtPosAnEnemy(game, board, potentialMove, playerId)) {
                availableMoves.Add(potentialMove);
            }
        }

        // same but capture right.
        potentialMove = pos + new Vector2Int(1, forwardVal);
        if (game.DoesPositionExist(potentialMove) && board.IsAnyPieceAt(potentialMove)) {
            // can only capture enemy pieces.
            if (IsPieceAtPosAnEnemy(game, board, potentialMove, playerId)) {
                availableMoves.Add(potentialMove);
            }
        }

        // TODO: En passant.

        return availableMoves;
    }

    private static HashSet<Vector2Int> GetAvailableRookMovesInBoard (
        ChessGame game,
        ClassicBoardState board,
        int playerId,
        Vector2Int pos,
        bool isQuantumMove
    ) {
        HashSet<Vector2Int> availableMoves = new();
        Vector2Int potentialMove;

        // moving down (0).
        for (int y = pos.y - 1; y >= 0; y--) {
            potentialMove = new(pos.x, y);

            if (board.IsAnyPieceAt(potentialMove)) {
                if (IsPieceAtPosAnEnemy(game, board, potentialMove, playerId)) {
                    availableMoves.Add(potentialMove);
                }
                break; // we cannot move past this cell.
            }
            else {
                availableMoves.Add(potentialMove);
            }
        }
        // moving up (max).
        for (int y = pos.y + 1; y < game.Height; y++) {
            potentialMove = new(pos.x, y);

            if (board.IsAnyPieceAt(potentialMove)) {
                if (IsPieceAtPosAnEnemy(game, board, potentialMove, playerId)) {
                    availableMoves.Add(potentialMove);
                }
                break; // we cannot move past this cell.
            }
            else {
                availableMoves.Add(potentialMove);
            }
        }
        // moving left (0).
        for (int x = pos.x - 1; x >= 0; x--) {
            potentialMove = new(x, pos.y);

            if (board.IsAnyPieceAt(potentialMove)) {
                if (IsPieceAtPosAnEnemy(game, board, potentialMove, playerId)) {
                    availableMoves.Add(potentialMove);
                }
                break; // we cannot move past this cell.
            }
            else {
                availableMoves.Add(potentialMove);
            }
        }
        // moving right (max).
        for (int x = pos.x + 1; x < game.Width; x++) {
            potentialMove = new(x, pos.y);

            if (board.IsAnyPieceAt(potentialMove)) {
                if (IsPieceAtPosAnEnemy(game, board, potentialMove, playerId)) {
                    availableMoves.Add(potentialMove);
                }
                break; // we cannot move past this cell.
            }
            else {
                availableMoves.Add(potentialMove);
            }
        }

        return availableMoves;
    }

    private static HashSet<Vector2Int> GetAvailableBishopMovesInBoard (
        ChessGame game,
        ClassicBoardState board,
        int playerId,
        Vector2Int pos,
        bool isQuantumMove
    ) {
        HashSet<Vector2Int> availableMoves = new();
        Vector2Int potentialMove;

        int x, y;

        // moving towards bottom-left (0, 0) corner
        x = pos.x - 1;
        y = pos.y - 1;
        while (x >= 0 && y >= 0) {
            potentialMove = new(x, y);

            if (board.IsAnyPieceAt(potentialMove)) {
                if (IsPieceAtPosAnEnemy(game, board, potentialMove, playerId)) {
                    availableMoves.Add(potentialMove);
                }
                break; // we cannot move past this cell.
            }
            else {
                availableMoves.Add(potentialMove);
            }

            x--;
            y--;
        }

        // moving towards bottom-right (max, 0) corner
        x = pos.x + 1;
        y = pos.y - 1;
        while (x < game.Width && y >= 0) {
            potentialMove = new(x, y);

            if (board.IsAnyPieceAt(potentialMove)) {
                if (IsPieceAtPosAnEnemy(game, board, potentialMove, playerId)) {
                    availableMoves.Add(potentialMove);
                }
                break; // we cannot move past this cell.
            }
            else {
                availableMoves.Add(potentialMove);
            }

            x++;
            y--;
        }

        // moving towards top-left (0, max) corner
        x = pos.x - 1;
        y = pos.y + 1;
        while (x >= 0 && y < game.Height) {
            potentialMove = new(x, y);

            if (board.IsAnyPieceAt(potentialMove)) {
                if (IsPieceAtPosAnEnemy(game, board, potentialMove, playerId)) {
                    availableMoves.Add(potentialMove);
                }
                break; // we cannot move past this cell.
            }
            else {
                availableMoves.Add(potentialMove);
            }

            x--;
            y++;
        }

        // moving towards top-left (max, max) corner
        x = pos.x + 1;
        y = pos.y + 1;
        while (x < game.Width && y < game.Height) {
            potentialMove = new(x, y);

            if (board.IsAnyPieceAt(potentialMove)) {
                if (IsPieceAtPosAnEnemy(game, board, potentialMove, playerId)) {
                    availableMoves.Add(potentialMove);
                }
                break; // we cannot move past this cell.
            }
            else {
                availableMoves.Add(potentialMove);
            }

            x++;
            y++;
        }

        return availableMoves;
    }

    private static HashSet<Vector2Int> GetAvailableKnightMovesInBoard (
        ChessGame game,
        ClassicBoardState board,
        int playerId,
        Vector2Int pos,
        bool isQuantumMove
    ) {
        HashSet<Vector2Int> availableMoves = new();

        Vector2Int[] potentialMoves = {
            // towards bottom-left (0, 0)
            pos + new Vector2Int(-2, -1),
            pos + new Vector2Int(-1, -2),
            // towards bottom-right (0, max)
            pos + new Vector2Int(-1, 2),
            pos + new Vector2Int(-2, 1),
            // towards top-left (max, 0)
            pos + new Vector2Int(1, -2),
            pos + new Vector2Int(2, -1),
            // towards top-right (max, max)
            pos + new Vector2Int(2, 1),
            pos + new Vector2Int(1, 2),
        };

        foreach (var move in potentialMoves) {
            if (game.DoesPositionExist(move)) {
                if (board.IsAnyPieceAt(move)) {
                    if (IsPieceAtPosAnEnemy(game, board, move, playerId)) {
                        availableMoves.Add(move);
                    }
                }
                else {
                    availableMoves.Add(move);
                }
            }
        }

        return availableMoves;
    }

    /// <summary>
    /// Returns true if the piece at the position given is an enemy piece.
    /// This method assumes there is a piece at the given position, and will
    /// crash if called for an empty cell.
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <param name="position">The cell to check.</param>
    /// <param name="playerId">The id of the friendly player.</param>
    /// <returns></returns>
    private static bool IsPieceAtPosAnEnemy (
        ChessGame game, ClassicBoardState board, Vector2Int position, int playerId
    ) {
        int pieceId = board[position];
        return game.GetPieceById(pieceId).PlayerId != playerId;
    }
}
