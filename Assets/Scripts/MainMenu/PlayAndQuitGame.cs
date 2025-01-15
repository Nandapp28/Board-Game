using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayAndQuitGame : MonoBehaviour {
    [Header("UI Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private const float ButtonClickDelay = 0.5f;

    private void Start() {
        // Assign listeners for buttons
        playButton.onClick.AddListener(OnPlayButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        if(AudioManagers.instance.isMusicPlaying == false)
        {
            AudioManagers.instance.PlayMusic(0);
        }
        AudioManagers.instance.SetMusicVolume(1);
    }

    private void OnPlayButtonClicked() {
        PlayButtonSound();
        StartCoroutine(LoadSceneAfterDelay());
    }

    private IEnumerator LoadSceneAfterDelay() {
        yield return new WaitForSeconds(ButtonClickDelay);
        SceneManager.LoadScene(3);
    }

    private void OnQuitButtonClicked() {
        PlayButtonSound();
        StartCoroutine(QuitGameAfterDelay());
    }

    private IEnumerator QuitGameAfterDelay() {
        yield return new WaitForSeconds(ButtonClickDelay);
        Application.Quit();
        Debug.Log("Game Quit");
    }

    private void PlayButtonSound() {
        AudioManagers.instance.PlaySoundEffect(0);
    }
}
