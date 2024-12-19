using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompanyPerformanceConfiguration
{
    public float offset;
    public Vector3 Rotation = new Vector3(0, -76.328f, 0);
}

[System.Serializable]
public class AllSectorIn
{
    public SectorsIn Sector1; // Sektor pertama
    public SectorsIn Sector2; // Sektor kedua
    public SectorsIn Sector3; // Sektor ketiga
    public SectorsIn Sector4; // Sektor keempat
}

[System.Serializable]
public class SectorsIn
{
    public List<GameObject> Sector; // Daftar objek harga saham dalam sektor
    public int CurrenIndikatorIndex; // Indeks harga saat ini yang aktif
}

public class CompanyPerformanceManager : MonoBehaviour {
    public GameObject sectorParentContainer;
    public List<GameObject> Parent = new List<GameObject>();

    public GameObject AvailableIndikatorContainer;
    public List<GameObject> AvailableIndikator = new List<GameObject>();

    public AllSectorIn allSectorIn; // Inisialisasi allSectorIn

    public CompanyPerformanceConfiguration companyPerformanceConfiguration;

    public void StartCompanyProfile() {
        Validation();
        CollectParents();
        CollectAvailableIndicators();
        SetDefaultCurrentIndikatorIndex();
        DistributeAvailableIndicators();
    }

    void Validation()
    {
        if(sectorParentContainer == null)
        {
            Debug.Log("sectorParentContainer tidak boleh null");
        }
    }

    void CollectParents()
    {
        Parent.Clear();
        foreach (Transform child in sectorParentContainer.transform)
        {
            Parent.Add(child.gameObject);
            DestroyIndicator(child);
        }
    }

    void DestroyIndicator(Transform Object)
    {
        foreach (Transform child in Object)
        {
            Destroy(child.gameObject);
        }
    }

    void CollectAvailableIndicators()
    {
        AvailableIndikator.Clear();
        foreach (Transform child in AvailableIndikatorContainer.transform)
        {
            AvailableIndikator.Add(child.gameObject);
        }
    }

    void DistributeAvailableIndicators()
    {
        if (AvailableIndikator.Count == 0)
        {
            Debug.Log("AvailableIndikator Kosong");
            return;
        }
        if (Parent.Count == 0)
        {
            Debug.Log("Parent Kosong");
            return;
        }

        // Daftar sektor dan harga yang akan didistribusikan
        List<List<GameObject>> sectors = new List<List<GameObject>>
        {
            allSectorIn.Sector1.Sector,
            allSectorIn.Sector2.Sector,
            allSectorIn.Sector3.Sector,
            allSectorIn.Sector4.Sector
        };

        // Logika untuk mendistribusikan indikator ke sektor
        for (int i = 0; i < Parent.Count; i++)
        {
            GameObject Parents = Parent[i];

            for (int x = 0; x < AvailableIndikator.Count; x++)
            {
                int middleindex = AvailableIndikator.Count / 2;

                Company obj = AvailableIndikator[x].GetComponent<Company>();
                
                GameObject instantiatedindikator = Instantiate(obj.gameObject , Parents.transform);
                
                if (obj.value == middleindex)
                {
                    instantiatedindikator.transform.localPosition = new Vector3(0,-0.05f,0);
                    instantiatedindikator.transform.localRotation = Quaternion.Euler(companyPerformanceConfiguration.Rotation);
                }
                if (obj.value < middleindex)
                {
                    int offset = middleindex - obj.value;

                    instantiatedindikator.transform.localPosition = new Vector3(0,-0.05f, offset*companyPerformanceConfiguration.offset);
                    instantiatedindikator.transform.localRotation = Quaternion.Euler(companyPerformanceConfiguration.Rotation);
                }
                if (obj.value > middleindex)
                {
                    int offset = obj.value - middleindex;

                    instantiatedindikator.transform.localPosition = new Vector3(0,-0.05f, -offset*companyPerformanceConfiguration.offset);
                    instantiatedindikator.transform.localRotation = Quaternion.Euler(companyPerformanceConfiguration.Rotation);
                }


                sectors[i].Add(instantiatedindikator);
            }
        }
    }

    void SetDefaultCurrentIndikatorIndex()
    {
        int middleindex = AvailableIndikator.Count / 2;

        allSectorIn.Sector1.CurrenIndikatorIndex = middleindex;
        allSectorIn.Sector2.CurrenIndikatorIndex = middleindex;
        allSectorIn.Sector3.CurrenIndikatorIndex = middleindex;
        allSectorIn.Sector4.CurrenIndikatorIndex = middleindex;
    }

    void CurrenPriceIndikatorActived(SectorsIn sectors)
    {
        for (int i = 0; i < sectors.Sector.Count; i++)
        {
            GameObject obj = sectors.Sector[i];
            StockPrice price = obj.GetComponent<StockPrice>();

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetActive(i == sectors.CurrenIndikatorIndex);
            }
        }
    }

        // Meningkatkan indeks harga saat ini untuk sektor yang diberikan.
    public void IncreaseCurrentPriceIndex(SectorsIn sectors, int increment)
    {
        if (sectors.CurrenIndikatorIndex + increment < sectors.Sector.Count)
        {
            sectors.CurrenIndikatorIndex += increment;
            CurrenPriceIndikatorActived(sectors);
        }
    }

    // Mengurangi indeks harga saat ini untuk sektor yang diberikan.
    public void DecreaseCurrentPriceIndex(SectorsIn sectors, int decrement)
    {
        if (sectors.CurrenIndikatorIndex - decrement >= 0)
        {
            sectors.CurrenIndikatorIndex -= decrement;
            CurrenPriceIndikatorActived(sectors);
        }
    }
}