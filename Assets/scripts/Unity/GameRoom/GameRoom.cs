#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoom : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private PieceStyle _pieceStyle;

    [Header("Scene objects")]
    [SerializeField] private Board _board;

    public ChessGame Game { get; private set; }

    private void Start () {
        Game = ChessGameFactory.Standard();
        _board.DrawBoard(_pieceStyle, Game.Meaning);

        Game.OnMove += RedrawBoard;
    }

    private void RedrawBoard (object sender, MoveEventArgs evt) {
        _board.DrawBoard(_pieceStyle, Game.Meaning);
    }
}
