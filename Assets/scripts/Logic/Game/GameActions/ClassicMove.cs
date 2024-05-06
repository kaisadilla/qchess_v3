#nullable enable

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClassicMove : IMove {
    public int PlayerId { get; private set; }
    public int PieceId { get; private set; }
    public RealPiece Piece { get; private set; }
    public Vector2Int Origin { get; private set; }
    public Vector2Int Target { get; private set; }
    /// <summary>
    /// The pieces that were at the target position before this piece moves
    /// into there.
    /// </summary>
    public List<RealPiece> PiecesAtTarget { get; private set; }

    /// <summary>
    /// If true, the origin cell will be measured.
    /// </summary>
    public bool MeasuresOrigin { get; private set; }
    /// <summary>
    /// If true, the target cell will be measured (the pieces themselves are
    /// not measured, so any piece discarded from this cell doesn't necessarily
    /// define its position elsewhere - it could still be in multiple cells,
    /// just not this specific one).
    /// </summary>
    public bool MeasuresTarget { get; private set; }

    /// <summary>
    /// True if this move triggers a castling.
    /// </summary>
    public bool IsCastling { get; private set; }

    public ClassicMove (
        int playerId, int pieceId, RealPiece piece, Vector2Int origin,
        Vector2Int target, bool isCastling, List<RealPiece> piecesAtTarget
    ) {
        PlayerId = playerId;
        PieceId = pieceId;
        Piece = piece;
        Origin = origin;
        Target = target;
        IsCastling = isCastling;

        PiecesAtTarget = piecesAtTarget;

        // if a quantum piece moves to a cell that contains a classic piece,
        // the attacking (quantum) piece will be measured.
        MeasuresOrigin = piece.IsQuantum && piecesAtTarget.Any(
            p => p.IsQuantum == false
        );
        // if a classic piece moves to a cell that contains a quantum piece
        // (or more), it'll always measure the cell.
        MeasuresTarget = piece.IsQuantum == false && piecesAtTarget.Count > 0;
    }
}
