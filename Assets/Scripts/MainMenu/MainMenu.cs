using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [System.Serializable]
    public class MenuTransformAnimation
    {
        [Header("Start")]
        public Vector3 StartPosition;
        public float StartRotation;
        public Vector3 StartScale;

        [Header("End")]
        public Vector3 EndPosition;
        public float EndRotation;
        public Vector3 EndScale;
    }

    [System.Serializable]
    public class ModeMenu
    {
        public GameObject modeParentContainer;
        public Button singlePlayer;
        public MenuTransformAnimation transformAnimation;
        public float modeMenuContainerDuration;
    }

    [System.Serializable]
    public class SelectPlayer
    {
        public GameObject bgSelect;
        public Button button;
    }

    public GameObject SelectPlayerMode;
    public GameObject SelectPlayersContainer;
    public List<SelectPlayer> selectPlayers = new List<SelectPlayer>();
    public Button playButton;
    public ModeMenu modeMenu;
    public Button backButton;
    private int indexBack = 0;

    private void Start()
    {
        modeMenu.singlePlayer.onClick.AddListener(SinglePlayerButton);
        backButton.onClick.AddListener(BackButton);
        ModeMenuAnimation();
        CollectSelectPlayers();
        InitiliazeSelectPlayer();
        playButton.onClick.AddListener(PlayButton);
    }

    #region Mode Menu

    public void ModeMenuAnimation()
    {
        modeMenu.modeParentContainer.SetActive(true);
        StartTransform();
        AnimationTransform(modeMenu.modeParentContainer, modeMenu.transformAnimation.EndPosition, modeMenu.transformAnimation.EndRotation, modeMenu.transformAnimation.EndScale, modeMenu.modeMenuContainerDuration);
    }

    public void StartTransform()
    {
        modeMenu.modeParentContainer.transform.localPosition = modeMenu.transformAnimation.StartPosition;
        modeMenu.modeParentContainer.transform.localEulerAngles = new Vector3(0, 0, modeMenu.transformAnimation.StartRotation);
        modeMenu.modeParentContainer.transform.localScale = modeMenu.transformAnimation.StartScale;
    }

    public void SinglePlayerButton()
    {
        buttonsound();
        AnimationTransform(modeMenu.modeParentContainer, modeMenu.transformAnimation.StartPosition, modeMenu.transformAnimation.StartRotation, modeMenu.transformAnimation.StartScale, modeMenu.modeMenuContainerDuration);
        SelectPlayerAppear();
        indexBack++;
    }

    #endregion

    #region Select Player

    public void InitiliazeSelectPlayer()
    {
        SelectPlayerMode.SetActive(true);
        SelectPlayerMode.transform.localScale = Vector3.zero;
    }

    private void SelectPlayerAppear()
    {
        AnimationTransform(SelectPlayerMode,Vector3.zero,0,modeMenu.transformAnimation.EndScale,0.7f);
    }
    private void CollectSelectPlayers()
    {
        foreach (Transform child in SelectPlayersContainer.transform)
        {
            Transform bgSelect = child.Find("Bgselect");
            Transform buttonTransform = child.Find("Button");

            if (bgSelect != null && buttonTransform != null)
            {
                Button button = buttonTransform.GetComponent<Button>();
                bgSelect.gameObject.SetActive(false);

                if (button != null)
                {
                    // Pass button reference to SelectPlayerButton
                    button.onClick.AddListener(() => SelectPlayerButton(button));
                }

                SelectPlayer player = new SelectPlayer
                {
                    bgSelect = bgSelect.gameObject,
                    button = button
                };
                selectPlayers.Add(player);
            }
        }
    }

    public void SelectPlayerButton(Button pressedButton)
    {
        buttonsound();
        foreach (var player in selectPlayers)
        {
            // Set bgSelect active only for the button pressed
            player.bgSelect.SetActive(player.button == pressedButton);
        }
    }

    public void PlayButton()
    {
        buttonsound();
        StartCoroutine(LoadSceneAfterDelay(2));
        AudioManagers.instance.StopMusic();
    }
    #endregion

    public void AnimationTransform(GameObject obj, Vector3 position, float rotation, Vector3 scale, float duration)
    {
        obj.transform.DOLocalMove(position, duration);
        obj.transform.DOLocalRotate(new Vector3(0, 0, rotation), duration);
        obj.transform.DOScale(scale, duration);
    }

    #region BackButton
    public void BackButton()
    {
        buttonsound();
        // Check if we are not at the initial index
        if (indexBack > 0)
        {
            // Decrease the index and animate the transition back to the previous menu state
            indexBack--;
            AnimateBackToPreviousMenu();
        }
        else
        {
            // If at the initial state, load the previous scene (likely the main menu)
            StartCoroutine(LoadSceneAfterDelay(4));
        }
    }

    private void AnimateBackToPreviousMenu()
    {
        // Animate SelectPlayerMode to its starting scale and modeParentContainer back to its original position and scale
        AnimationTransform(SelectPlayerMode, Vector3.zero, 0, modeMenu.transformAnimation.StartScale, modeMenu.modeMenuContainerDuration);
        AnimationTransform(modeMenu.modeParentContainer, modeMenu.transformAnimation.EndPosition, modeMenu.transformAnimation.EndRotation, modeMenu.transformAnimation.EndScale, modeMenu.modeMenuContainerDuration);
    }

    private IEnumerator LoadSceneAfterDelay(int index) {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(index);
    }
    #endregion

    private void buttonsound()
    {
        AudioManagers.instance.PlaySoundEffect(0);
    }
}
