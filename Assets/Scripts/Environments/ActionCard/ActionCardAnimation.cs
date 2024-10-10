using DG.Tweening;
using UnityEngine;

public class ActionCardAnimation : MonoBehaviour
{

    public Transform cameraTransform; // Transform kamera
    public Vector3 offsetFromCamera = new Vector3(0, 0, 2); // Jarak offset kartu dari kamera (maju ke depan)
    public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f); // Skala target kartu
    public float animationDuration = 0.5f; // Durasi animasi
    public Vector3 manualRotation = new Vector3(0, 0, 0); // Rotasi manual yang dapat diatur
    public Vector3 activecardposition = Vector3.zero;
    public Vector3 activecardrotation = Vector3.zero;

    private Vector3 initialPosition;
    private Vector3 initialRotation;
    private Vector3 initialScale;

    private bool isAtTarget = false; // Menyimpan status apakah kartu sedang di posisi target
    private ActionCardDeck cardDeck;

    void Start()
    {
        // Jika cameraTransform tidak diset, gunakan kamera utama
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
         cardDeck = FindObjectOfType<ActionCardDeck>();
    }

    

    public void SetInitialPosition(Vector3 position)
    {
        initialPosition = position;
    }

    public void SetInitialScale(Vector3 scale)
    {
        initialScale = scale;
    }


    void Update()
    {
        // Update ini sekarang hanya dibutuhkan jika ada logika lain yang ingin kamu tambahkan
    }

    private void OnMouseDown()
    {
        if (isAtTarget)
        {
            // Animasi kembali ke posisi awal
            AnimateToInitial();
        }
        else
        {
            // Animasi ke posisi target di depan kamera
            AnimateToTarget();
        }

        if (cardDeck != null)
        {
            cardDeck.OnCardClick(this); // Kirim referensi kartu yang diklik ke deck
        }

        // Tukar status posisi kartu
        isAtTarget = !isAtTarget;
    }

    public void AnimateToTarget()
    {
        // Hitung posisi target agar selalu di depan kamera menggunakan transform kamera
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * offsetFromCamera.z  +
                                 cameraTransform.right * offsetFromCamera.x +
                                 cameraTransform.up * offsetFromCamera.y  ;

        // Gunakan rotasi manual yang telah diatur
        Quaternion targetRotation = Quaternion.Euler(manualRotation);

        // Animasi ke target menggunakan DoTween
        transform.DOMove(targetPosition, animationDuration).SetEase(Ease.InOutQuad);
        transform.DORotateQuaternion(targetRotation, animationDuration).SetEase(Ease.InOutQuad);
        transform.DOScale(targetScale, animationDuration).SetEase(Ease.InOutQuad);
    }

    public void AnimateToInitial()
    {
        // Animasi kembali ke posisi awal menggunakan DoTween
        transform.DOLocalMove(initialPosition, animationDuration).SetEase(Ease.InOutQuad);
        // transform.DOLocalRotate(initialRotation, animationDuration).SetEase(Ease.InOutQuad);
        transform.DOScale(initialScale, animationDuration).SetEase(Ease.InOutQuad);
    }

    public void ActiveCardAnimation()
    {
        // Hitung posisi target agar selalu di depan kamera menggunakan transform kamera
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * activecardposition.z  +
                                 cameraTransform.right * activecardposition.x +
                                 cameraTransform.up * activecardposition.y  ;

        // Gunakan rotasi manual yang telah diatur
        Quaternion targetRotation = Quaternion.Euler(activecardrotation);

        // Animasi ke target menggunakan DoTween
        transform.DOMove(targetPosition, animationDuration).SetEase(Ease.InOutQuad);
        transform.DORotateQuaternion(targetRotation, animationDuration).SetEase(Ease.InOutQuad);
    }
}