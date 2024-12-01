using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string Name;       // Nama pemain
    public int Dice1Value;    // Nilai dadu pertama
    public int Dice2Value;    // Nilai dadu kedua
    public float TotalScore;    // Total nilai dari kedua dadu
    public int playOrder;    // Urutan main pemain
    public int Wealth;       // Kekayaan pemain

    [Header("Stock")]
    public int Konsumen = 0;  // Jumlah konsumen
    public int Mining = 0;     // Jumlah sumber daya tambang
    public int Industrial = 0; // Jumlah sumber daya industri
    public int Keuangan = 0;   // Jumlah sumber daya keuangan

    // Method untuk melempar dadu dan menghitung total nilai
    public void RollDice(int Dice1, int Dice2)
    {
        Dice1Value = Dice1;        // Menyimpan nilai dadu pertama
        Dice2Value = Dice2;        // Menyimpan nilai dadu kedua
        TotalScore = Dice1Value + Dice2Value; // Menghitung total nilai
    }

    // Method untuk mengatur urutan bermain pemain
    public void SetPlayOrder(int order)
    {
        playOrder = order; // Menyimpan urutan main pemain
    }
}