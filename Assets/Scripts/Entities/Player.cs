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

    #region Dice Methods
    // Method untuk melempar dadu dan menghitung total nilai
    public void RollDice(int Dice1, int Dice2)
    {
        Dice1Value = Dice1;        // Menyimpan nilai dadu pertama
        Dice2Value = Dice2;        // Menyimpan nilai dadu kedua
        TotalScore = Dice1Value + Dice2Value; // Menghitung total nilai
    }
    #endregion

    #region Play Order Methods
    // Method untuk mengatur urutan bermain pemain
    public void SetPlayOrder(int order)
    {
        playOrder = order; // Menyimpan urutan main pemain
    }
    #endregion

    #region Wealth Methods
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
    #endregion

    #region Resource Methods
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
    #endregion

    #region StockCard Effect Methods

    // Method untuk efek FlashBuy
    public void FlashBuy()
    {
        // Logika untuk mengambil kartu aksi tambahan
        Debug.Log(Name + " menggunakan efek FlashBuy: Mengambil kartu Saham tambahan.");
    }

    // Method untuk efek TradeFee
    public void TradeFee(int saleAmount)
    {
        // Logika untuk menghitung biaya saat menjual saham
        int fee = CalculateTradeFee(saleAmount);
        RemoveWealth(fee); // Mengurangi kekayaan sesuai biaya
        Debug.Log(Name + " menggunakan efek TradeFee: Biaya penjualan adalah " + fee);
    }

    // Method untuk efek StockSplit
    public void StockSplit(float splitRatio)
    {
        // Logika untuk pembagian nilai saham
        // Misalnya, jika splitRatio adalah 2, maka nilai saham menjadi setengah
        // Implementasikan logika sesuai kebutuhan game
        Debug.Log(Name + " menggunakan efek StockSplit: Nilai saham dibagi dengan rasio " + splitRatio);
    }

    // Method untuk efek InsiderTrade
    public void InsiderTrade(string sector)
    {
        // Logika untuk mendapatkan informasi dari rumor dalam sektor tertentu
        Debug.Log(Name + " menggunakan efek InsiderTrade: Mendapatkan informasi dari sektor " + sector);
    }

    // Method untuk efek TenderOffer
    public void TenderOffer(float priceImpact)
    {
        // Logika untuk mempengaruhi harga saham saat diungkapkan
        // Misalnya, jika priceImpact adalah 1.5, harga saham meningkat 50%
        Debug.Log(Name + " menggunakan efek TenderOffer: Harga saham dipengaruhi dengan faktor " + priceImpact);
    }

    // Method untuk menghitung biaya perdagangan
    private int CalculateTradeFee(int saleAmount)
    {
        // Misalnya, biaya perdagangan adalah 5% dari jumlah penjualan
        return Mathf.CeilToInt(saleAmount * 0.05f);
    }

    #endregion
}