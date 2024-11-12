using DG.Tweening;
using UnityEngine;
using System.Collections;

public class Splashscreen : MonoBehaviour
{
    public RectTransform Logo;
    public GameObject loading;
    private RectTransform loadingRectTransform;
    public float duration;

    private LoadingBar loadingbar;


    private void Start()
    {
        loading.SetActive(false);
        loadingRectTransform = loading.GetComponent<RectTransform>();
        loadingbar = GetComponent<LoadingBar>();
        // Start the countdown coroutine
        StartCoroutine(CountdownAndAnimate());
    }

    private IEnumerator CountdownAndAnimate()
    {

        // Reset Logo's initial position if needed
        Logo.localPosition = new Vector2(0, 0);
        // Wait for the specified duration before starting the animation
        yield return new WaitForSeconds(duration);

        // Call the animation method
        DoMoveLogo();
    }

    private void DoMoveLogo()
    {
        // Animate the Logo moving to the Y position 40 over 0.7 seconds with linear easing
        Logo.DOAnchorPosY(40, 0.7f).SetEase(Ease.Linear);
        Loading();
    }

    private void Loading()
    {
 
        loadingRectTransform.localScale = new Vector2(0,0);
        loading.SetActive(true);

        loadingRectTransform.DOScale(1,1).SetEase(Ease.Linear);

        loadingbar.StartLoading();
    }
}
