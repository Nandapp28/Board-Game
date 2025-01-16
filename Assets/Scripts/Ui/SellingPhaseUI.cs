using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SellingPhaseUI : MonoBehaviour
{
    // Parent GameObject yang berisi sektor-sektor
    public GameObject sectorsParent;

    [System.Serializable]
    public class CategoryUI
    {
        // UI Elements
        [Header("UI Elements")]
        public TextMeshProUGUI countText;
        public TextMeshProUGUI CurrentStockText;
        public TextMeshProUGUI CurrentPriceSectorText;
        public Button plusButton;
        public Button minusButton;

        // Status Variables
        [Header("Status Variables")]
        public int count = 0;
        public int CurrentStock;
        public int CurrentPriceSector = 5;
    }

    public CategoryUI Infrastuktur = new CategoryUI();
    public CategoryUI Mining = new CategoryUI();
    public CategoryUI Consumen = new CategoryUI();
    public CategoryUI Finance = new CategoryUI();
    public TextMeshProUGUI TotalEarnings;
    private StockPriceManager stockPriceManager;

    #region Unity Methods


    public void StartSellingPhaseUI()
    {
        stockPriceManager = FindAnyObjectByType<StockPriceManager>();
        if (sectorsParent != null)
        {
            sectorsParent.gameObject.SetActive(true);
        }

        StartCoroutine(InitializeUIAfterDelay(0.3f));
    }

    private IEnumerator InitializeUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        CollectUIElements("Infrastuktur", ref Infrastuktur);
        CollectUIElements("Mining", ref Mining);
        CollectUIElements("Consumen", ref Consumen);
        CollectUIElements("Finance", ref Finance);

        InitializeCategory(Infrastuktur);
        InitializeCategory(Mining);
        InitializeCategory(Consumen);
        InitializeCategory(Finance);

        CurrentStock();
        CurrentMarket();
    }
    #endregion

    #region UI Element Collection
    void CollectUIElements(string sectorName, ref CategoryUI category)
    {
        Transform sectorTransform = sectorsParent.transform.Find(sectorName);
        if (sectorTransform != null)
        {
            category.countText = sectorTransform.Find("Count")?.GetComponent<TextMeshProUGUI>();
            category.CurrentStockText = sectorTransform.Find("CurrentStockText")?.GetComponent<TextMeshProUGUI>();
            category.CurrentPriceSectorText = sectorTransform.Find("CurrentMarket")?.GetComponent<TextMeshProUGUI>();
            category.plusButton = sectorTransform.Find("Plus")?.GetComponent<Button>();
            category.minusButton = sectorTransform.Find("Minus")?.GetComponent<Button>();
        }
        else
        {
            Debug.LogError("Sector not found: " + sectorName);
        }
    }

    void InitializeCategory(CategoryUI category)
    {
        category.plusButton.onClick.AddListener(() => UpdateCount(category, 1));
        category.minusButton.onClick.AddListener(() => UpdateCount(category, -1));
    }
    #endregion

    #region Stock Management
    public void CurrentStock()
    {
        Infrastuktur.CurrentStockText.text = Infrastuktur.CurrentStock.ToString();
        Mining.CurrentStockText.text = Mining.CurrentStock.ToString();
        Finance.CurrentStockText.text = Finance.CurrentStock.ToString();
        Consumen.CurrentStockText.text = Consumen.CurrentStock.ToString();
    }

    void CurrentMarket()
    {
        Infrastuktur.CurrentPriceSectorText.text = Infrastuktur.CurrentPriceSector.ToString();
        Mining.CurrentPriceSectorText.text = Mining.CurrentPriceSector.ToString();
        Finance.CurrentPriceSectorText.text = Finance.CurrentPriceSector.ToString();
        Consumen.CurrentPriceSectorText.text = Consumen.CurrentPriceSector.ToString();
    }

    void UpdateCount(CategoryUI category, int change)
    {
        buttonSoundEffect();
        int newCount = category.count + change;

        if (newCount >= 0 && newCount <= category.CurrentStock)
        {
            category.count = newCount;
            category.countText.text = category.count.ToString();

            CalculateTotalEarnings();
        }
    }

    public void ResetCounts()
    {
        ResetSectorCount(Infrastuktur);
        ResetSectorCount(Mining);
        ResetSectorCount(Finance);
        ResetSectorCount(Consumen);
        
        TotalEarnings.text = "$0";
    }

    private void ResetSectorCount(CategoryUI sector)
    {
        sector.count = 0;
        UpdateCountText(sector);
    }

    private void UpdateCountText(CategoryUI sector)
    {
        sector.countText.text = sector.count.ToString();
    }

    public void GetStockData(Player Player)
    {
        Infrastuktur.CurrentStock = Player.Infrastuctur;
        Mining.CurrentStock = Player.Mining;
        Consumen.CurrentStock = Player.Consumen;
        Finance.CurrentStock = Player.Finance;
    }
    #endregion

    #region Earnings Calculation
    public int CalculateTotalEarnings()
    {
        int total = 0;

        total += Infrastuktur.count * Infrastuktur.CurrentPriceSector;
        total += Mining.count * Mining.CurrentPriceSector;
        total += Consumen.count * Consumen.CurrentPriceSector;
        total += Finance.count * Finance.CurrentPriceSector;

        TotalEarnings.text = "$" + total.ToString();

        return total;
    }
    #endregion

    private void buttonSoundEffect()
    {
        AudioManagers.instance.PlaySoundEffect(0);
    }

    public void CollectCurrentPriceSector()
    {
        for (int i = 0; i < 4; i++)
        {
            UpdateCurrentPriceSector(i);
        }
    }

    private void UpdateCurrentPriceSector(int index)
    {
        List<List<int>> allPriceSectors = new List<List<int>>
        {
            new List<int> { 1, 2, 3, 5, 6, 7, 8 },
            new List<int> { 1, 2, 4, 5, 6, 7, 9 },
            new List<int> { 1, 3, 4, 5, 6, 7, 9 },
            new List<int> { 2, 4, 5, 7, 9 },
        };

        int sector = GetSector(index) ?? 0;
        int currentPrice = allPriceSectors[index][sector];

        switch (index)
        {
            case 0:
                Consumen.CurrentPriceSector = currentPrice;
                break;
            case 1:
                Infrastuktur.CurrentPriceSector = currentPrice;
                break;
            case 2:
                Finance.CurrentPriceSector = currentPrice;
                break;
            case 3:
                Mining.CurrentPriceSector = currentPrice;
                break;
        }

        Debug.Log(currentPrice);
    }

    private int? GetSector(int index)
    {
        switch (index)
        {
            case 0:
                return stockPriceManager.allSector.Consumen.CurrenPriceIndex;
            case 1:
                return stockPriceManager.allSector.Infrastuctur.CurrenPriceIndex;
            case 2:
                return stockPriceManager.allSector.Finance.CurrenPriceIndex;
            case 3:
                return stockPriceManager.allSector.Mining.CurrenPriceIndex;
            default:
                return null;
        }
    }

}