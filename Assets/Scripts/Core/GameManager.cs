using System;
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
        // Setelah fase bidding selesai, lanjutkan ke fase action
        biddingSystem.OnBiddingCompleted += StartActionSystem;
    }

    void StartActionSystem()
    {
        actionSystem.StartActionPhase();
    }
}
