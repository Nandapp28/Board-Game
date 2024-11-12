using UnityEngine;
using DG.Tweening;
using System.Collections;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] public RectTransform loadingBar; // RectTransform reference for the loading bar
    [SerializeField] public float animationDuration = 1.0f; // Duration of the loading animation
    private const float delayDuration = 4f; // Delay before starting the animation

    public void StartLoading()
    {
        StartCoroutine(StartLoadingWithDelay());
    }

    private IEnumerator StartLoadingWithDelay()
    {
        yield return new WaitForSeconds(delayDuration);
        AnimateLoadingBar();
    }

    private void AnimateLoadingBar()
    {
        // Reset scale X to 0 before starting the animation
        loadingBar.localScale = new Vector3(0, 1, 1);

        // Animate scale X from 0 to 1 over 'animationDuration' seconds with linear easing
        loadingBar.DOScaleX(1, animationDuration).SetEase(Ease.Linear);
    }
}
