using UnityEngine;

public class StockCard : MonoBehaviour
{
    // Enum untuk jenis kartu saham
    public enum StockType
    {
        FlashBuy,      // Mengambil kartu aksi tambahan
        TradeFee,      // Biaya saat menjual saham
        StockSplit,    // Pembagian nilai saham
        InsiderTrade,  // Informasi dari rumor dalam sektor tertentu
        TenderOffer    // Mempengaruhi harga saham saat diungkapkan
    }

    // Enum untuk sektor yang terhubung dengan kartu saham
    public enum Sector
    {
        Infrastuctur,   // Infrastruktur
        Finance,      // Keuangan
        Mining,        // Pertambangan
        Consumen       // Konsumen
    }

    public StockType Type; // Tipe kartu saham
    [TextArea(2, 5)]
    public string Descriptions; // Deskripsi efek kartu
    public Sector Connected_Sectors; // Sektor yang terhubung dengan kartu

    // Memperbarui deskripsi berdasarkan tipe kartu saat validasi
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
            case StockType.TenderOffer:
                Descriptions = "Can affect stock prices positively or negatively when revealed.";
                break;
        }
    }

    // Mengaktifkan efek kartu untuk pemain yang diberikan
    public void ActivateEffect(Player player)
    {
        Debug.Log(player.Name + " activated the card: " + Type);
        switch (Type)
        {
            case StockType.FlashBuy:
                Debug.Log("Effect: Draw an additional card.");
                // Logika untuk menarik kartu tambahan di sini
                break;
            case StockType.TradeFee:
                Debug.Log("Effect: Apply trade fee logic.");
                // Logika untuk biaya perdagangan di sini
                break;
            case StockType.StockSplit:
                Debug.Log("Effect: Apply stock split logic.");
                // Logika untuk pembagian saham di sini
                break;
            case StockType.InsiderTrade:
                Debug.Log("Effect: Apply insider trading logic.");
                // Logika untuk perdagangan orang dalam di sini
                break;
            // Tambahkan logika untuk TenderOffer jika diperlukan
        }
    }
}