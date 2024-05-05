#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityUtils {
    /// <summary>
    /// Stops the coroutine given if it's not null, and then assigns
    /// "null" to the variable holding the coroutine.
    /// </summary>
    /// <param name="owner">The MonoBehavior object in which to call
    /// this method.</param>
    /// <param name="coroutine">The coroutine to stop.</param>
    public static void StopCoroutine (this MonoBehaviour owner, ref Coroutine? coroutine) {
        if (coroutine != null) {
            owner.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}