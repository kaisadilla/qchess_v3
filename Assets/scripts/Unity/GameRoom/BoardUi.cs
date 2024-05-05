#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUi : MonoBehaviour {
    [Header("Prefabs")]
    [SerializeField] private UiCell _prefabUiCell;
    [Header("Components")]
    [SerializeField] private Transform _uiContainer;
    [Header("Settings")]
    [SerializeField] private int _width = Constants.DEFAULT_BOARD_WIDTH;
    [SerializeField] private int _height = Constants.DEFAULT_BOARD_HEIGHT;

    /// <summary>
    /// A dictionary containing all UiCell objects currently on the scene.
    /// Each key represents the position in the grid for that specific UiCell.
    /// </summary>
    private UiCell[,] _uiCells;
    //private Dictionary<Vector2Int, UiCell> _uiCells = new();

    private void Awake () {
        _uiCells = new UiCell[_width, _height];
    }

    public void Clear () {
        foreach (var uiCell in GetAllUiCells()) {
            uiCell.Clear();
        }
    }

    public void ShowAvailable (Vector2Int pos, bool isAvailable) {
        _uiCells[pos.x, pos.y].DisplayAvailable(isAvailable);
    }

    public void SetSelected (Vector2Int pos, bool isSelected) {
        _uiCells[pos.x, pos.y].DisplaySelected(isSelected);
    }

    public void UpdateSize (int width, int height) {
        _width = width;
        _height = height;
        BuildCellGrid();
    }

    private void BuildCellGrid () {
        foreach (var cell in GetAllUiCells()) {
            if (cell) {
                Destroy(cell.gameObject);
            }
        }

        _uiCells = new UiCell[_width, _height];

        for (int y = 0; y < _height; y++) {
            for (int x = 0; x < _width; x++) {
                Vector2Int pos = new(x, y);

                var uiCell = Instantiate(_prefabUiCell);
                uiCell.Initialize(pos);

                var coords = BoardManager.GetGridCoordinates(pos);

                uiCell.transform.SetParent(_uiContainer);
                uiCell.transform.localPosition = coords;
                uiCell.name = $"UI Cell at {pos}";

                _uiCells[x, y] = uiCell;
            }
        }
    }

    private IEnumerable<UiCell> GetAllUiCells () {
        for (int x = 0; x < _uiCells.GetLength(0); x++) {
            for (int y = 0; y < _uiCells.GetLength(1); y++) {
                yield return _uiCells[x, y];
            }
        }
    }
}
