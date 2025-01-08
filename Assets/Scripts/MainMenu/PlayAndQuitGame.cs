using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayAndQuitGame : MonoBehaviour {
    public Button Play;
    public Button Quit;

    private void Start() {
        Play.onClick.AddListener(PlayHandler);
        Quit.onClick.AddListener(QuitHandler);
    }

    private void PlayHandler()
    {
        SceneManager.LoadScene(3);
    }
    private void QuitHandler()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}