using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonAnimator : MonoBehaviour
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private Vector2 startScale = Vector2.zero;
    [SerializeField] private Vector2 endScale = Vector2.one;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float overshootScale = 1.2f; // Slightly larger scale for bounce effect
    [SerializeField] private Ease animationEase = Ease.OutBounce;

    private RectTransform playButtonRect;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
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

        SetInitialScale(); // Moved this after assigning playButtonRect
        TriggerScaleAnimation();
    }

    private void SetInitialScale()
    {
        if (playButtonRect != null)
        {
            playButtonRect.localScale = startScale;
        }
    }

    public void TriggerScaleAnimation()
    {
        if (playButtonRect == null) return;

        // Create a bounce sequence
        Sequence bounceSequence = DOTween.Sequence();

        bounceSequence
            .Append(playButtonRect.DOScale(overshootScale, animationDuration * 0.5f).SetEase(Ease.OutQuad)) // Scale up past endScale
            .Append(playButtonRect.DOScale(endScale, animationDuration * 0.5f).SetEase(animationEase))       // Scale down to endScale
            .Play(); // Play the sequence
    }

    public void Clicked()
    {
        // Play the bounce animation, then load the new scene
        playButtonRect.DOScale(overshootScale, animationDuration * 0.1f).SetEase(Ease.OutQuad)
            .OnComplete(() => 
            {
                playButtonRect.DOScale(endScale, animationDuration * 0.1f).SetEase(animationEase)
                    .OnComplete(() => 
                    {
                        // Load the new scene after the animation completes
                        SceneManager.LoadScene(0);
                    });
            });
    }
}
