using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StockPriceConfiguration
{
    public float offset;
    public Vector3 Rotation = new Vector3(0,-76.328f,0);
}

[System.Serializable]
public class AllSector
{
    public List<GameObject> Sector1;
    public List<GameObject> Sector2;
    public List<GameObject> Sector3;
    public List<GameObject> Sector4;
}

public class StockPriceManager : MonoBehaviour {
    public GameObject allParentStockPriceContainer;
    public List<GameObject> parentSectors;
    public GameObject availablePriceContainer;
    public List<GameObject> availablePrice;
    public AllSector allSector;
    public StockPriceConfiguration StockPriceConfiguration;

    private void Start() {
        Validate();
        CollectAllSectors();
        CollectavailablePrice();
        DistributeAvailblePrice();
    }

    private void Validate()
    {
        if(allParentStockPriceContainer == null)
        {
            Debug.Log("allParentStockPriceContainer null atau belum di update");
        }
    }

    private void CollectAllSectors() 
    {
        parentSectors.Clear();

        foreach(Transform child in allParentStockPriceContainer.transform)
        {
            parentSectors.Add(child.gameObject);
            DestroyStockPrice(child);
        }

        Debug.Log("ada " + parentSectors.Count + " sector");
    }

    private void DestroyStockPrice(Transform Object)
    {
        foreach (Transform child in Object)
        {
            Destroy(child.gameObject);
        }
    }

    void CollectavailablePrice()
    {
        availablePrice.Clear();
        
        foreach(Transform child in availablePriceContainer.transform)
        {
            availablePrice.Add(child.gameObject);
        }

        Debug.Log("ada " + availablePrice.Count + " Price");
    }

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

        List<List<GameObject>> sectors = new List<List<GameObject>>
        {
            allSector.Sector1,
            allSector.Sector2,
            allSector.Sector3,
            allSector.Sector4,
        };
        List<List<int>> allPriceInsectors = new List<List<int>>
        {
            new List<int>{1,2,3,5,6,7,8},
            new List<int>{1,2,4,5,6,7,9},
            new List<int>{1,3,4,5,6,7,9},
            new List<int>{2,4,5,7,9},
        };

        for (int i = 0; i < parentSectors.Count; i++)
        {
            GameObject Parent = parentSectors[i];

            int middleindex = allPriceInsectors[i].Count / 2;
            int middleValue = allPriceInsectors[i][middleindex];

            // Hitung posisi untuk setiap harga berdasarkan index
            for (int z = 0; z < allPriceInsectors[i].Count; z++)
            {
                int price = allPriceInsectors[i][z];
                StockPrice priceValue = availablePrice[price-1].GetComponent<StockPrice>();

                GameObject instantiatedStockPrice = Instantiate(priceValue.gameObject ,Parent.transform);
                instantiatedStockPrice.transform.localRotation = Quaternion.Euler(StockPriceConfiguration.Rotation);

                // Jika harga adalah nilai tengah, posisikan di tengah (Vector3.zero)
                if (price == middleValue)
                {
                    instantiatedStockPrice.transform.localPosition = Vector3.zero;
                }
                else
                {
                    // Hitung offset berdasarkan index
                    int offsetIndex = Mathf.Abs(middleindex - z); // Selisih antara index tengah dan index saat ini
                    float offset = offsetIndex * StockPriceConfiguration.offset;

                    // Posisi untuk harga lebih kecil dari nilai tengah (Positif Z)
                    if (price < middleValue)
                    {
                        instantiatedStockPrice.transform.localPosition = new Vector3(0, 0, offset);
                    }
                    // Posisi untuk harga lebih besar dari nilai tengah (Negatif Z)
                    else
                    {
                        instantiatedStockPrice.transform.localPosition = new Vector3(0, 0, -offset);
                    }
                }

                sectors[i].Add(instantiatedStockPrice);
            }
        }
    }

}