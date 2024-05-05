#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealPiece {
    /// <summary>
    /// The id of the classic piece this piece represents.
    /// </summary>
    public int ClassicId { get; private set; }
    /// <summary>
    /// The position of this piece in the board.
    /// </summary>
    public Vector2Int Position { get; private set; }
    /// <summary>
    /// The "presence" of this specific real piece. The "presence" is a number
    /// that represents the % of all possible board states where this piece is
    /// located here. This indicates the probability of finding the piece here,
    /// although note that this is quantum chess, and entanglement means that
    /// certain pieces' locations are linked to other pieces' locations.
    /// </summary>
    public double Presence { get; private set; }
    /// <summary>
    /// A piece is quantum when its quantum value is lower than 1, which means
    /// its presence is in more than one place (note that one of these 'places'
    /// may be the pile of captured pieces). A piece that is not quantum is
    /// classical, which means its a 'normal' chess piece that behaves according
    /// to classical chess rules.
    /// </summary>
    public bool IsQuantum => Presence < 1.0;
    /// <summary>
    /// The classic piece this piece represents.
    /// </summary>
    public ClassicPiece ClassicPiece { get; private set; }

    public RealPiece (
        ChessGame game, int pieceId, Vector2Int position, double presence
    ) {
        ClassicId = pieceId;
        Position = position;
        Presence = presence;
        ClassicPiece = game.GetPieceById(pieceId);
    }
}
