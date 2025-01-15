using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CardAnimation : MonoBehaviour {
    public Transform cameraTransform; // Transform kamera
    public Vector3 offsetFromCamera; // Offset dari kamera
    public Vector3 manualRotation; // Rotasi yang diinginkan
    public Vector3 targetScale; // Skala target
    public float animationDuration = 1.0f; // Durasi animasi

    public CardManager cardManager;

    private Vector3 initialPosition; // Posisi awal objek
    private Quaternion initialRotation; // Rotasi awal objek
    private Vector3 initialScale; // Skala awal objek

    public void Initial()
    {
        cardManager = FindObjectOfType<CardManager>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }

    private void OnMouseDown()
    {
        // Hanya izinkan klik jika CardManager tersedia
        if (cardManager != null)
        {
            buttonSoundEffect();
            cardManager.HandleCardClick(this);
        }
    }
    public void AnimatedToTarget() {
        // Hitung posisi target agar selalu di depan kamera
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

    public void ResetPosition()
    {
            transform.DOMove(initialPosition, animationDuration).SetEase(Ease.InOutQuad);
            transform.DORotateQuaternion(initialRotation, animationDuration).SetEase(Ease.InOutQuad);
            transform.DOScale(initialScale, animationDuration).SetEase(Ease.InOutQuad);
    }
    
    public void ActiveCard()
    {
        // Hitung rotasi yang diperlukan untuk kembali ke rotasi semula
        Quaternion targetRotation = Quaternion.Euler(manualRotation); // Rotasi yang diinginkan

        // Animasi berputar sebanyak 4 kali (1440 derajat) selama 0.5 detik
        transform.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Setelah berputar, kembalikan ke rotasi semula
                transform.rotation = targetRotation;

                // Mulai animasi maju mundur
                StartCoroutine(PulseAnimation());
            });
    }

    private IEnumerator PulseAnimation()
    {
        float elapsedTime = 0f;
        float pulseDuration = 0.5f; // Durasi maju mundur
        float pulseSpeed = 10f; // Kecepatan maju mundur

        Vector3 originalScale = transform.localScale; // Simpan skala awal

        while (elapsedTime < pulseDuration)
        {
            // Hitung skala baru untuk efek maju mundur
            float scale = originalScale.x + Mathf.Sin(Time.time * pulseSpeed) * 0.1f; // Ubah 0.1f sesuai kebutuhan
            transform.localScale = new Vector3(scale, scale, scale);

            elapsedTime += Time.deltaTime;
            yield return null; // Tunggu satu frame
        }

        // Kembalikan ke skala awal
        transform.localScale = originalScale;
    }

        private void buttonSoundEffect()
    {
        AudioManagers.instance.PlaySoundEffect(0);
    }

}