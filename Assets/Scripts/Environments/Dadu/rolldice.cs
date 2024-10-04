using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    private Rigidbody rb;
    public float maxRandomForce = 10f;
    public float startRollForce = 10f;
    private float forceX, forceY, forceZ;

    // Tambahkan variabel untuk posisi awal
    public Vector3 initialPosition;

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
        ResetPosition(); // Setel ulang posisi sebelum melempar

        rb.isKinematic = false;

        // Menghasilkan gaya acak untuk lemparan
        forceX = Random.Range(0f, maxRandomForce);
        forceY = Random.Range(0f, maxRandomForce);
        forceZ = Random.Range(0f, maxRandomForce);

        // Terapkan gaya lemparan
        rb.AddForce(Vector3.up * startRollForce);
        rb.AddTorque(new Vector3(forceX, forceY, forceZ));
    }
}
