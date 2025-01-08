using System.Collections;
using UnityEngine;

public class ShadowBackground : MonoBehaviour {
    public CanvasGroup shadowBackgroud;
    public float fadeDuration = 1.0f;
    
    private void Start() {
        InitializeCanvasGroups();
    }

    private void InitializeCanvasGroups()
    {
        CanvasGroup[] canvasGroups = { shadowBackgroud };
        foreach (CanvasGroup canvasGroup in canvasGroups)
        {
            canvasGroup.alpha = 0; 
            canvasGroup.interactable = false; 
            canvasGroup.blocksRaycasts = false; 
        }
    }

    public void HandleShadowBackground(bool isFadeIn)
    {
        if(isFadeIn)
        {
            shadowBackgroud.gameObject.SetActive(true);
            FadeIn(shadowBackgroud);
        }else{
            FadeOut(shadowBackgroud);
        }
    }

    #region Fade Methods
    public void FadeIn(CanvasGroup canvasGroup)
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1));
    }

    public void FadeOut(CanvasGroup canvasGroup)
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = end;
        canvasGroup.interactable = (end == 1);
        canvasGroup.blocksRaycasts = (end == 1);
    }
    #endregion
}