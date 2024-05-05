#nullable enable

using System.Collections.Generic;
using UnityEngine;

public static class ChessGameFactory {
    public static ChessGame Standard () {
        Dictionary<int, ClassicPiece> pieces = new() {
            [0] = new(0, 0, PieceType.Pawn),
            [1] = new(0, 1, PieceType.Pawn),
            [2] = new(0, 2, PieceType.Pawn),
            [3] = new(0, 3, PieceType.Pawn),
            [4] = new(0, 4, PieceType.Pawn),
            [5] = new(0, 5, PieceType.Pawn),
            [6] = new(0, 6, PieceType.Pawn),
            [7] = new(0, 7, PieceType.Pawn),
            [8] = new(0, 8, PieceType.Rook),
            [9] = new(0, 9, PieceType.Rook),
            [10] = new(0, 10, PieceType.Knight),
            [11] = new(0, 11, PieceType.Knight),
            [12] = new(0, 12, PieceType.Bishop),
            [13] = new(0, 13, PieceType.Bishop),
            [14] = new(0, 14, PieceType.Queen),
            [15] = new(0, 15, PieceType.King),

            [16] = new(1, 16, PieceType.Pawn),
            [17] = new(1, 17, PieceType.Pawn),
            [18] = new(1, 18, PieceType.Pawn),
            [19] = new(1, 19, PieceType.Pawn),
            [20] = new(1, 20, PieceType.Pawn),
            [21] = new(1, 21, PieceType.Pawn),
            [22] = new(1, 22, PieceType.Pawn),
            [23] = new(1, 23, PieceType.Pawn),
            [24] = new(1, 24, PieceType.Rook),
            [25] = new(1, 25, PieceType.Rook),
            [26] = new(1, 26, PieceType.Knight),
            [27] = new(1, 27, PieceType.Knight),
            [28] = new(1, 28, PieceType.Bishop),
            [29] = new(1, 29, PieceType.Bishop),
            [30] = new(1, 30, PieceType.Queen),
            [31] = new(1, 31, PieceType.King),
        };

        Dictionary<Vector2Int, int> initialDistribution = new() {
            [new(0, 0)] = 8,
            [new(1, 0)] = 10,
            [new(2, 0)] = 12,
            [new(3, 0)] = 14,
            [new(4, 0)] = 15,
            [new(5, 0)] = 13,
            [new(6, 0)] = 11,
            [new(7, 0)] = 9,
            [new(0, 1)] = 0,
            [new(1, 1)] = 1,
            [new(2, 1)] = 2,
            [new(3, 1)] = 3,
            [new(4, 1)] = 4,
            [new(5, 1)] = 5,
            [new(6, 1)] = 6,
            [new(7, 1)] = 7,

            [new(0, 7)] = 24,
            [new(1, 7)] = 26,
            [new(2, 7)] = 28,
            [new(3, 7)] = 30,
            [new(4, 7)] = 31,
            [new(5, 7)] = 29,
            [new(6, 7)] = 27,
            [new(7, 7)] = 25,
            [new(0, 6)] = 16,
            [new(1, 6)] = 17,
            [new(2, 6)] = 18,
            [new(3, 6)] = 19,
            [new(4, 6)] = 20,
            [new(5, 6)] = 21,
            [new(6, 6)] = 22,
            [new(7, 6)] = 23,
        };

        return new ChessGame(pieces, initialDistribution);
    }
}
