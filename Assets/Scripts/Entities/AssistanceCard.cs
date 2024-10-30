using System;
using UnityEngine;

public class AssistanceCard : MonoBehaviour
{
    // Enum for the Assistance Card types
    public enum AssistanceType
    {
        EarlyRumorReveal,
        ShareSwap,
        StockStabilization,
        TaxAvoidancePenalty,
        OJKSanction,
        NegativeEquity,
        RumorInvestigation,
        Takeover
    }

    public int ID_Stock;  // Unique identifier for the assistance card
    public AssistanceType Type;  // Dropdown to select the type of assistance card

    [TextArea(3, 5)]  // Makes the description appear as a multiline text area
    public string Descriptions;  // Description of the assistance card

    // This method runs when values in the Inspector are changed
    void OnValidate()
    {
        // Update the Descriptions field based on the selected AssistanceType
        switch (Type)
        {
            case AssistanceType.EarlyRumorReveal:
                Descriptions = "Reveal rumor cards before the phase begins.";
                break;
            case AssistanceType.ShareSwap:
                Descriptions = "Swap shares with another player.";
                break;
            case AssistanceType.StockStabilization:
                Descriptions = "Stabilize stock prices when they are falling.";
                break;
            case AssistanceType.TaxAvoidancePenalty:
                Descriptions = "Apply a 2-point penalty for each selected stock or sector.";
                break;
            case AssistanceType.OJKSanction:
                Descriptions = "Stock price drops by 3 points due to OJK sanctions.";
                break;
            case AssistanceType.NegativeEquity:
                Descriptions = "Stock price drops by 3 points due to negative financial reports.";
                break;
            case AssistanceType.RumorInvestigation:
                Descriptions = "Investigate rumors in one sector and find the truth.";
                break;
            case AssistanceType.Takeover:
                Descriptions = "Acquire opponent's shares at half the price.";
                break;
        }
    }
}
