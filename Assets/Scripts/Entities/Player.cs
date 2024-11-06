using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string Name;       // Nama pemain
    public int Dice1Value;    // Nilai dadu pertama
    public int Dice2Value;    // Nilai dadu kedua
    public float TotalScore;    // Total nilai dari kedua dadu
    public int playOrder;    // Urutan main
    public int Wealth;
    private List<StockCard> SharesOwned;

    public void RollDice(int Dice1, int Dice2)
    {
        Dice1Value = Dice1;
        Dice2Value = Dice2;
        TotalScore = Dice1Value + Dice2Value;
    }

    public void SetPlayOrder(int order)
    {
        playOrder = order;
    }
}
