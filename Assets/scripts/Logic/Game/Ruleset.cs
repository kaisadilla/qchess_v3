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
        var meaning = game.Meaning;

        HashSet<Vector2Int> availableCells;

        if (piece.ClassicPiece.Type == PieceType.Pawn) {
            availableCells = GetAvailablePawnMoves(game, piece, isQuantumMove);
        }
        else if (piece.ClassicPiece.Type == PieceType.Rook) {
            availableCells = GetAvailableRookMoves(game, piece, isQuantumMove);
        }
        else if(piece.ClassicPiece.Type == PieceType.Bishop) {
            availableCells = GetAvailableBishopMoves(game, piece, isQuantumMove);
        }
        else if(piece.ClassicPiece.Type == PieceType.Knight) {
            availableCells = GetAvailableKnightMoves(game, piece, isQuantumMove);
        }
        else if(piece.ClassicPiece.Type == PieceType.Queen) {
            availableCells = GetAvailableRookMoves (game, piece, isQuantumMove);
            var bishopMoves = GetAvailableBishopMoves(game, piece, isQuantumMove);

            availableCells.UnionWith(bishopMoves);
        }
        else if(piece.ClassicPiece.Type == PieceType.King) {
            availableCells = GetAvailableKingMoves(game, piece, isQuantumMove);
        }
        else {
            availableCells = new();
        }

        HashSet<Vector2Int> cellsToRemove = new();
        // we'll validate that every cell marked as available is actually
        // available. This is because some moves are not allowed in the real
        // board, even if there's some classic states where that move would
        // be possible.
        foreach (var cell in availableCells) {
            // all the real pieces in the cell where are checking.
            List<RealPiece> piecesInCell = meaning[cell];

            // if any of the pieces in that cell belongs to the same player as
            // the piece we are going to move, then that cell may not be
            // available.
            foreach (var p in piecesInCell) {
                // a piece can move to where another part of itself is.
                if (p.ClassicId == piece.ClassicId) {
                    continue;
                }
                // but it cannot move to where a DIFFERENT piece belonging
                // to the same player is.
                if (p.ClassicPiece.PlayerId == piece.ClassicPiece.PlayerId) {
                    cellsToRemove.Add(cell);
                    break;
                }
            }
        }

        availableCells.ExceptWith(cellsToRemove);

        return new(availableCells);
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
        if (piece.Type == PieceType.King) {
            return new(GetAvailableKingMovesInBoard(
                game, board, piece.PlayerId, piece.Id, origin, false
            ));
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
            if (IsPieceAt(state, piece.Position, piece.ClassicId) == false) {
                continue;
            }

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
            if (IsPieceAt(state, piece.Position, piece.ClassicId) == false) {
                continue;
            }

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
            if (IsPieceAt(state, piece.Position, piece.ClassicId) == false) {
                continue;
            }

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
            if (IsPieceAt(state, piece.Position, piece.ClassicId) == false) {
                continue;
            }

            var moves = GetAvailableKnightMovesInBoard(
                game, state, piece.ClassicPiece.PlayerId, piece.Position, isQuantumMove
            );
            availableMoves.UnionWith(moves);
        }

        return availableMoves;
    }

    private static HashSet<Vector2Int> GetAvailableKingMoves (
        ChessGame game,
        RealPiece piece,
        bool isQuantumMove
    ) {
        HashSet<Vector2Int> availableMoves = new();

        foreach (var state in game.CurrentState.ClassicStates) {
            if (IsPieceAt(state, piece.Position, piece.ClassicId) == false) {
                continue;
            }

            var moves = GetAvailableKingMovesInBoard(
                game, state, piece.ClassicPiece.PlayerId, piece.ClassicId,
                piece.Position, isQuantumMove
            );
            availableMoves.UnionWith(moves);
        }

        return availableMoves;
    }

    private static bool IsPieceAt (ClassicBoardState state, Vector2Int pos, int pieceId) {
        // there's no piece at the given cell.
        if (state.IsAnyPieceAt(pos) == false) return false;
        // the piece in the given cell is a different one.
        if (state[pos] != pieceId) return false;

        return true;
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
        if (pos.y == GetPlayerNthFile(game, playerId, 1)) {
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

        // en passant, only available when the pawn is three squares away from
        // its starting position, and the pawn next two it executed its first
        // move (a double advance forward) in the preceding turn.
        if (pos.y == GetPlayerNthFile(game, playerId, 4)) {
            var enemyPawnRow = playerId == 0 ? 6 : 1;

            var cellsToCheck = new Vector2Int[] {
                pos + new Vector2Int(-1, 0), // left
                pos + new Vector2Int(1, 0), // right
            };

            foreach (var cell in cellsToCheck) {
                // if there's no piece at that cell, en passant is not possible.
                if (board.TryGetPieceAt(cell, out int potentialPawnId) == false) {
                    continue;
                }

                var potentialPawn = game.GetPieceById(potentialPawnId);

                // if that piece is owned by the same player, skip.
                if (potentialPawn.PlayerId == playerId) continue;
                // if that piece is not a pawn, skip.
                if (potentialPawn.Type != PieceType.Pawn) continue;

                var lastMove = game.LastMove;
                // if that pawn didn't move last turn, skip.
                if (lastMove.PieceId != potentialPawnId) continue;
                // or if it moved from a position different from its starting point, skip.
                if (lastMove.Origin.y != enemyPawnRow) continue;

                availableMoves.Add(cell +  new Vector2Int(0, forwardVal));
            }
        }

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
    
    private static HashSet<Vector2Int> GetAvailableKingMovesInBoard (
        ChessGame game,
        ClassicBoardState board,
        int playerId,
        int pieceId,
        Vector2Int pos,
        bool isQuantumMove
    ) {
        HashSet<Vector2Int> availableMoves = new();

        Vector2Int[] potentialMoves = {
            pos + new Vector2Int(-1, -1), // botton-left
            pos + new Vector2Int(0, -1), // bottom
            pos + new Vector2Int(1, -1), // bottom-right
            pos + new Vector2Int(1, 0), // right
            pos + new Vector2Int(1, 1), // top-right
            pos + new Vector2Int(0, 1), // top
            pos + new Vector2Int(-1, 1), // top-left
            pos + new Vector2Int(-1, 0), // left
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

        // Castling: can be done when the king and the target rook haven't
        // been moved yet this game, and when there's no piece in between them.
        // if the king hasn't been moved yet.
        int startingKingRow = GetPlayerNthFile(game, playerId, 0);

        // the king is its in starting file and hasn't moved.
        if (pos.y == startingKingRow && board.MovedPieces.Contains(pieceId) == false) {
            bool isCastlingPossible = true;

            // castling towards the left (x = 0).
            // if there's a piece at the leftmost column which hasn't moved. TODO: Maybe we allow non-rooks to, for non-conventional armies.
            if (board.TryGetPieceAt(new(0, pos.y), out int potentialRookId)
                && game.GetPieceById(potentialRookId).Type == PieceType.Rook
                && board.MovedPieces.Contains(potentialRookId) == false
            ) {
                // every space between the rook and the king must be empty.
                for (int i = 1; i < pos.x; i++) {
                    if (board.IsAnyPieceAt(new(i, pos.y))) {
                        isCastlingPossible = false;
                        break;
                    }
                }

                if (isCastlingPossible) {
                    availableMoves.Add(new(2, pos.y));
                }
            }

            // castling towards the right (x = max).
            // if there's a piece at the rightmost column which hasn't moved.
            int rightmost = GetRightNthRank(game, 0);
            isCastlingPossible = true; // reset it to true.
            if (board.TryGetPieceAt(new(rightmost, pos.y), out potentialRookId)
                && game.GetPieceById(potentialRookId).Type == PieceType.Rook
                && board.MovedPieces.Contains(potentialRookId) == false
            ) {
                // every space between the rook and the king must be empty.
                for (int i = rightmost - 1; i > pos.x; i--) {
                    if (board.IsAnyPieceAt(new(i, pos.y))) {
                        isCastlingPossible = false;
                        break;
                    }
                }

                if (isCastlingPossible) {
                    availableMoves.Add(new(rightmost - 1, pos.y));
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

    /// <summary>
    /// Returns the y coordinate of the player's nth file (starting at 0th).
    /// </summary>
    /// <param name="game">The game in which the board is.</param>
    /// <param name="playerId">The id of the player.</param>
    /// <param name="n">The # of the file (starting at 0), from the player's
    /// perspective.</param>
    private static int GetPlayerNthFile (ChessGame game, int playerId, int n) {
        return playerId == 0 ? n : game.Height - n - 1;
    }

    private static int GetRightNthRank (ChessGame game, int n) {
        return game.Width - n - 1;
    }
}
