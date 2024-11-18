using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonAnimator : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject playButton;

    [Header("Animation Settings")]
    [SerializeField] private Vector2 startScale = Vector2.zero;
    [SerializeField] private Vector2 endScale = Vector2.one;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float overshootScale = 1.2f; // Slightly larger scale for bounce effect
    [SerializeField] private Ease animationEase = Ease.OutBounce;

    [Header("Scene Settings")]
    [SerializeField] private int targetSceneIndex = 3;

    private RectTransform playButtonRect;

    private const float BounceUpDurationMultiplier = 0.5f;
    private const float BounceDownDurationMultiplier = 0.1f;

    private void Awake()
    {
        InitializePlayButton();
        TriggerScaleAnimation();
    }

    /// <summary>
    /// Initializes the Play Button's RectTransform and sets its initial scale.
    /// </summary>
    private void InitializePlayButton()
    {
        if (playButton == null)
        {
            Debug.LogWarning("Play Button GameObject is not assigned.", this);
            return;
        }

        playButtonRect = playButton.GetComponent<RectTransform>();
        if (playButtonRect == null)
        {
            Debug.LogWarning("RectTransform component is missing on Play Button GameObject.", playButton);
            return;
        }

        playButtonRect.localScale = startScale;
    }

    /// <summary>
    /// Triggers the scale bounce animation for the Play Button.
    /// </summary>
    public void TriggerScaleAnimation()
    {
        if (playButtonRect == null) return;

        Sequence bounceSequence = DOTween.Sequence();

        bounceSequence
            .Append(playButtonRect.DOScale(overshootScale, animationDuration * BounceUpDurationMultiplier).SetEase(Ease.OutQuad))
            .Append(playButtonRect.DOScale(endScale, animationDuration * BounceUpDurationMultiplier).SetEase(animationEase))
            .Play();
    }

    /// <summary>
    /// Handles button click event, playing an animation before loading the target scene.
    /// </summary>
    public void OnButtonClicked()
    {
        if (playButtonRect == null) return;

        Sequence clickSequence = DOTween.Sequence();

        clickSequence
            .Append(playButtonRect.DOScale(overshootScale, animationDuration * BounceDownDurationMultiplier).SetEase(Ease.OutQuad))
            .Append(playButtonRect.DOScale(endScale, animationDuration * BounceDownDurationMultiplier).SetEase(animationEase))
            .OnComplete(() => LoadTargetScene())
            .Play();
    }

    /// <summary>
    /// Loads the target scene specified in the settings.
    /// </summary>
    private void LoadTargetScene()
    {
        SceneManager.LoadScene(targetSceneIndex);
    }
}
