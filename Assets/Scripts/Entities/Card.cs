using UnityEngine;
using DG.Tweening;

public class CardAnimation : MonoBehaviour
{
    public Transform cameraTransform; // Transform kamera
    public Vector3 offsetFromCamera = new Vector3(0, 0, 2); // Jarak offset kartu dari kamera (maju ke depan)
    public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f); // Skala target kartu
    public float animationDuration = 0.5f; // Durasi animasi
    public Vector3 manualRotation = new Vector3(0, 0, 0); // Rotasi manual yang dapat diatur

    private Vector3 initialPosition;
    private Vector3 initialRotation;
    private Vector3 initialScale;

    private bool isAtTarget = false; // Menyimpan status apakah kartu sedang di posisi target

    void Start()
    {
        // Simpan posisi, rotasi, dan skala awal
        initialPosition = transform.localPosition;
        initialRotation = transform.localEulerAngles;
        initialScale = transform.localScale;

        // Jika cameraTransform tidak diset, gunakan kamera utama
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
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

        // Tukar status posisi kartu
        isAtTarget = !isAtTarget;
    }

    void AnimateToTarget()
    {
        // Hitung posisi target agar selalu di depan kamera menggunakan transform kamera
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * offsetFromCamera.z +
                                 cameraTransform.right * offsetFromCamera.x +
                                 cameraTransform.up * offsetFromCamera.y;

        // Gunakan rotasi manual yang telah diatur
        Quaternion targetRotation = Quaternion.Euler(manualRotation);

        // Animasi ke target menggunakan DoTween
        transform.DOMove(targetPosition, animationDuration).SetEase(Ease.InOutQuad);
        transform.DORotateQuaternion(targetRotation, animationDuration).SetEase(Ease.InOutQuad);
        transform.DOScale(targetScale, animationDuration).SetEase(Ease.InOutQuad);
    }

    void AnimateToInitial()
    {
        // Animasi kembali ke posisi awal menggunakan DoTween
        transform.DOLocalMove(initialPosition, animationDuration).SetEase(Ease.InOutQuad);
        transform.DOLocalRotate(initialRotation, animationDuration).SetEase(Ease.InOutQuad);
        transform.DOScale(initialScale, animationDuration).SetEase(Ease.InOutQuad);
    }
}
