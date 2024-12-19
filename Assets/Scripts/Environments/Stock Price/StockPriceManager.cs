using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StockPriceConfiguration
{
    public float offset; // Offset untuk posisi harga saham
    public Vector3 Rotation = new Vector3(0, -76.328f, 0); // Rotasi default untuk harga saham
}

[System.Serializable]
public class AllSector
{
    public Sectors Consumen; // Sektor pertama
    public Sectors Infrastuctur; // Sektor kedua
    public Sectors Finance; // Sektor ketiga
    public Sectors Mining; // Sektor keempat
}

[System.Serializable]
public class Sectors
{
    public List<GameObject> Sector; // Daftar objek harga saham dalam sektor
    public int CurrenPriceIndex; // Indeks harga saat ini yang aktif
}

public class StockPriceManager : MonoBehaviour
{
    #region Fields
    public GameObject allParentStockPriceContainer; // Kontainer untuk semua sektor harga saham
    public List<GameObject> parentSectors; // Daftar sektor yang ada
    public GameObject availablePriceContainer; // Kontainer untuk harga yang tersedia
    public List<GameObject> availablePrice; // Daftar harga yang tersedia
    public AllSector allSector; // Semua sektor yang ada
    public StockPriceConfiguration StockPriceConfiguration; // Konfigurasi harga saham
    #endregion

    #region Unity Methods
    // Metode ini dipanggil saat objek pertama kali diinisialisasi. 
    // Ini melakukan validasi, mengumpulkan semua sektor, mengumpulkan harga yang tersedia, 
    // mendistribusikan harga yang tersedia, mengatur indeks harga saat ini ke default, 
    // dan memperbarui harga saat ini.
    public void StartStockManager()
    {
        Validate();
        CollectAllSectors();
        CollectavailablePrice();
        DistributeAvailblePrice();
        SetDefaultCurrentPriceIndex();
        CurrentPrice();
    }
    #endregion

    #region Validation
    // Metode ini memeriksa apakah allParentStockPriceContainer telah diatur. 
    // Jika tidak, akan mencetak pesan debug.
    private void Validate()
    {
        if (allParentStockPriceContainer == null)
        {
            Debug.Log("allParentStockPriceContainer null atau belum di update");
        }
    }
    #endregion

    #region Collecting Data
    // Metode ini mengumpulkan semua sektor dari allParentStockPriceContainer 
    // dan menghapus semua harga saham yang ada di dalamnya.
    private void CollectAllSectors()
    {
        parentSectors.Clear();

        foreach (Transform child in allParentStockPriceContainer.transform)
        {
            parentSectors.Add(child.gameObject);
            DestroyStockPrice(child);
        }

        Debug.Log("ada " + parentSectors.Count + " sector");
    }

    // Metode ini menghapus semua objek anak dari objek yang diberikan 
    // untuk membersihkan harga saham yang ada.
    private void DestroyStockPrice(Transform Object)
    {
        foreach (Transform child in Object)
        {
            Destroy(child.gameObject);
        }
    }

    // Metode ini mengumpulkan semua harga yang tersedia dari availablePriceContainer 
    // dan menyimpannya dalam daftar availablePrice.
    void CollectavailablePrice()
    {
        availablePrice.Clear();

        foreach (Transform child in availablePriceContainer.transform)
        {
            availablePrice.Add(child.gameObject);
        }

        Debug.Log("ada " + availablePrice.Count + " Price");
    }
    #endregion

