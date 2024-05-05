#nullable enable

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PieceIcon : MonoBehaviour {
    #region Constants
    /// <summary>
    /// The scale of the entire piece normally (i.e. when it's not selected).
    /// </summary>
    private static readonly Vector3 NORMAL_SCALE = Vector3.one;
    /// <summary>
    /// The scale of the entire piece while it's selected.
    /// </summary>
    private static readonly Vector3 SELECTED_SCALE = new(1.2f, 1.2f, 1.2f);
    /// <summary>
    /// The local position of the sprite inside this icon when it's a classic piece.
    /// </summary>
    private static readonly Vector3 CLASSIC_SPRITE_POS = new(0, 0, 0);
    /// <summary>
    /// The local scale of the sprite inside this icon when it's a classic piece.
    /// </summary>
    private static readonly Vector3 CLASSIC_SPRITE_SCALE = new(1, 1, 1);
    /// <summary>
    /// The local position of the sprite inside this icon when it's a quantum piece.
    /// </summary>
    private static readonly Vector3 QUANTUM_SPRITE_POS = new(0, 0.03f, 0);
    /// <summary>
    /// The local scale of the sprite inside this icon when it's a quantum piece.
    /// </summary>
    private static readonly Vector3 QUANTUM_SPRITE_SCALE = new(0.7f, 0.7f, 1);
    #endregion

    #region UnityFields
    [Header("Scene objects")]
    [Tooltip("The game object containing the visual elements of this piece.")]
    [SerializeField] private GameObject pieceGO;
    [Tooltip("The game object containing the square around this piece.")]
    [SerializeField] private GameObject squareGO;
    [SerializeField] private SpriteRenderer pieceSprite;
    [SerializeField] private GameObject apperance;
    [SerializeField] private SpriteMask apperanceMeterMask;
    [SerializeField] private TextMeshProUGUI apperanceText;
    [SerializeField] private SpriteMask quantumMoveLoaderMask;

    [Header("Debug information")]
    [SerializeField] private TextMeshProUGUI debugId;
    #endregion

    /// <summary>
    /// The piece in the game logic this icon represents.
    /// </summary>
    public RealPiece? LogicPiece { get; private set; }

    /// <summary>
    /// Initializes this piece icon with the real piece given.
    /// This method can be called on an icon that is already initialized,
    /// to override its previous values.
    /// </summary>
    /// <param name="piece">The piece to represent.</param>
    /// <param name="pieceStyle">The piece style to use for its icons.</param>
    public void Initialize (RealPiece piece, PieceStyle pieceStyle) {
        LogicPiece = piece;

        var spriteArray = piece.ClassicPiece.PlayerId switch {
            0 => pieceStyle.WhitePieces,
            _ => pieceStyle.BlackPieces,
        };

        pieceSprite.sprite = spriteArray[(int)piece.ClassicPiece.Type];
        debugId.text = piece.ClassicId.ToString();

        if (piece.IsQuantum) {
            DrawQuantum(piece);
        }
        else {
            DrawClassic(piece);
        }

        gameObject.name = $"{piece.ClassicId} (${piece.ClassicPiece.PlayerName} " +
            $"{piece.ClassicPiece.Type})";
    }

    /// <summary>
    /// Draws the piece as a quantum piece.
    /// </summary>
    /// <param name="piece">The quantum piece to represent.</param>
    private void DrawQuantum (RealPiece piece) {
        apperance.SetActive(true);
        pieceSprite.transform.localPosition = QUANTUM_SPRITE_POS;
        pieceSprite.transform.localScale = QUANTUM_SPRITE_SCALE;
        apperanceMeterMask.alphaCutoff = 1 - (float)piece.Presence;

        apperanceText.text = piece.Presence switch {
            0 => "0",
            < 0.0001 => "<0.01",
            < 0.9999 => (piece.Presence * 100).ToString("0.##"),
            _ => "~100"
        };
    }

    /// <summary>
    /// Draws the piece as a classic piece.
    /// </summary>
    /// <param name="piece">The quantum piece to represent.</param>
    private void DrawClassic (RealPiece piece) {
        apperance.SetActive(false);
        pieceSprite.transform.localPosition = CLASSIC_SPRITE_POS;
        pieceSprite.transform.localScale = CLASSIC_SPRITE_SCALE;
    }
}
