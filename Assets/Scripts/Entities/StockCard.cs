using UnityEngine;

public class StockCard : MonoBehaviour
{
    public enum StockType
    {
        FlashBuy,
        TradeFee,
        StockSplit,
        InsiderTrade,
        RumorCard
    }

    public enum Sector
    {
        Economy,
        Industry,
        Mining,
        Consumer
    }

    public StockType Type;
    [TextArea(2, 5)]
    public string Descriptions;
    public Sector Connected_Sectors;

    void OnValidate()
    {
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
    public void ActivateEffect(Player player)
    {
        Debug.Log(player.nama + " activated the card: " + Type);
        switch (Type)
        {
            case StockType.FlashBuy:
                Debug.Log("Effect: Draw an additional card.");
                // Logic for drawing an additional card here
                break;
            case StockType.TradeFee:
                Debug.Log("Effect: Apply trade fee logic.");
                // Logic for trade fee here
                break;
            case StockType.StockSplit:
                Debug.Log("Effect: Apply stock split logic.");
                // Logic for stock split here
                break;
            case StockType.InsiderTrade:
                Debug.Log("Effect: Apply insider trading logic.");
                // Logic for insider trade here
                break;
        }
    }
}
