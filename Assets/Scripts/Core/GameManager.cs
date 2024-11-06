using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BiddingSystem biddingSystem;
    public ActionSystem actionSystem;

    void Start()
    {
        StartBiddingSystem();
    }

    void StartBiddingSystem()
    {
        biddingSystem.StartBidding();
        biddingSystem.OnBiddingCompleted += StartActionSystem;
    }

    void StartActionSystem()
    {
        actionSystem.StartActionPhase();
    }
}
