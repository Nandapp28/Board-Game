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
        public TextMeshProUGUI countText;
        public Button plusButton;
        public Button minusButton;
        public int count = 0;
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
    }

    void CollectUIElements(string sectorName, ref CategoryUI category)
    {
        // Mencari sektor berdasarkan nama dan mencari elemen-elemen di dalamnya
        Transform sectorTransform = sectorsParent.transform.Find(sectorName);
        if (sectorTransform != null)
        {
            category.countText = sectorTransform.Find("Count")?.GetComponent<TextMeshProUGUI>();
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

    void UpdateCount(CategoryUI category, int change)
    {
        // Menghitung nilai baru
        int newCount = category.count + change;

        // Memastikan count tidak kurang dari nol
        if (newCount >= 0)
        {
            category.count = newCount;
            category.countText.text = category.count.ToString();
        }
    }
}