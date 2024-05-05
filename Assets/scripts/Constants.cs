#nullable enable

using System.Collections.Generic;
using UnityEngine;

public class Constants {
    public const int DEFAULT_BOARD_WIDTH = 8;
    public const int DEFAULT_BOARD_HEIGHT = 8;

    /********
     * GAME *
     ********/
    /// <summary>
    /// The delay, in seconds, between the player presses a piece and the
    /// quantum move starts loading.
    /// </summary>
    public const float QUANTUM_LOAD_INITIAL_DELAY = 0.1f;
    /// <summary>
    /// The amount of time, in seconds, that it takes the quantum load to
    /// complete.
    /// </summary>
    public const float QUANTUM_LOAD_DURATION = 0.75f;
}
