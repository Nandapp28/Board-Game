using UnityEngine;

public enum CardType
{
    FlashBuy,
    TradeFee,
    StockSplit,
    InsiderTrade
}

public class ActionCard : MonoBehaviour
{
    public string CardSector;       // Name of the card
    public CardType cardType;     // Type of the card
    public string description;     // Description of the card's effect

    // Method to activate the card effect
    public void Activate(Player player)
    {
        switch (cardType)
        {
            case CardType.FlashBuy:
                Debug.Log(player.nama + " activated " + cardType + " : " + CardSector);
                // Implement effect for Flash Buy
                // Example: Allow drawing another card
                break;

            case CardType.TradeFee:
                Debug.Log(player.nama + " activated " + cardType + ": " + CardSector);
                // Implement effect for Trade Fee
                break;

            case CardType.StockSplit:
                Debug.Log(player.nama + " activated " + cardType + ": " + CardSector);
                // Implement effect for Stock Split
                break;

            case CardType.InsiderTrade:
                Debug.Log(player.nama + " activated " + cardType + ": " + CardSector);
                // Implement effect for Insider Trade
                break;

            default:
                Debug.Log("Unknown card type.");
                break;
        }
    }
}
