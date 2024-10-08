using UnityEngine;
using System.Collections;

public class ActionCardDeck : MonoBehaviour
{
    public Transform cameraTransform; // Transform kamera
    public Vector3 offsetFromCamera = new Vector3(0, 0, 2); // Jarak offset parent dari kamera (maju ke depan)
    public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f); // Skala target kartu
    public Vector3 manualRotation = new Vector3(0, 0, 0); // Manual rotation of the card

    public int rows = 2; // Jumlah baris
    public int columns = 5; // Jumlah kolom
    public float spacingX = 1.0f; // Jarak antar kartu dalam sumbu X
    public float spacingY = 1.0f; // Jarak antar kartu dalam sumbu Y
    public float spacingZ = 0.0f; // Jika Anda ingin menyusun kartu dalam sumbu Z (misalnya lapisan)

    public GameObject cardPrefab; // Prefab kartu
    private GameObject[] cards; // Array untuk menyimpan kartu

    public float animationDuration = 0.5f; // Durasi animasi munculnya kartu
    public float cardDelay = 0.2f; // Delay antar kartu muncul

    // Variabel untuk menyimpan nilai sebelumnya
    private int prevRows;
    private int prevColumns;
    private float prevSpacingX;
    private float prevSpacingY;

    private void Start()
    {
        // Inisialisasi array sesuai dengan jumlah kartu
        cards = new GameObject[rows * columns];

        // Set posisi parent di depan kamera
        PositionParentInFrontOfCamera();

        // Inisialisasi nilai cache
        CacheCurrentValues();

        // Membuat kartu-kartu dan menempatkannya dalam grid dengan animasi
        StartCoroutine(GenerateCardGridWithAnimation());
    }

    private void Update()
    {
        // Set posisi parent di depan kamera
        PositionParentInFrontOfCamera();

        // Hanya regenerate kartu jika ada perubahan pada properti layout
        if (ValuesChanged())
        {
            // Jika ada perubahan pada jumlah baris/kolom atau spacing
            ClearOldCards(); // Hapus kartu lama
            cards = new GameObject[rows * columns]; // Buat ulang array untuk menyimpan kartu baru
            StartCoroutine(GenerateCardGridWithAnimation());
            CacheCurrentValues(); // Simpan nilai-nilai yang baru untuk perbandingan di frame berikutnya
        }
    }

    private void PositionParentInFrontOfCamera()
    {
        // Hitung posisi target agar parent berada di depan kamera
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * offsetFromCamera.z +
                                 cameraTransform.right * offsetFromCamera.x +
                                 cameraTransform.up * offsetFromCamera.y;

        // Set posisi dan rotasi parent
        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(manualRotation);
    }

    private IEnumerator GenerateCardGridWithAnimation()
    {
        // Hitung offset untuk memusatkan grid kartu di tengah parent
        float totalWidth = (columns - 1) * spacingX; // Total lebar grid (sumbu X)
        float totalHeight = (rows - 1) * spacingY;   // Total tinggi grid (sumbu Y)
        float totalDepth = spacingZ;                 // Sumbu Z tetap diatur ke 0 kecuali ingin ada variasi kedalaman

        // Offset untuk menempatkan grid tepat di tengah parent pada semua sumbu
        Vector3 centerOffset = new Vector3(totalWidth / 2, totalHeight / 2, totalDepth / 2);

        // Loop untuk membuat kartu sesuai dengan baris dan kolom dengan animasi
        int cardIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                // Buat kartu baru dari prefab
                GameObject newCard = Instantiate(cardPrefab, transform);

                // Hitung posisi kartu relatif terhadap parent
                Vector3 cardPosition = new Vector3(column * spacingX, row * spacingY, 0) - centerOffset;

                // Set posisi kartu dan skala awal (Vector3.zero untuk animasi muncul)
                newCard.transform.localPosition = cardPosition; // Menggunakan localPosition agar diatur relatif terhadap parent
                newCard.transform.localScale = Vector3.zero; // Skala awal untuk animasi muncul

                // Simpan kartu ke dalam array
                cards[cardIndex] = newCard;

                // Mulai animasi muncul untuk kartu ini
                StartCoroutine(AnimateCardAppearance(newCard, targetScale));

                // Jeda sebelum kartu berikutnya muncul
                yield return new WaitForSeconds(cardDelay);
                cardIndex++;
            }
        }
    }

    private IEnumerator AnimateCardAppearance(GameObject card, Vector3 targetScale)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = card.transform.localScale;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            // Interpolasi skala dari 0 ke targetScale
            card.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / animationDuration);
            yield return null; // Lanjut ke frame berikutnya
        }

        // Pastikan skala akhir benar-benar sesuai target
        card.transform.localScale = targetScale;
    }

    // Fungsi untuk menghapus kartu lama
    private void ClearOldCards()
    {
        if (cards != null)
        {
            foreach (GameObject card in cards)
            {
                if (card != null)
                {
                    Destroy(card); // Hapus game object kartu
                }
            }
        }
    }

    // Fungsi untuk memeriksa apakah ada perubahan pada layout
    private bool ValuesChanged()
    {
        return rows != prevRows || columns != prevColumns || spacingX != prevSpacingX || spacingY != prevSpacingY;
    }

    // Fungsi untuk menyimpan nilai-nilai properti saat ini
    private void CacheCurrentValues()
    {
        prevRows = rows;
        prevColumns = columns;
        prevSpacingX = spacingX;
        prevSpacingY = spacingY;
    }
}
