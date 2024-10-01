using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BiddingSystem biddingSystem;

    void Start()
    {
        
        StartBiddingSystem();
    }

    void StartBiddingSystem()
    {
        biddingSystem.StartBidding();
    }
}
