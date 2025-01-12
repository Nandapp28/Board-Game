using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingInGame : MonoBehaviour {
    public GameObject PausedContainers;
    public Button Close;
    public Button Replay;
    public Button Resume;
    public Button Quit;
    public Button Settings;
    private bool isOpen = false;
    private ShadowBackground shadowBackground;


    private void Start() {
        shadowBackground = FindAnyObjectByType<ShadowBackground>();
        PausedContainers.SetActive(false);
        Close.onClick.AddListener(CloseSettings);
        Replay.onClick.AddListener(ReplayGame);
        Quit.onClick.AddListener(QuitGame);
        Resume.onClick.AddListener(CloseSettings);
        Settings.onClick.AddListener(OpenSettings);
    }

    private void OpenSettings()
    {
        shadowBackground.HandleShadowBackground(true);
        buttonSoundEffect();
        PausedContainers.SetActive(true);
    }

    private void CloseSettings()
    {
        shadowBackground.HandleShadowBackground(false);
        buttonSoundEffect();
        PausedContainers.SetActive(false);
    }

    private void ReplayGame()
    {
        buttonSoundEffect();
        StartCoroutine(LoadSceneHandler(1));
    }

    private void QuitGame()
    {
        buttonSoundEffect();
        StartCoroutine(LoadSceneHandler(4));
    }

    private IEnumerator LoadSceneHandler(int index)
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(index);
    }

    private void buttonSoundEffect()
    {
        AudioController.PlaySoundEffect(0);
    }
}