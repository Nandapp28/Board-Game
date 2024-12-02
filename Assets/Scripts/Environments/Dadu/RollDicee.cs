using DG.Tweening;
using TMPro;
using UnityEngine;

public class RollDice : MonoBehaviour
{
    [Header("Pengaturan Dadu")]
    public Rigidbody diceRigidbody1; // Rigidbody untuk dadu pertama
    public Rigidbody diceRigidbody2; // Rigidbody untuk dadu kedua

    [Header("Pengaturan UI")]
    public TextMeshProUGUI resultText; // UI Text untuk menampilkan hasil

    [Header("Pengaturan Animasi")]
    public Transform cameraTransform; // Referensi kamera
    public Vector3 offsetFromCamera1 = new Vector3(0, 0, 2);
    public Vector3 offsetFromCamera2 = new Vector3(0, 0, 2);
    public Vector3 offsetRotationFromCamera1 = new Vector3(0, 0, 2);
    public Vector3 offsetRotationFromCamera2 = new Vector3(0, 0, 2);
    public float moveSpeed = 5f; // Kecepatan gerak dadu
    public float resetPosition = 2f; // Jarak offset dari kamera
    
    [HideInInspector] public bool isRolling = false; // Status apakah dadu sedang bergulir
    [HideInInspector] public bool isMovingToCamera = false; // Status apakah dadu sedang bergerak ke kamera
    [HideInInspector] public bool isKinematic = false; // Status kinematic untuk kontrol manual

    [HideInInspector] public float timeSinceRolling = 0f; // Waktu yang telah berlalu sejak dadu mulai bergulir

    [HideInInspector] public Vector3 initialPosition1;
    [HideInInspector] public Vector3 initialPosition2;
    [HideInInspector] public Vector3 initialRotation1;
    [HideInInspector] public Vector3 initialRotation2;
    [HideInInspector] public Quaternion currentRotation1;
    [HideInInspector] public Quaternion currentRotation2;
    [HideInInspector] public bool hasRotationsCaptured = false;

    [HideInInspector] public int Dice1 = 0;
    [HideInInspector] public int Dice2 = 0;

    public void Start()
    {
        // Kosongkan teks di awal
        resultText.text = "";

        // Simpan posisi awal dari kedua dadu
        initialPosition1 = diceRigidbody1.transform.position;
        initialPosition2 = diceRigidbody2.transform.position;
        initialRotation1 = diceRigidbody1.transform.rotation.eulerAngles;
        initialRotation2 = diceRigidbody2.transform.rotation.eulerAngles;
    }

    /// Memulai lemparan untuk kedua dadu.
    public void RollTheDice()
    {
        diceRigidbody1.isKinematic = false;
        diceRigidbody2.isKinematic = false;
        isKinematic = false;
        isRolling = true;

        // Reset kecepatan dan rotasi untuk dadu pertama
        ResetDicePhysics(diceRigidbody1);

        // Reset kecepatan dan rotasi untuk dadu kedua
        ResetDicePhysics(diceRigidbody2);

        // Terapkan gaya acak untuk dadu pertama
        ApplyRandomForceAndTorque(diceRigidbody1);

        // Terapkan gaya acak untuk dadu kedua
        ApplyRandomForceAndTorque(diceRigidbody2);

        // Perbarui UI untuk menunjukkan proses rolling
        resultText.text = "...";
    }

    /// Mengatur ulang kecepatan dan rotasi Rigidbody.
    public void ResetDicePhysics(Rigidbody rigidbody)
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    /// Terapkan gaya dan rotasi acak pada Rigidbody.
    public void ApplyRandomForceAndTorque(Rigidbody rigidbody)
    {
        rigidbody.AddForce(new Vector3(
            Random.Range(-5f, 5f), // Gaya horizontal
            Random.Range(5f, 10f), // Gaya vertikal
            Random.Range(-5f, 5f)  // Gaya kedalaman
        ), ForceMode.Impulse);

        rigidbody.AddTorque(new Vector3(
            Random.Range(-10f, 10f), // Rotasi sumbu X
            Random.Range(-10f, 10f), // Rotasi sumbu Y
            Random.Range(-10f, 10f)  // Rotasi sumbu Z
        ), ForceMode.Impulse);
    }

