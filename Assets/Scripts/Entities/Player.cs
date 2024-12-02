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
    public int Consumen = 0;  // Jumlah konsumen
    public int Mining = 0;     // Jumlah sumber daya tambang
    public int Industrial = 0; // Jumlah sumber daya industri
    public int Finance = 0;   // Jumlah sumber daya keuangan

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

        // Method untuk menambah kekayaan
    public void AddWealth(int amount)
    {
        Wealth += amount; // Menambah kekayaan
    }

    // Method untuk mengurangi kekayaan
    public void RemoveWealth(int amount)
    {
        if (Wealth >= amount)
        {
            Wealth -= amount; // Mengurangi kekayaan
        }
    }

    // Method untuk menambah sumber daya tambang
    public void AddMining(int amount)
    {
        Mining += amount; // Menambah jumlah sumber daya tambang
    }

    // Method untuk mengurangi sumber daya tambang
    public void RemoveMining(int amount)
    {
        if (Mining >= amount)
        {
            Mining -= amount; // Mengurangi jumlah sumber daya tambang
        }
    }

    // Method untuk menambah sumber daya industri
    public void AddIndustrial(int amount)
    {
        Industrial += amount; // Menambah jumlah sumber daya industri
    }

    // Method untuk mengurangi sumber daya industri
    public void RemoveIndustrial(int amount)
    {
        if (Industrial >= amount)
        {
            Industrial -= amount; // Mengurangi jumlah sumber daya industri
        }
    }

    // Method untuk menambah sumber daya keuangan
    public void AddFinance(int amount)
    {
        Finance += amount; // Menambah jumlah sumber daya keuangan
    }

    // Method untuk mengurangi sumber daya keuangan
    public void RemoveFinance(int amount)
    {
        if (Finance >= amount)
        {
            Finance -= amount; // Mengurangi jumlah sumber daya keuangan
        }
    }

    // Method untuk menambah jumlah konsumen
    public void AddConsumen(int amount)
    {
        Consumen += amount; // Menambah jumlah konsumen
    }

    // Method untuk mengurangi jumlah konsumen
    public void RemoveConsumen(int amount)
    {
        if (Consumen >= amount)
        {
            Consumen -= amount; // Mengurangi jumlah konsumen
        }
    }
}