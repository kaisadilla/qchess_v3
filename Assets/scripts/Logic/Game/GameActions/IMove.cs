#nullable enable

using System.Collections.Generic;
using UnityEngine;

public interface IMove {
    /// <summary>
    /// The id of the player who made this move.
    /// </summary>
    public int PlayerId { get; }
    public int PieceId { get; }
    public RealPiece Piece { get; }
    public Vector2Int Origin { get; }
}
