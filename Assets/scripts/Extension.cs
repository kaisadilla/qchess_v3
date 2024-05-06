#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extension {
    public static T RandomElement<T> (this IEnumerable<T> col) {
        var arr = col.ToArray();
        return arr[Random.Range(0, arr.Length)];
    }
}
