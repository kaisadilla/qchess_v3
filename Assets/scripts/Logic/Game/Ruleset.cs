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
            return GetAvailablePawnMoves(game, piece, isQuantumMove);
        }

        // TODO: This is mock data.
        List<Vector2Int> positions = new();

        for (int y = 0; y < game.Height; y++) {
            for (int x = 0; x < game.Width; x++) {
                Vector2Int pos = new(x, y);

                if (pos == piece.Position) continue;
                if (game.CurrentState.IsCellEmpty(pos) == false) continue;

                positions.Add(pos);
            }
        }

        return positions;
    }

    private static List<Vector2Int> GetAvailablePawnMoves (
        ChessGame game,
        RealPiece piece,
        bool isQuantumMove
    ) {
        HashSet<Vector2Int> availableMoves = new();

        foreach (var state in game.CurrentState.ClassicStates) {
            var moves = GetAvailablePawnMovesInBoard(
                game, state, piece, isQuantumMove
            );
            availableMoves.UnionWith(moves); // TODO: Check this is the most efficient way to merge both sets.
        }

        return new(availableMoves);
    }

    private static HashSet<Vector2Int> GetAvailablePawnMovesInBoard (
        ChessGame game,
        ClassicBoardState board,
        RealPiece piece,
        bool isQuantumMove
    ) {
        HashSet<Vector2Int> availableMoves = new();

        int playerId = piece.ClassicPiece.PlayerId;
        int forwardVal = playerId == 0 ? 1 : -1;

        // the cell ahead.
        Vector2Int potentialMove = piece.Position + new Vector2Int(0, forwardVal);
        bool isPieceInFront = board.IsAnyPieceAt(potentialMove);
        if (game.DoesPositionExist(potentialMove) && isPieceInFront == false) {
            availableMoves.Add(potentialMove);
        }

        // double move, only available when the pawn is in its starting row.
        if (piece.Position.y == (playerId == 0 ? 1 : 6)) {
            var potentialSuperMove = piece.Position + new Vector2Int(0, forwardVal + forwardVal);

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
        potentialMove = piece.Position + new Vector2Int(-1, forwardVal);
        if (game.DoesPositionExist(potentialMove) && board.IsAnyPieceAt(potentialMove)) {
            int pieceId = board[potentialMove];
            // can only capture enemy pieces.
            if (game.GetPieceById(pieceId).PlayerId != playerId) {
                availableMoves.Add(potentialMove);
            }
        }

        // same but capture right.
        potentialMove = piece.Position + new Vector2Int(1, forwardVal);
        if (game.DoesPositionExist(potentialMove) && board.IsAnyPieceAt(potentialMove)) {
            int pieceId = board[potentialMove];
            // can only capture enemy pieces.
            if (game.GetPieceById(pieceId).PlayerId != playerId) {
                availableMoves.Add(potentialMove);
            }
        }

        return availableMoves;
    }
}
