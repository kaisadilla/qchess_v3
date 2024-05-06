#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardInput : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private Board board;

    [Header("Layer masks")]
    [SerializeField] private LayerMask pieceLayer;
    [SerializeField] private LayerMask selectableLayer;

    /// <summary>
    /// The piece the player is holding to make a selection. This value is only
    /// assigned while the player holds the piece. Once the player releases the
    /// mouse button and the piece becomes selected, this variable is set to null.
    /// </summary>
    private PieceIcon? heldPiece = null;

    private void Update () {
        /* The player can select a piece to execute a classical or quantum move.
         * When the player presses down the left button of their mouse, a bar
         * will start loading. If the player releases the mouse button before
         * the bar is fully loaded, the move will be a classic move.
         */
        if (Input.GetMouseButtonDown(0)) {
            HandleLeftMouseButtonDown();
        }
        else if (Input.GetMouseButtonUp(0)) {
            HandleLeftMouseButtonUp();
        }
        else if (Input.GetMouseButtonUp(1)) {
            HandleRightMouseButtonUp();
        }
    }

    private void HandleLeftMouseButtonDown () {
        if (board.UiState == UiState.AwaitingPlayerAction) {
            TryStartPieceSelection();
        }
        else if (board.UiState == UiState.SelectingMove) {
            SelectCellForMove();
        }
    }

    private void HandleLeftMouseButtonUp () {
        if (heldPiece != null) {
            EndPieceSelection();
        }
    }

    private void HandleRightMouseButtonUp () {
        if (board.UiState == UiState.SelectingMove) {
            board.CancelMove();
        }
    }

    /// <summary>
    /// Starts the selection of a piece, if there's any piece where the player
    /// clicked. Depending on the amount of time passed between the start and
    /// end of the selection, the player will enable a classical or quantum
    /// move with that piece.
    /// </summary>
    private void TryStartPieceSelection () {
        var mousePos = GetMousePos();
        var pieceHit = Physics2D.Raycast(
            mousePos, Vector2.zero, Mathf.Infinity, pieceLayer
        );

        if (pieceHit.collider == null) return;

        heldPiece = pieceHit.collider.gameObject.GetComponentInParent<PieceIcon>();
        heldPiece.StartSelection();
    }

    /// <summary>
    /// Finishes the selection of a piece.
    /// </summary>
    private void EndPieceSelection () {
        if (heldPiece == null) return;

        heldPiece.EndSelection();
        board.StartMove(heldPiece);
        heldPiece = null;
    }

    private void SelectCellForMove () {
        var mousePos = GetMousePos();
        var validCellHit = Physics2D.Raycast(
            mousePos,
            Vector2.zero,
            Mathf.Infinity,
            selectableLayer
        );

        if (validCellHit.collider == null) return;

        var uiCell = validCellHit.collider.GetComponentInParent<UiCell>();
        board.SelectMoveTarget(uiCell.Position);
    }

    private Vector2 GetMousePos () {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
