#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ruleset {
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
        ChessGame game, Vector2Int origin, PieceType type, bool isQuantumMove
    ) {
        // TODO: This is mock data.
        List<Vector2Int> positions = new();

        for (int y = 0; y < game.Height; y++) {
            for (int x = 0; x < game.Width; x++) {
                Vector2Int pos = new(x, y);

                if (pos == origin) continue;
                if (game.CurrentState.IsCellEmpty(pos) == false) continue;

                positions.Add(pos);
            }
        }

        return positions;
    }
}
