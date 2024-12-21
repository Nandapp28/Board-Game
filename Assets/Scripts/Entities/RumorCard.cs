using System;
using UnityEngine;

public class RumorCard : MonoBehaviour
{
    // Enum for the Rumor Card types
    public enum RumorType
    {
        ExtraFee,
        TaxRevenue,
        CompetitiveTender,
        AssetRevaluation,
        Expansion,
        WageIncrease,
        RupiahDepreciation,
        EconomicReform,
        StockIssuance,
        StockBuyback,
        AuditBribery,
        ForensicAudit,
        FinancialCrisis,
        FinancialDeficit,
        Merger,
        EconomicRecession,
        TaxReduction,
        EconomicStimulus
    }

    // Enum for connected sectors
    public enum Sector
    {
        Finance,
        Infrastuctur,
        Mining,
        Consumen
    }

    public int ID_Stock;  // Unique identifier for the rumor card
    public RumorType Type;  // Dropdown to select the type of rumor

    [TextArea(3, 5)]  // Makes the description appear as a multiline text area
    public string Descriptions;  // Description based on selected rumor type

    public Sector Connected_Sectors;  // Sector impacted by the rumor
    public int PriceChanges;  // Change in price caused by the rumor

    // This method runs when values in the Inspector are changed
    void OnValidate()
    {
        // Update the Descriptions and PriceChanges based on the selected RumorType
        switch (Type)
        {
            case RumorType.ExtraFee:
                Descriptions = "Players pay 1 currency per stock in sector.";
                PriceChanges = 0;
                break;
            case RumorType.TaxRevenue:
                Descriptions = "Players pay 1 currency per turn order.";
                PriceChanges = 0;
                break;
            case RumorType.CompetitiveTender:
                Descriptions = "Increase stock price by 1 point.";
                PriceChanges = 1;
                break;
            case RumorType.AssetRevaluation:
                Descriptions = "Increase stock price by 1 point.";
                PriceChanges = 1;
                break;
            case RumorType.Expansion:
                Descriptions = "Increase stock price by 2 points.";
                PriceChanges = 2;
                break;
            case RumorType.WageIncrease:
                Descriptions = "Increase stock price by 2 points.";
                PriceChanges = 2;
                break;
            case RumorType.RupiahDepreciation:
                Descriptions = "Decrease stock price by 2 points.";
                PriceChanges = -2;
                break;
            case RumorType.EconomicReform:
                Descriptions = "Reset all stock prices to initial value.";
                PriceChanges = 0;  // Reset to initial value, no specific change
                break;
            case RumorType.StockIssuance:
                Descriptions = "Decrease stock price by 1 point. All shareholders receive 1 additional share.";
                PriceChanges = -1;
                break;
            case RumorType.StockBuyback:
                Descriptions = "Increase stock price by 1 point, then the bank buys player shares.";
                PriceChanges = 1;
                break;
            case RumorType.AuditBribery:
                Descriptions = "Decrease stock price by 2 points.";
                PriceChanges = -2;
                break;
            case RumorType.ForensicAudit:
                Descriptions = "Decrease stock price by 2 points.";
                PriceChanges = -2;
                break;
            case RumorType.FinancialCrisis:
                Descriptions = "Decrease stock price by 2 points.";
                PriceChanges = -2;
                break;
            case RumorType.FinancialDeficit:
                Descriptions = "Decrease stock price by 3 points.";
                PriceChanges = -3;
                break;
            case RumorType.Merger:
                Descriptions = "Increase stock price by 3 points.";
                PriceChanges = 3;
                break;
            case RumorType.EconomicRecession:
                Descriptions = "Decrease stock price by 1 point for all stocks above initial price.";
                PriceChanges = -1;
                break;
            case RumorType.TaxReduction:
                Descriptions = "Increase stock price by 2 points.";
                PriceChanges = 2;
                break;
            case RumorType.EconomicStimulus:
                Descriptions = "Increase stock price by 2 points.";
                PriceChanges = 2;
                break;
        }
    }
}
