
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject StockCards;
    public List<GameObject> AllCards = new List<GameObject>();

    [Header("Camera Settings")]
    public Camera Camera;
    public Vector3 offsetFromCamera = new Vector3(0, 0, 5);
    
    [Header("Card Spacing")]
    public float horizontalSpacing = 2.0f;
    public float verticalSpacing = 1.0f;

    [Header("Card Rotation")]
    public Vector3 manualRotation = new Vector3(0, 90, 0);

    [Header("Game Settings")]
    public int numberOfPlayers = 3; // Jumlah pemain
    public float animationDuration = 1.0f;

    [Header("Card Animation")]
    public CardAnimation currentActiveCard = null;
    public List<GameObject> selectedCards = new List<GameObject>();

    public GameObject canvas;

    private ActionPhase ActionPhase;

    void Start()
    {
        ActionPhase = FindAnyObjectByType<ActionPhase>();
        ActionPhase.ActiveButton.onClick.AddListener(OnActiveButtonClicked);
    }

    public void StartStockCards()
    {
        CollectingCards();
        ShadowBackground(Camera.main,canvas);
        StartCoroutine(ShowRandomCards());
    }

    void CollectingCards()
    {
        AllCards.Clear();

        foreach (Transform child in StockCards.transform)
        {
            AllCards.Add(child.gameObject);
            
            // Tambahkan komponen CardAnimation ke setiap child
            if (child.GetComponent<CardAnimation>() == null)
            {
                CardAnimation cardAnimation = child.gameObject.AddComponent<CardAnimation>();
                cardAnimation.cardManager = this;
            }
        }
    }

    public IEnumerator ShowRandomCards()
    {
        // Hitung jumlah kartu yang akan ditampilkan
        int numberOfCardsToShow = Mathf.Min(numberOfPlayers * 2, AllCards.Count); // 2 kartu per pemain
        int cardsToShow = Mathf.Min(numberOfCardsToShow, AllCards.Count);

        selectedCards.Clear();

        for (int i = 0; i < cardsToShow; i++)
        {
            int randomIndex = Random.Range(0, AllCards.Count);
            selectedCards.Add(AllCards[randomIndex]);
            AllCards.RemoveAt(randomIndex);
        }

        int columns = cardsToShow / 2; 
        int rows = 2; // Menghitung baris berdasarkan jumlah kartu

        float offsetX = (columns - 1) * horizontalSpacing / 2;
        float offsetY = (rows - 1) * verticalSpacing / 2;

        for (int i = 0; i < selectedCards.Count; i++)
        {
            GameObject card = selectedCards[i];

            int column = columns - 1 - (i % columns);
            int row = i / columns;

            Vector3 localoffsetFromCamera = new Vector3(column * horizontalSpacing - offsetX + offsetFromCamera.x, -row * verticalSpacing + offsetY +offsetFromCamera.y, offsetFromCamera.z);
            Vector3 targetPosition = Camera.transform.position + Camera.transform.forward * localoffsetFromCamera.z +
                                    Camera.transform.right * localoffsetFromCamera.x +
                                    Camera.transform.up * localoffsetFromCamera.y;

            Quaternion targetRotation = Quaternion.Euler(manualRotation);

            card.transform.DOMove(targetPosition, animationDuration).SetEase(Ease.InOutQuad);
            card.transform.DORotateQuaternion(targetRotation, animationDuration).SetEase(Ease.InOutQuad);

            card.SetActive(true);

            yield return new WaitForSeconds(animationDuration);
            CardAnimation cardAnimation = card.GetComponent<CardAnimation>();
            cardAnimation.Initial();
        }
    }

    public void HandleCardClick(CardAnimation clickedCard)
    {
        // Jika tidak ada kartu aktif saat ini
        if (currentActiveCard == null)
        {
            // Aktifkan kartu yang diklik
            currentActiveCard = clickedCard;
            
            // Nonaktifkan mouse click untuk semua kartu kecuali kartu yang sedang aktif
            foreach (GameObject card in selectedCards)
            {
                CardAnimation cardAnim = card.GetComponent<CardAnimation>();
                if (cardAnim != currentActiveCard)
                {
                    // Nonaktifkan komponen yang memungkinkan interaksi mouse
                    Collider collider = card.GetComponent<Collider>();
                    if (collider) collider.enabled = false;
                }
            }

            // Jalankan animasi kartu
            clickedCard.AnimatedToTarget();
            ShowButton(ActionPhase.ActiveButton);
            ShowButton(ActionPhase.KeepButton);
        }
        else
        {
            // Jika kartu yang sama diklik kembali, reset posisi
            if (clickedCard == currentActiveCard)
            {
                clickedCard.ResetPosition();
                currentActiveCard = null;

                // Aktifkan kembali semua kartu
                foreach (GameObject card in selectedCards)
                {
                    Collider collider = card.GetComponent<Collider>();
                    if (collider) collider.enabled = true;
                }
            }

            HideButton(ActionPhase.ActiveButton);
            HideButton(ActionPhase.KeepButton);
        }
    }

    private void OnActiveButtonClicked()
    {
        if (currentActiveCard != null)
        {
            // Panggil fungsi ActiveCard dari CardAnimation
            CardAnimation cardAnimation = currentActiveCard.GetComponent<CardAnimation>();
            if (cardAnimation != null)
            {
                cardAnimation.ActiveCard(); // Pastikan fungsi ini ada di CardAnimation
            }
        }
    }

    public void ShowButton(Button button)
    {
        button.gameObject.SetActive(true);
        button.transform.localScale = Vector3.zero; // Set scale awal ke 0
        button.transform.DOScale(Vector3.one, animationDuration * 1.5f) // Perpanjang durasi
            .SetEase(Ease.OutQuad); // Ganti easing function
    }

    public void HideButton(Button button)
    {
        button.transform.DOScale(Vector3.zero, animationDuration * 1.5f) // Perpanjang durasi
            .SetEase(Ease.InOutQuad) // Ganti easing function
            .OnComplete(() => {
                button.gameObject.SetActive(false); // Nonaktifkan button setelah animasi selesai
            });
    }

    private void ShadowBackground(Camera camera, GameObject background, float offset = 0.5f)
    {
        background.SetActive(true);
        var ofst = offsetFromCamera.z + offset;
        // Tentukan posisi latar belakang di depan kamera
        Vector3 backgroundPosition = camera.transform.position + camera.transform.forward * ofst;

        // Rotasi latar belakang mengikuti rotasi kamera
        Quaternion backgroundRotation = camera.transform.rotation;

        // Terapkan posisi dan rotasi ke latar belakang
        background.transform.position = backgroundPosition;
        background.transform.rotation = backgroundRotation;

        // Pastikan latar belakang aktif
        background.SetActive(true);
    }

}