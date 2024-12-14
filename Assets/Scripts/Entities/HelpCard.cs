using System;
using UnityEngine;

public class HelpCard : MonoBehaviour
{
    // Enum untuk jenis Kartu Bantuan
    public enum AssistanceType
    {
        PengungkapanRumorAwal,
        PertukaranSaham,
        StabilisasiSaham,
        PenaltiPenghindaranPajak,
        SanksiOJK,
        EkuitasNegatif,
        InvestigasiRumor,
        Pengambilalihan
    }

    public int ID_Saham;  // Identifikasi unik untuk kartu bantuan
    public AssistanceType Tipe;  // Dropdown untuk memilih jenis kartu bantuan

    [TextArea(3, 5)]  // Membuat deskripsi muncul sebagai area teks multiline
    public string Deskripsi;  // Deskripsi dari kartu bantuan

    // Metode ini dijalankan ketika nilai di Inspector diubah
    void OnValidate()
    {
        // Memperbarui field Deskripsi berdasarkan AssistanceType yang dipilih
        switch (Tipe)
        {
            case AssistanceType.PengungkapanRumorAwal:
                Deskripsi = "Ungkap kartu rumor sebelum fase dimulai.";
                break;
            case AssistanceType.PertukaranSaham:
                Deskripsi = "Tukar saham dengan pemain lain.";
                break;
            case AssistanceType.StabilisasiSaham:
                Deskripsi = "Stabilkan harga saham saat harga jatuh.";
                break;
            case AssistanceType.PenaltiPenghindaranPajak:
                Deskripsi = "Terapkan penalti 2 poin untuk setiap saham atau sektor yang dipilih.";
                break;
            case AssistanceType.SanksiOJK:
                Deskripsi = "Harga saham turun 3 poin akibat sanksi OJK.";
                break;
            case AssistanceType.EkuitasNegatif:
                Deskripsi = "Harga saham turun 3 poin akibat laporan keuangan negatif.";
                break;
            case AssistanceType.InvestigasiRumor:
                Deskripsi = "Investigasi rumor di satu sektor dan temukan kebenarannya.";
                break;
            case AssistanceType.Pengambilalihan:
                Deskripsi = "Akuisisi saham lawan dengan setengah harga.";
                break;
        }
    }
}