    /// Menghitung nilai dari kedua dadu dan menampilkan hasilnya.
    public void CalculateDiceValues()
    {
        int dice1Value = GetSingleDiceValue(diceRigidbody1.gameObject);
        int dice2Value = GetSingleDiceValue(diceRigidbody2.gameObject);

        if (dice1Value > 0 && dice2Value > 0)
        {
            Dice1 = dice1Value;
            Dice2 = dice2Value;
            // Hitung total nilai
            int total = dice1Value + dice2Value;

            // Tampilkan hasil di UI
            resultText.text = $"{total}";
            Debug.Log($"Dadu 1: {dice1Value}, Dadu 2: {dice2Value}, Total: {total}");
        }
        else
        {
            // Tampilkan pesan jika salah satu nilai tidak valid
            resultText.text = "Lemparan Tidak Valid";
            Debug.LogWarning("Nilai salah satu dadu tidak valid.");
        }
    }

    public void ResetDiceResult()
    {
        Dice1 = 0;
        Dice2 = 0;
    }

    /// Mendapatkan nilai dadu berdasarkan deteksi raycast.
    public int GetSingleDiceValue(GameObject dice)
    {
        Ray ray = new Ray(dice.transform.position, Vector3.up);
        if (Physics.Raycast(ray, out RaycastHit hit, 1f))
        {
            string faceName = hit.collider.gameObject.name;
            if (int.TryParse(faceName, out int diceValue))
            {
                return diceValue;
            }
        }
        return -1; // Nilai invalid jika tidak ada deteksi
    }

    /// Menggerakkan dadu ke depan kamera dengan sisi atas menghadap kamera.
/// Menggerakkan dadu ke depan kamera dengan sisi atas menghadap kamera.
public void MoveDiceToCamera(GameObject dice, Vector3 offsetFromCamera, Vector3 rotationOffset, Quaternion currentRotation)
{
    // Hitung posisi target di depan kamera
    Vector3 targetPosition = Camera.main.transform.position + 
                             Camera.main.transform.forward * offsetFromCamera.z +
                             Camera.main.transform.right * offsetFromCamera.x +
                             Camera.main.transform.up * offsetFromCamera.y;

    // Tentukan rotasi target berdasarkan nilai dadu
    int diceValue = GetSingleDiceValue(dice);
    Quaternion targetRotation;

    // if (diceValue >= 2 && diceValue <= 5)
    // {
    //     targetRotation = Quaternion.Euler(0, 0, currentRotation.eulerAngles.z + rotationOffset.z);
    // }
    // else // untuk 1 dan 6
    // {
    //     targetRotation = Quaternion.Euler(currentRotation.eulerAngles.x - rotationOffset.x, 90, 90);
    // }

    if (diceValue == 3 || diceValue == 5)
    {
        targetRotation = Quaternion.Euler(currentRotation.eulerAngles.x - rotationOffset.x, 90, 90);

    }
    else // untuk 1 dan 6
    {
        targetRotation = Quaternion.Euler(currentRotation.eulerAngles.x - rotationOffset.x, 0, currentRotation.eulerAngles.z);
    }


    // Hitung durasi animasi
    float distance = Vector3.Distance(dice.transform.position, targetPosition);
    float duration = distance / moveSpeed;

    // Animasi menggunakan DOTween
    dice.transform.DOMove(targetPosition, duration).SetEase(Ease.Linear);
    dice.transform.DORotateQuaternion(targetRotation, duration).SetEase(Ease.Linear);
}

    public bool CheckIfDiceReachedCamera(GameObject Dice, Vector3 offset)
    {
        // Hitung posisi target
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 targetPosition = cameraPosition + cameraForward * offset.z +
                                Camera.main.transform.right * offset.x +
                                Camera.main.transform.up * offset.y;

        return Vector3.Distance(Dice.transform.position, targetPosition) < 0.1f;
    }

    public void ResetDicePosition(GameObject Dice, Vector3 position, Vector3 rotation, float duration)
    {
        Dice.transform.DOMove(position, duration).SetEase(Ease.InOutQuad);
        Dice.transform.DORotate(rotation, duration).SetEase(Ease.InOutQuad);
        // Reset flag saat melakukan reset posisi atau status lainnya
        hasRotationsCaptured = false;
    }
}
