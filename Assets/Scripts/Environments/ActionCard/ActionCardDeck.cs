using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ActionCardDeck : MonoBehaviour
{
    [Header("Pengaturan Kamera")]
    public Transform cameraTransform; // Transformasi kamera
    public Vector3 offsetFromCamera = new Vector3(0, 0, 2); // Jarak offset dari kamera
    public Vector3 manualRotation = Vector3.zero; // Rotasi manual dari kartu

    [Header("Pengaturan Kartu")]
    public GameObject CardParent; // Objek induk yang berisi semua kartu sebagai anak
    public Transform newParent; // Induk baru untuk menyimpan kartu yang dipilih untuk ditampilkan
    public int rows = 2; // Jumlah baris
    public int columns = 5; // Jumlah kolom
    public float spacingX = 1.0f; // Jarak antar kartu pada sumbu X
    public float spacingY = 1.0f; // Jarak antar kartu pada sumbu Y
    public float spacingZ = 0.0f; // Jarak opsional pada sumbu Z

    [Header("Pengaturan Animasi")]
    public float animationDuration = 0.5f; // Durasi animasi kemunculan kartu
    public float cardDelay = 0.2f; // Jeda antara kemunculan kartu
    public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f); // Skala target dari kartu

    private List<Transform> allCards; // Daftar untuk menyimpan semua kartu anak
    public List<Transform> selectedCards; // Daftar untuk menyimpan kartu-kartu acak yang dipilih


    // Memulai dek kartu
    public void StartCardDeck()
    {
        // Inisialisasi daftar
        allCards = new List<Transform>();
        selectedCards = new List<Transform>();

        // Ambil semua anak dari CardParent dan tambahkan ke daftar
        int childCount = CardParent.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = CardParent.transform.GetChild(i);
            allCards.Add(child); // Tambahkan setiap anak ke daftar
        }

        // Pilih secara acak 10 kartu unik
        SelectRandomCards();

        // Posisikan newParent di depan kamera
        PositionNewParentInFrontOfCamera();

        // Mulai menampilkan kartu yang dipilih dalam grid dengan animasi
        StartCoroutine(GenerateCardGridWithAnimation());
    }

    // Pilih kartu secara acak
    private void SelectRandomCards()
    {
        // Buat daftar sementara untuk menyimpan kartu yang tersedia
        List<Transform> availableCards = new List<Transform>(allCards);

        // Pilih secara acak 10 kartu unik
        for (int i = 0; i < Mathf.Min(10, availableCards.Count); i++)
        {
            int randomIndex = Random.Range(0, availableCards.Count);
            Transform selectedCard = availableCards[randomIndex];
            selectedCards.Add(selectedCard);
            availableCards.RemoveAt(randomIndex); // Hapus kartu yang dipilih untuk menghindari duplikasi
        }
    }

    // Posisikan induk baru di depan kamera
    private void PositionNewParentInFrontOfCamera()
    {
        // Hitung posisi di depan kamera, memastikan posisinya terpusat
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * offsetFromCamera.z;

        // Pastikan grid terpusat secara vertikal relatif terhadap kamera
        targetPosition += cameraTransform.up * (offsetFromCamera.y + (rows * spacingY) / 2);

        newParent.position = targetPosition; // Set posisi induk baru
        newParent.rotation = Quaternion.Euler(manualRotation); // Set rotasi untuk induk baru
    }

    // Menghasilkan grid kartu dengan animasi
    private IEnumerator GenerateCardGridWithAnimation()
    {
        Vector3 centerOffset = CalculateCenterOffset(); // Hitung offset untuk memusatkan grid
        int cardIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (cardIndex >= selectedCards.Count) yield break; // Keluar jika tidak ada lagi kartu untuk ditampilkan
                Transform card = selectedCards[cardIndex++];
                card.SetParent(newParent, false); // Re-parent kartu ke newParent
                PositionCard(card, row, column, centerOffset); // Posisikan kartu di grid

                // Mendapatkan komponen ActionCardAnimation dan mengatur posisi dan skala inisial
                ActionCardAnimation cardAnimation = card.GetComponent<ActionCardAnimation>();
                if (cardAnimation != null)
                {
                    cardAnimation.SetInitialPosition(card.localPosition);
                    cardAnimation.SetInitialScale(targetScale);
                }

                StartCoroutine(AnimateCardAppearance(card)); // Animasi kemunculan kartu
                yield return new WaitForSeconds(cardDelay); // Tunggu sebelum kartu berikutnya
            }
        }
    }

    // Menghitung offset untuk memusatkan grid
    private Vector3 CalculateCenterOffset()
    {
        float totalWidth = (columns - 1) * spacingX; // Total lebar grid
        float totalHeight = (rows - 1) * spacingY;   // Total tinggi grid
        return new Vector3(totalWidth / 2, totalHeight / 2, spacingZ / 2); // Offset pusat
    }

    // Posisikan kartu dalam grid
    private void PositionCard(Transform card, int row, int column, Vector3 centerOffset)
    {
        // Posisikan kartu dalam ruang lokal relatif ke newParent
        Vector3 cardPosition = new Vector3(column * spacingX, -row * spacingY, 0) - centerOffset;
        card.localPosition = cardPosition; // Set posisi lokal relatif ke newParent
        card.localScale = Vector3.zero; // Set skala awal
        card.gameObject.SetActive(true); // Tampilkan kartu
    }

    // Animasi kemunculan kartu
    private IEnumerator AnimateCardAppearance(Transform card)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = card.localScale;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            card.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / animationDuration); // Animasi skala
            yield return null; // Tunggu frame berikutnya
        }

        card.localScale = targetScale; // Pastikan skala akhir sesuai
    }


    public void AnimateAndDestroyCard(Transform card)
    {
        // Pastikan kartu memiliki komponen ActionCardAnimation
        ActionCardAnimation cardAnimation = card.GetComponent<ActionCardAnimation>();
        
        if (cardAnimation != null)
        {
            // Panggil animasi menuju target
            cardAnimation.AnimateToTarget();

            // Buat sequence untuk animasi dengan jeda sebelum penghancuran
            Sequence sequence = DOTween.Sequence();
            sequence.Append(cardAnimation.transform.DOScale(cardAnimation.targetScale, cardAnimation.animationDuration))
                    .AppendInterval(3f) // Tambahkan jeda 1 detik
                    .OnComplete(() =>
                    {
                        Destroy(card.gameObject); // Hancurkan kartu setelah animasi dan jeda selesai
                    });
        }
        else
        {
            Debug.LogWarning("Kartu tidak memiliki komponen ActionCardAnimation.");
        }
    }

}
