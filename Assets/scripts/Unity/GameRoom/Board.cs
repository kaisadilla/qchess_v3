#nullable enable

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Board : MonoBehaviour {
    [Header("Prefabs")]
    [SerializeField] private PieceIcon piecePrefab;
    [Header("Components")]
    [SerializeField] private GameRoom _room;
    [SerializeField] private BoardManager _boardManager;

    /// <summary>
    /// The piece the player has selected to make the next move.
    /// </summary>
    private PieceIcon? _selectedPiece = null;
    /// <summary>
    /// The targets selected for the next move.
    /// </summary>
    private readonly List<Position> _selectedTargets = new();

    public UiState UiState { get; private set; } = UiState.AwaitingPlayerAction;

    public void DrawBoard (PieceStyle style, BoardMeaning board) {
        _boardManager.Clear();

        for (int y = 0; y < _room.Game.Height; y++) {
            for (int x = 0; x < _room.Game.Width; x++) {
                var pieces = board[x, y];
                DrawPieces(style, pieces, new(x, y));
            }
        }
    }

    public void StartMove (PieceIcon piece) {
        UiState = UiState.SelectingMove;

        _selectedPiece = piece;
        _selectedPiece.SetSelected(true);
        
        var logicPiece = _selectedPiece.LogicPiece!;

        var validMoves = Ruleset.GetAvailableMoves(
            _room.Game,
            logicPiece.Position,
            logicPiece.ClassicPiece.Type,
            false
        );

        ShowAvailablePositions(validMoves);
    }

    public void CancelMove () {
        UiState = UiState.AwaitingPlayerAction;

        _selectedPiece?.SetSelected(false);
        _selectedPiece = null;
        _boardManager.BoardUi.Clear();
        _selectedTargets.Clear();
    }

    public void ShowAvailablePositions (List<Vector2Int> positions) {
        foreach (var pos in positions) {
            _boardManager.BoardUi.ShowAvailable(pos, true);
        }
    }

    public void SelectMoveTarget (Vector2Int target) {
        if (_selectedPiece.IsQuantumMove) {

        }
        else {
            MakeClassicMove(target);
        }
    }

    public void MakeClassicMove (Vector2Int target) {
        if (_selectedPiece == null) return; // TODO: Throw

        var piece = _selectedPiece.LogicPiece;
        _room.Game.TryClassicMove(piece.ClassicId, piece.Position, target);
        CancelMove();
    }

    private void DrawPieces (
        PieceStyle style, List<RealPiece> pieces, Vector2Int cell
    ) {
        if (pieces.Count > 4) throw new System.Exception(
            "There can't be more than 4 pieces in a single cell."
        );

        for (int i = 0; i < pieces.Count; i++) {
            RealPiece piece = pieces[i];
            var icon = Instantiate(piecePrefab);
            icon.Initialize(piece, style);

            _boardManager.PlaceIntoGrid(icon.transform, cell, pieces.Count != 1, i);
        }
    }
}