    #region Price Distribution
    // Metode ini mendistribusikan harga yang tersedia ke sektor-sektor yang ada. 
    // Ini memeriksa apakah daftar harga dan sektor tidak kosong sebelum melanjutkan.
    void DistributeAvailblePrice()
    {
        if (availablePrice.Count == 0)
        {
            Debug.Log("availablePrice Kosong");
            return;
        }
        if (parentSectors.Count == 0)
        {
            Debug.Log("parentSectors Kosong");
            return;
        }

        // Daftar sektor dan harga yang akan didistribusikan
        List<List<GameObject>> sectors = new List<List<GameObject>>
        {
            allSector.Consumen.Sector,
            allSector.Infrastuctur.Sector,
            allSector.Finance.Sector,
            allSector.Mining.Sector,
        };
        List<List<int>> allPriceInsectors = new List<List<int>>
        {
            new List<int>{1,2,3,5,6,7,8},
            new List<int>{1,2,4,5,6,7,9},
            new List<int>{1,3,4,5,6,7,9},
            new List<int>{2,4,5,7,9},
        };

        // Mendis ```csharp
        // Mendistribusikan harga ke setiap sektor berdasarkan indeks harga
        for (int i = 0; i < parentSectors.Count; i++)
        {
            GameObject Parent = parentSectors[i];

            int middleindex = allPriceInsectors[i].Count / 2;
            int middleValue = allPriceInsectors[i][middleindex];

            for (int z = 0; z < allPriceInsectors[i].Count; z++)
            {
                int price = allPriceInsectors[i][z];
                StockPrice priceValue = availablePrice[price - 1].GetComponent<StockPrice>();

                GameObject instantiatedStockPrice = Instantiate(priceValue.gameObject, Parent.transform);
                instantiatedStockPrice.transform.localRotation = Quaternion.Euler(StockPriceConfiguration.Rotation);

                if (price == middleValue)
                {
                    instantiatedStockPrice.transform.localPosition = Vector3.zero;
                }
                else
                {
                    int offsetIndex = Mathf.Abs(middleindex - z);
                    float offset = offsetIndex * StockPriceConfiguration.offset;

                    if (price < middleValue)
                    {
                        instantiatedStockPrice.transform.localPosition = new Vector3(0, 0, offset);
                    }
                    else
                    {
                        instantiatedStockPrice.transform.localPosition = new Vector3(0, 0, -offset);
                    }
                }

                sectors[i].Add(instantiatedStockPrice);
            }
        }
    }
    #endregion

    #region Current Price Management
    // Metode ini memperbarui harga saat ini untuk semua sektor.
    void CurrentPrice()
    {
        UpdateSector1();
        UpdateSector2();
        UpdateSector3();
        UpdateSector4();
    }

    // Memperbarui harga untuk sektor pertama.
    public void UpdateSector1()
    {
        CurrenPriceIndexActived(allSector.Consumen);
    }

    // Memperbarui harga untuk sektor kedua.
    public void UpdateSector2()
    {
        CurrenPriceIndexActived(allSector.Infrastuctur);
    }

    // Memperbarui harga untuk sektor ketiga.
    public void UpdateSector3()
    {
        CurrenPriceIndexActived(allSector.Finance);
    }

    // Memperbarui harga untuk sektor keempat.
    public void UpdateSector4()
    {
        CurrenPriceIndexActived(allSector.Mining);
    }

    // Mengaktifkan harga saat ini berdasarkan indeks yang aktif untuk sektor yang diberikan.
    void CurrenPriceIndexActived(Sectors sectors)
    {
        for (int i = 0; i < sectors.Sector.Count; i++)
        {
            GameObject obj = sectors.Sector[i];
            StockPrice price = obj.GetComponent<StockPrice>();

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetActive(i == sectors.CurrenPriceIndex);
            }
        }
    }

    // Meningkatkan indeks harga saat ini untuk sektor yang diberikan.
    public void IncreaseCurrentPriceIndex(Sectors sectors, int increment)
    {
        if (sectors.CurrenPriceIndex + increment < sectors.Sector.Count)
        {
            sectors.CurrenPriceIndex += increment;
            CurrenPriceIndexActived(sectors);
        }
    }

    // Mengurangi indeks harga saat ini untuk sektor yang diberikan.
    public void DecreaseCurrentPriceIndex(Sectors sectors, int decrement)
    {
        if (sectors.CurrenPriceIndex - decrement >= 0)
        {
            sectors.CurrenPriceIndex -= decrement;
            CurrenPriceIndexActived(sectors);
        }
    }

    // Mengatur indeks harga saat ini ke nilai default untuk semua sektor.
    private void SetDefaultCurrentPriceIndex()
    {
        SetCurrentPriceIndex(allSector.Consumen);
        SetCurrentPriceIndex(allSector.Infrastuctur);
        SetCurrentPriceIndex(allSector.Finance);
        SetCurrentPriceIndex(allSector.Mining);
    }

    // Mengatur indeks harga saat ini ke indeks tengah untuk sektor yang diberikan.
    private void SetCurrentPriceIndex(Sectors sector)
    {
        if (sector.Sector.Count > 0)
        {
            sector.CurrenPriceIndex = sector.Sector.Count / 2; // Mengatur ke indeks tengah
        }
    }
    #endregion

}