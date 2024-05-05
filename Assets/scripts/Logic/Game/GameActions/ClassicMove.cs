#nullable enable

using UnityEngine;

public class ClassicMove : IMove {
    public int PlayerId { get; private set; }
    public int PieceId { get; private set; }
    public Vector2Int Origin { get; private set; }
    public Vector2Int Target { get; private set; }

    /// <summary>
    /// True if this move triggers a castling.
    /// </summary>
    public bool IsCastling { get; private set; }

    public ClassicMove (
        int playerId, int pieceId, Vector2Int origin, Vector2Int target,
        bool isCastling
    ) {
        PlayerId = playerId;
        PieceId = pieceId;
        Origin = origin;
        Target = target;
        IsCastling = isCastling;
    }
}
