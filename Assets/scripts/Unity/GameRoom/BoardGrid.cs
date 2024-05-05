#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGrid : MonoBehaviour {
    [SerializeField] private Transform _pieceGrid;

    [SerializeField] private int _width = Constants.DEFAULT_BOARD_WIDTH;
    [SerializeField] private int _height = Constants.DEFAULT_BOARD_HEIGHT;

    private void Start () {
        PlaceContainerAtCorrectPosition();
    }

    public void Initialize (int width, int height) {
        _width = width;
        _height = height;

        PlaceContainerAtCorrectPosition();
    }

    public void PlaceIntoGrid (
        Transform obj, Vector2Int cell, bool isFourPieceCell = false, int cellIndex = 0
    ) {
        var coords = GetGridCoordinates(cell, isFourPieceCell, cellIndex);

        obj.SetParent(_pieceGrid);
        obj.transform.localPosition = coords;
    }

    /// <summary>
    /// Removes all pieces currently on the grid.
    /// </summary>
    public void Clear () {
        foreach (GameObject child in _pieceGrid) {
            Destroy(child);
        }
    }

    /// <summary>
    /// Returns the local coordinates in the grid where a piece in the position
    /// given would belong.
    /// </summary>
    /// <param name="cell">The cell in the grid the piece is in.</param>
    /// <param name="isFourPieceCell">True if there are 4 pieces in that
    /// position, false if there's only one.</param>
    /// <param name="cellIndex">The order of that piece within his cell, in
    /// case there are more than one piece in the cell.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public Vector2 GetGridCoordinates (
        Vector2Int cell, bool isFourPieceCell = false, int cellIndex = 0
    ) {
        if (isFourPieceCell == false) {
            return new(cell.x + 0.5f, cell.y + 0.5f);
        }

        if (cellIndex == 0) {
            return new(cell.x + 0.25f, cell.y + 0.75f);
        }
        if (cellIndex == 1) {
            return new(cell.x + 0.75f, cell.y + 0.75f);
        }
        if (cellIndex == 2) {
            return new(cell.x + 0.25f, cell.y + 0.25f);
        }
        if (cellIndex == 3) {
            return new(cell.x + 0.75f, cell.y + 0.25f);
        }

        throw new System.Exception(
            "Cell index must be between 0 and 3 (inclusive)."
        );
    }

    /// <summary>
    /// Places the grid that contains the pieces at the correct location
    /// based on the size of the board.
    /// </summary>
    private void PlaceContainerAtCorrectPosition () {
        _pieceGrid.transform.position = new(-_width / 2f, -_height / 2f);
    }
}
