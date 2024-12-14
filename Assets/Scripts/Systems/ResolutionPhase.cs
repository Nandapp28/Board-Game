using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Pastikan Anda menggunakan DoTween

[System.Serializable]
public class HelpCards
{
    public GameObject helpCardDeck;
    public List<HelpCard> helpcard; // Tetap menggunakan List<HelpCard>
}

[System.Serializable]
public class CardSettings
{
    public Vector3 offsetFromCamera; // Offset dari kamera
    public float animationDuration; // Durasi animasi
    public Vector3 manualRotation; // Rotasi manual jika diperlukan
    public Vector3 targetScale; // Skala target untuk kartu
}

public class ResolutionPhase : MonoBehaviour
{
    #region Variabel And Property
    public HelpCards helpCards;
    public CardSettings cardSettings; // Tambahkan pengaturan kartu
    public HelpCard duplicatedHelpCard;

    public Camera Camera;
    public PlayerManager Players;
    public int currentPlayerIndex = 0;
    #endregion

    #region Unity Methods
    public void StartResolutionPhase()
    {
        Camera = Camera.main;
        Players = FindAnyObjectByType<PlayerManager>();
        Players.ShufflePlayers();
        CollectHelpCards();
        StartCoroutine(StartForNexPlayer());
    }
    #endregion

    public IEnumerator StartForNexPlayer()
    {
        yield return new WaitForSeconds(0.2f);
        Debug.Log("Current Player Index: " + currentPlayerIndex);
        Debug.Log("Player Count: " + Players.PlayerCount);
        if (currentPlayerIndex < Players.PlayerCount)
        {
            Debug.Log("Duplicating card for player: " + currentPlayerIndex);
            DuplicateRandomHelpCard();

            Debug.Log("Menunggu selama 10 detik sebelum melanjutkan ke pemain berikutnya...");
            yield return new WaitForSeconds(10f); // Tunggu selama 10 detik

            DestroyCard();
            MoveNextPlayer();
        }
        else
        {
            Debug.Log("No more players to process.");
        }
    }

    private void MoveNextPlayer()
    {
        currentPlayerIndex++;
        StartCoroutine(StartForNexPlayer());
    }

    private void DestroyCard()
    {
        if (duplicatedHelpCard != null)
        {
            Debug.Log("Destroying duplicated card.");
            Destroy(duplicatedHelpCard.gameObject);
            duplicatedHelpCard = null;
        }
    }

    #region Help Card Management
    private void CollectHelpCards()
    {
        helpCards.helpcard.Clear();
        foreach (Transform child in helpCards.helpCardDeck.transform)
        {
            HelpCard helpCardComponent = child.GetComponent<HelpCard>(); // Mengambil komponen HelpCard
            if (helpCardComponent != null) // Memeriksa apakah komponen HelpCard ada
            {
                helpCards.helpcard.Add(helpCardComponent); // Menambahkan komponen HelpCard ke list
            }
        }

        Debug.Log("Jumlah HelpCard yang ditemukan: " + helpCards.helpcard.Count);
    }

    private void DuplicateRandomHelpCard()
    {
        if (helpCards.helpcard.Count > 0) // Pastikan ada kartu dalam list
        {
            int randomIndex = Random.Range(0, helpCards.helpcard.Count); // Ambil indeks acak
            HelpCard randomHelpCard = helpCards.helpcard[randomIndex]; // Ambil kartu acak

            // Menduplikat objek HelpCard
            GameObject DuplicateObject = Instantiate(randomHelpCard.gameObject); // Membuat salinan baru dari objek HelpCard

            duplicatedHelpCard = DuplicateObject.GetComponent<HelpCard>();
            if (duplicatedHelpCard != null)
            {
                Debug.Log("HelpCard duplicated: " + duplicatedHelpCard.name);
            }
            else
            {
                Debug.LogError("Duplicated HelpCard component is null!");
            }

            // Pindahkan kartu yang telah diduplikat ke depan kamera
            MoveCardToCamera(DuplicateObject.transform);
        }
        else
        {
            Debug.LogWarning("Tidak ada HelpCard yang tersedia untuk diduplikasi.");
        }
    }
    #endregion

    #region Card Animation
    private void MoveCardToCamera(Transform cardTransform)
    {
        // Hitung posisi target agar selalu di depan kamera dengan offset
        Vector3 targetPosition = Camera.transform.position + Camera.transform.forward * cardSettings.offsetFromCamera.z +
                                 Camera.transform.right * cardSettings.offsetFromCamera.x +
                                 Camera.transform.up * cardSettings.offsetFromCamera.y;

        // Gunakan rotasi manual yang telah diatur
        Quaternion targetRotation = Quaternion.Euler(cardSettings.manualRotation);

        // Pindahkan kartu ke posisi target
        Debug.Log("Moving card to position: " + targetPosition);
        cardTransform.DOMove(targetPosition, cardSettings.animationDuration).SetEase(Ease.InOutQuad);
        cardTransform.DORotateQuaternion(targetRotation, cardSettings.animationDuration).SetEase(Ease.InOutQuad);

        // Atur skala kartu
        cardTransform.localScale = Vector3.zero; // Mulai dari skala 0
        cardTransform.DOScale(cardSettings.targetScale, cardSettings.animationDuration).SetEase(Ease.OutBack); // Animasi skala
    }
    #endregion
}
