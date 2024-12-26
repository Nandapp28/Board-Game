using System.Collections;
using TMPro;
using UnityEngine;

public class PhaseUI : MonoBehaviour
{
    #region UI References
    public CanvasGroup BiddingPhase;
    public CanvasGroup ActionPhase;
    public CanvasGroup SellingPhase;
    public CanvasGroup RumorPhase;
    public CanvasGroup ResolutionPhase;
    #endregion

    #region Configuration
    public float fadeDuration = 1.0f;
    #endregion

    private SemesterManager semesterManager;

    #region Initialization
    private void Start() 
    {
        semesterManager = FindAnyObjectByType<SemesterManager>();
        InitializeCanvasGroups();
    }

    private void InitializeCanvasGroups()
    {
        CanvasGroup[] canvasGroups = { BiddingPhase, ActionPhase, SellingPhase, RumorPhase, ResolutionPhase };
        foreach (CanvasGroup canvasGroup in canvasGroups)
        {
            canvasGroup.alpha = 0; 
            canvasGroup.interactable = false; 
            canvasGroup.blocksRaycasts = false; 
        }
    }
    #endregion

    #region Phase Handling Methods
    public void HandleBiddingPhase(bool isFadeIn)
    {
        Transform SemesterText = BiddingPhase.transform.Find("Semester");
        if(SemesterText != null)
        {
           TextMeshProUGUI TextComponent = SemesterText.GetComponent<TextMeshProUGUI>();

           TextComponent.text = "Semester " + semesterManager.CurrentSemester;
            if(isFadeIn)
            {
                BiddingPhase.gameObject.SetActive(true);
                FadeIn(BiddingPhase);
            }else{
                FadeOut(BiddingPhase);
            }
        }

    }

    public void HandleActionPhase(bool isFadeIn)
    {
        Transform SemesterText = ActionPhase.transform.Find("Semester");
        if (SemesterText != null)
        {
            TextMeshProUGUI TextComponent = SemesterText.GetComponent<TextMeshProUGUI>();
            TextComponent.text = "Semester " + semesterManager.CurrentSemester;
        }

        if (isFadeIn)
        {
            ActionPhase.gameObject.SetActive(true);
            FadeIn(ActionPhase);
        }
        else
        {
            FadeOut(ActionPhase);
        }
    }

    public void HandleSellingPhase(bool isFadeIn)
    {
        Transform SemesterText = SellingPhase.transform.Find("Semester");
        if (SemesterText != null)
        {
            TextMeshProUGUI TextComponent = SemesterText.GetComponent<TextMeshProUGUI>();
            TextComponent.text = "Semester " + semesterManager.CurrentSemester;
        }

        if (isFadeIn)
        {
            SellingPhase.gameObject.SetActive(true);
            FadeIn(SellingPhase);
        }
        else
        {
            FadeOut(SellingPhase);
        }
    }

    public void HandleRumorPhase(bool isFadeIn)
    {
        Transform SemesterText = RumorPhase.transform.Find("Semester");
        if (SemesterText != null)
        {
            TextMeshProUGUI TextComponent = SemesterText.GetComponent<TextMeshProUGUI>();
            TextComponent.text = "Semester " + semesterManager.CurrentSemester;
        }

        if (isFadeIn)
        {
            RumorPhase.gameObject.SetActive(true);
            FadeIn(RumorPhase);
        }
        else
        {
            FadeOut(RumorPhase);
        }
    }

    public void HandleResolutionPhase(bool isFadeIn)
    {
        Transform SemesterText = ResolutionPhase.transform.Find("Semester");
        if (SemesterText != null)
        {
            TextMeshProUGUI TextComponent = SemesterText.GetComponent<TextMeshProUGUI>();
            TextComponent.text = "Semester " + semesterManager.CurrentSemester;
        }

        if (isFadeIn)
        {
            ResolutionPhase.gameObject.SetActive(true);
            FadeIn(ResolutionPhase);
        }
        else
        {
            FadeOut(ResolutionPhase);
        }
    }

    #endregion

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