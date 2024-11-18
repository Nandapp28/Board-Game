using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject variantScreen;
    [SerializeField] private GameObject selectPlayerScreen;
    [SerializeField] private GameObject numbers;
    private readonly List<GameObject> number = new();

    [Header("Animation Settings")]
    [SerializeField] private Vector2 startScale = Vector2.zero;
    [SerializeField] private Vector2 endScale = Vector2.one;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease openEase = Ease.OutBounce;
    [SerializeField] private Ease closeEase = Ease.InBounce;

    [Header("Scene Settings")]
    [SerializeField] private int Scene;

    private RectTransform variantScreenRect;
    private RectTransform selectPlayerScreenRect;

    private void Awake()
    {
        InitializeScreens();
    }

    private void InitializeScreens()
    {
        variantScreenRect = ValidateAndResetScreen(variantScreen);
        selectPlayerScreenRect = ValidateAndResetScreen(selectPlayerScreen);

        CollectChildButtons();
    }

    private void CollectChildButtons()
    {
        for (int i = 0; i < numbers.transform.childCount; i++)
        {
            GameObject child = numbers.transform.GetChild(i).gameObject;
            number.Add(child);

            InitializeChildButton(child, i);
        }
    }

    private void InitializeChildButton(GameObject buttonObject, int index)
    {
        SetCheckState(buttonObject, false);

        Button button = buttonObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnButtonClicked(index));
        }
    }

    private void OnButtonClicked(int clickedIndex)
    {
        for (int i = 0; i < number.Count; i++)
        {
            bool isActive = i == clickedIndex;
            SetCheckState(number[i], isActive);
        }
    }

    private void SetCheckState(GameObject buttonObject, bool state)
    {
        Transform checkTransform = buttonObject.transform.Find("check");
        if (checkTransform != null)
        {
            checkTransform.gameObject.SetActive(state);
        }
    }

    private RectTransform ValidateAndResetScreen(GameObject screen)
    {
        if (screen == null)
        {
            Debug.LogError($"Screen is not assigned in the Inspector.");
            return null;
        }

        var rectTransform = screen.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError($"RectTransform component is missing on {screen.name}.");
            return null;
        }

        ResetScreenState(rectTransform);
        return rectTransform;
    }

    private void ResetScreenState(RectTransform screenRect)
    {
        if (screenRect == null) return;

        screenRect.localScale = startScale;
        screenRect.gameObject.SetActive(false);
    }

    public void ShowGameModeScreen()
    {
        SwitchScreens(variantScreen, variantScreenRect, openEase, selectPlayerScreen);
    }

    public void ShowVariantScreen()
    {
        SwitchScreens(selectPlayerScreen, selectPlayerScreenRect, openEase, variantScreen);
    }

    public void CloseVariantScreen()
    {
        AnimateScreenClose(variantScreen, variantScreenRect);
    }

    public void CloseSelectPlayerScreen()
    {
        AnimateScreenClose(selectPlayerScreen, selectPlayerScreenRect);
    }

    private void SwitchScreens(GameObject toActivate, RectTransform toActivateRect, Ease ease, GameObject toDeactivate)
    {
        AnimateScreenClose(toDeactivate, toDeactivate.GetComponent<RectTransform>());
        AnimateScreenOpen(toActivate, toActivateRect, ease);
    }

    private void AnimateScreenOpen(GameObject screen, RectTransform rectTransform, Ease ease)
    {
        if (rectTransform == null) return;

        screen.SetActive(true);
        rectTransform.DOScale(endScale, animationDuration).SetEase(ease);
    }

    private void AnimateScreenClose(GameObject screen, RectTransform rectTransform)
    {
        if (rectTransform == null) return;

        rectTransform.DOScale(startScale, animationDuration).SetEase(closeEase)
            .OnComplete(() => screen.SetActive(false));
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(Scene);
    }
}
