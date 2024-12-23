using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;  // Untuk mengakses Image UI

public class SplashScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform logo;
    [SerializeField] private GameObject loading;
    [SerializeField] private Image loadingBar;  // Menggunakan Image untuk loading bar

    [Header("Animation Settings")]
    [SerializeField] private float animationDelay = 1f;
    [SerializeField] private float logoMoveDuration = 0.7f;
    [SerializeField] private float loadingScaleDuration = 0.5f;
    [SerializeField] private float targetLogoYPosition = 40f;
    [SerializeField] private float loadingDuration = 5f;  // Durasi loading bar

    private RectTransform loadingRectTransform;

    private void Start()
    {
        InitializeComponents();
        StartCoroutine(AnimateSplashScreen());
    }

    private void InitializeComponents()
    {
        if (loading == null || logo == null || loadingBar == null)
        {
            Debug.LogWarning("UI elements not assigned.", this);
            return;
        }

        loading.SetActive(false);
        loadingRectTransform = loading.GetComponent<RectTransform>();

        if (loadingRectTransform == null)
            Debug.LogWarning("Loading GameObject is missing RectTransform component.", loading);
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
            .OnComplete(StartLoadingBar);
    }

    private void StartLoadingBar()
    {
        // Set the fillAmount to 0 before starting
        loadingBar.fillAmount = 0f;
        StartCoroutine(UpdateLoadingBar());
    }

    private IEnumerator UpdateLoadingBar()
    {
        float elapsedTime = 0f;

        while (elapsedTime < loadingDuration)
        {
            elapsedTime += Time.deltaTime;
            loadingBar.fillAmount = Mathf.Lerp(0f, 1f, elapsedTime / loadingDuration);
            yield return null;
        }

        // Setelah loading bar selesai, lakukan aksi berikutnya (misalnya, pindah ke scene berikutnya)
        OnLoadingComplete();
    }

    private void OnLoadingComplete()
    {
        Debug.Log("Loading Complete!");
        // Aksi setelah loading selesai, seperti pindah ke scene berikutnya
        // SceneManager.LoadScene("NextScene"); // Contoh penggunaan
    }
}
