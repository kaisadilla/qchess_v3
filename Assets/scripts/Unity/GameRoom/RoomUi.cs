#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomUi : MonoBehaviour {
    public void NewGame () {
        SceneManager.LoadScene("GameRoom");
    }
}
