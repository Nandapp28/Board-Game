using UnityEngine;

public enum ActionCardType
{
    FlashBuy,
    TradeFee,
    InsiderTrade,
    StockSplit,
    Penalty,
    Special // Add more types as necessary
}

[System.Serializable]
public class ActionCard
{
    public string cardName;
    public ActionCardType cardType;

    public ActionCard(string name, ActionCardType type)
    {
        cardName = name;
        cardType = type;
    }
}
