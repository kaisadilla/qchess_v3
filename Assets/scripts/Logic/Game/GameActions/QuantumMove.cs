#nullable enable

using System.Collections.Generic;
using UnityEngine;

public class QuantumMove : IMove {
    public int PlayerId { get; private set; }
    public int PieceId { get; private set; }
    public RealPiece Piece { get; private set; }
    public Vector2Int Origin { get; private set; }
    public List<Vector2Int> Targets { get; private set; }

    public QuantumMove (
        int playerId, int pieceId, RealPiece piece,
        Vector2Int origin, List<Vector2Int> targets
    ) {
        PlayerId = playerId;
        PieceId = pieceId;
        Piece = piece;
        Origin = origin;
        Targets = targets;
    }
}
