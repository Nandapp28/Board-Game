using System;
using UnityEngine;

public class StockCard : MonoBehaviour
{
    // Enum for the Stock Card types
    public enum StockType
    {
        FlashBuy,
        TradeFee,
        StockSplit,
        InsiderTrade,
        RumorCard
    }

    // Enum for the connected sectors
    public enum Sector
    {
        Economy,
        Industry,
        Mining,
        Consumer
    }

    public StockType Type;  // Will show up as a dropdown in Unity Inspector

    [TextArea(2, 5)] // Makes the description appear as a multiline text area
    public string Descriptions;  // This will be dynamically updated

    public Sector Connected_Sectors;  // Sector related to the stock

    // This method runs when values in the Inspector are changed
    void OnValidate()
    {
        // Update the Descriptions field based on the selected StockType
        switch (Type)
        {
            case StockType.FlashBuy:
                Descriptions = "Draw 2 additional action cards immediately.";
                break;
            case StockType.TradeFee:
                Descriptions = "Sell shares before the selling phase, but pay a fee of 1 unit.";
                break;
            case StockType.StockSplit:
                Descriptions = "Split the value of shares, granting additional shares.";
                break;
            case StockType.InsiderTrade:
                Descriptions = "Reveal a rumor card in a specific sector for insider information.";
                break;
            case StockType.RumorCard:
                Descriptions = "Can affect stock prices positively or negatively when revealed.";
                break;
        }
    }
}
