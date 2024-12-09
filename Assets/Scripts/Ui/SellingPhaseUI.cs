using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections; // Pastikan untuk menambahkan namespace ini

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

    public void StartSellingPhaseUI()
    {
        // Memastikan sektorParent diaktifkan saat permainan dimulai
        if (sectorsParent != null)
        {
            sectorsParent.gameObject.SetActive(true);  // Mengaktifkan objek
        }

        // Memulai coroutine untuk menunggu 1 detik
        StartCoroutine(InitializeUIAfterDelay(1f));
    }

    private IEnumerator InitializeUIAfterDelay(float delay)
    {
        // Menunggu selama delay yang ditentukan
        yield return new WaitForSeconds(delay);

        // Mengumpulkan elemen UI berdasarkan sektor dalam parent
        CollectUIElements("Infrastuktur", ref Infrastuktur);
        CollectUIElements("Mining", ref Mining);
        CollectUIElements("Consumen", ref Consumen);
        CollectUIElements("Finance", ref Finance);

        // Inisialisasi listener tombol
        InitializeCategory(Infrastuktur);
        InitializeCategory(Mining);
        InitializeCategory(Consumen);
        InitializeCategory(Finance);

        CurrentStock();
    }

    void CollectUIElements(string sectorName, ref CategoryUI category)
    {
        // Mencari sektor berdasarkan nama dan mencari elemen-elemen di dalamnya
        Transform sectorTransform = sectorsParent.transform.Find(sectorName);
        if (sectorTransform != null)
        {
            category.countText = sectorTransform.Find("Count")?.GetComponent<TextMeshProUGUI>();
            category.CurrentStockText = sectorTransform.Find("CurrentStockText")?.GetComponent<TextMeshProUGUI>();
            // category.CurrentPriceSectorText = sectorTransform.Find("CurrentPriceSectorText")?.GetComponent<TextMeshProUGUI>();
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
        // Menambahkan listener pada tombol plus dan minus
        category.plusButton.onClick.AddListener(() => UpdateCount(category, 1));
        category.minusButton.onClick.AddListener(() => UpdateCount(category, -1));
    }

    void CurrentStock()
    {
        Infrastuktur.CurrentStockText.text = Infrastuktur.CurrentStock.ToString();
        Mining.CurrentStockText.text = Mining.CurrentStock.ToString();
        Finance.CurrentStockText.text = Finance.CurrentStock.ToString();
        Consumen.CurrentStockText.text = Consumen.CurrentStock.ToString();
    }

    void UpdateCount(CategoryUI category, int change)
    {
        // Menghitung nilai baru
        int newCount = category.count + change;

        // Memastikan count tidak kurang dari nol dan tidak lebih dari CurrentStock
        if (newCount >= 0 && newCount <= category.CurrentStock)
        {
            category.count = newCount;
            category.countText.text = category.count.ToString();
        }
    }

    public void ResetCounts()
    {
        ResetSectorCount(Infrastuktur);
        ResetSectorCount(Mining);
        ResetSectorCount(Finance);
        ResetSectorCount(Consumen);
    }

    private void ResetSectorCount(CategoryUI sector)
    {
        sector.count = 0; // Reset the count
        UpdateCountText(sector); // Update the corresponding text
    }

    private void UpdateCountText(CategoryUI sector)
    {
        sector.countText.text = sector.count.ToString(); // Update the UI text
    }

    public void GetStockData(Player Player)
    {
        Infrastuktur.CurrentStock = Player.Infrastuktur;
        Mining.CurrentStock = Player.Mining;
        Consumen.CurrentStock = Player.Consumen;
        Finance.CurrentStock = Player.Finance;
    }
}