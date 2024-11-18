using DG.Tweening;
using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform logo;
    [SerializeField] private GameObject loading;

    [Header("Animation Settings")]
    [SerializeField] private float animationDelay = 1f;
    [SerializeField] private float logoMoveDuration = 0.7f;
    [SerializeField] private float loadingScaleDuration = 0.5f;
    [SerializeField] private float targetLogoYPosition = 40f;

    private RectTransform loadingRectTransform;
    private LoadingBar loadingBar;

    private void Start()
    {
        InitializeComponents();
        StartCoroutine(AnimateSplashScreen());
    }

    private void InitializeComponents()
    {
        if (loading == null || logo == null)
        {
            Debug.LogWarning("UI elements not assigned.", this);
            return;
        }

        loading.SetActive(false);
        loadingRectTransform = loading.GetComponent<RectTransform>();
        loadingBar = GetComponent<LoadingBar>();

        if (loadingRectTransform == null)
            Debug.LogWarning("Loading GameObject is missing RectTransform component.", loading);

        if (loadingBar == null)
            Debug.LogWarning("Missing LoadingBar component on SplashScreen GameObject.", this);
    }

    private IEnumerator AnimateSplashScreen()
    {
        ResetLogoPosition();
        yield return new WaitForSeconds(animationDelay);
        AnimateLogoMovement();
    }

    private void ResetLogoPosition()
    {
        if (logo != null)
            logo.localPosition = Vector2.zero;
    }

    private void AnimateLogoMovement()
    {
        if (logo == null) return;

        logo.DOAnchorPosY(targetLogoYPosition, logoMoveDuration).SetEase(Ease.Linear)
            .OnComplete(AnimateLoadingBar);
    }

    private void AnimateLoadingBar()
    {
        if (loadingRectTransform == null || loadingBar == null) return;

        loading.SetActive(true);
        loadingRectTransform.localScale = Vector2.zero;
        loadingRectTransform.DOScale(Vector2.one, loadingScaleDuration).SetEase(Ease.Linear)
            .OnComplete(loadingBar.StartLoading);
    }
}
