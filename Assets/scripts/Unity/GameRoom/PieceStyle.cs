#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Piece style", menuName = "QChess/Piece style", order = 1)]
public class PieceStyle : ScriptableObject {
    [SerializeField] private Sprite[] whitePieces = new Sprite[6];
    [SerializeField] private Sprite[] blackPieces = new Sprite[6];

    public Sprite[] WhitePieces => whitePieces;
    public Sprite[] BlackPieces => blackPieces;
}
