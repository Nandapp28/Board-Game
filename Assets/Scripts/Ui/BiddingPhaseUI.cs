using DG.Tweening;
using TMPro;
using UnityEngine;

public class BiddingPhaseUI : MonoBehaviour {
    public GameObject DiceResultParent;
    public Vector3 scale;
    public float duration;
    private Vector3 initScale = Vector3.zero;

    private void Start() {
        DiceResultParent.transform.localScale = initScale;
    }

    public void resultText(int total)
    {
        Transform TextResult = DiceResultParent.transform.Find("DiceResult");
        if(TextResult != null)
        {
            TextResult.GetComponent<TextMeshProUGUI>().text = "YOU’VE GOT " + total.ToString() + " SCORE !";
        }
    }

    public void AnimateActive()
    {
        DiceResultParent.SetActive(true);
        DiceResultParent.transform.DOScale(scale,duration);
    }
    public void AnimateDeactive()
    {
        DiceResultParent.transform.DOScale(initScale,duration);
    }
}