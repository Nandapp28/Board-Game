using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionPhaseUI : MonoBehaviour {

    public Button Buys;
    public Button Skip;
    public GameObject HelpCard;

    public GameObject ParentToken;
    public GameObject ParentDividen;

    private GameObject Token;
    private GameObject Dividen;
    private ResolutionPhase resolutionPhase;

    private void Start() {
        resolutionPhase = FindAnyObjectByType<ResolutionPhase>();
        CollectHelpcard();
        Collect(ParentToken,Token);
        Collect(ParentDividen,Dividen);
        
        ParentDividen.SetActive(true);
        ParentToken.SetActive(true);
        HelpCard.SetActive(true);
        ParentDividen.transform.localScale = Vector3.zero;
        ParentToken.transform.localScale = Vector3.zero;
        HelpCard.transform.localScale = Vector3.zero;

        Buys.onClick.AddListener(resolutionPhase.BuyButton);
        Skip.onClick.AddListener(resolutionPhase.SkipButton);
    }

    public void Collect(GameObject obj,GameObject children)
    {
        foreach (Transform child in obj.transform)
        {
            children = child.gameObject;
        }
    }

    public void CollectHelpcard()
    {
        foreach (Transform child in HelpCard.transform)
        {
            if(child.gameObject.name == "Buys")
            {
                Buys = child.GetComponent<Button>();
            }
            if(child.gameObject.name == "Skip")
            {
                Skip = child.GetComponent<Button>();
            }
        }
    }

    private void AnimateObj(GameObject obj)
    {
        obj.transform.DOScale(Vector3.one,0.6f);
    }
    private void ResetAnimateObj(GameObject obj)
    {
        obj.transform.DOScale(Vector3.zero,0.6f);
    }

    public void HandleToken(bool IsScale)
    {
        if(IsScale)
        {
            AnimateObj(ParentToken);
        }else{
            ResetAnimateObj(ParentToken);
        }
    }
    public void HandleDividen(bool IsScale)
    {
        if(IsScale)
        {
            AnimateObj(ParentDividen);
        }else{
            ResetAnimateObj(ParentDividen);
        }
    }
    public void HandleHelpCard(int index)
    {
        if(index == 1)
        {
            HelpCard.SetActive(true);
            AnimateObj(HelpCard);
        }else{
            ResetAnimateObj(HelpCard);
        }
    }
}