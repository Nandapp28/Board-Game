using DG.Tweening;
using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    [Header("Dice Settings")]
    public float maxTorque = 10f;
    public float minInitialForce = 5f;
    public float maxInitialForce = 10f;
    public Vector3 initialPosition;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // Set posisi awal dadu
        initialPosition = transform.position; // Mengambil posisi awal dari editor
    }

    // Fungsi untuk mengembalikan dadu ke posisi awal sebelum melempar
    public void ResetPosition()
    {
        rb.isKinematic = true; // Membuat dadu tidak dipengaruhi fisika
        transform.position = initialPosition; // Setel posisi dadu ke posisi awal
        transform.rotation = Quaternion.identity; // Reset rotasi dadu
    }

    // Fungsi untuk melempar dadu
    public void RollDice()
    {
        ResetPosition();

        rb.isKinematic = false;

        // Gaya awal acak dengan arah acak
        Vector3 initialForce = Random.insideUnitSphere * Random.Range(minInitialForce, maxInitialForce);
        rb.AddForce(initialForce);

        // Gaya torsi acak dengan variasi yang lebih besar
        Vector3 torque = Random.insideUnitSphere * maxTorque;
        rb.AddTorque(torque);
    }

    public void DiceToCamera(Transform cameraTransform, Vector3 offset, Vector3 rotation, float duration, Ease ease)
    {       
        rb.isKinematic = true;
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * offset.z  +
                                 cameraTransform.right * offset.x +
                                 cameraTransform.up * offset.y;

        // Gunakan rotasi manual yang telah diatur
        Quaternion targetRotation = Quaternion.Euler(rotation);

        // Animasi ke target menggunakan DoTween
        transform.DOMove(targetPosition, duration).SetEase(Ease.InOutQuad);
        transform.DORotateQuaternion(targetRotation, duration).SetEase(Ease.InOutQuad);
    }



}
