#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCell : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private GameObject goAvailable;
    [SerializeField] private GameObject goSelected;

    public Vector2Int Position { get; private set; }

    public void Initialize (Vector2Int position) {
        Position = position;
    }

    public void DisplayAvailable (bool isAvailable) {
        goAvailable.SetActive(isAvailable);
    }

    public void DisplaySelected (bool isSelected) {
        goSelected.SetActive(isSelected);
    }

    /// <summary>
    /// Hides all the elements in this UI cell.
    /// </summary>
    public void Clear () {
        goAvailable.SetActive(false);
        goSelected.SetActive(false);
    }
}
