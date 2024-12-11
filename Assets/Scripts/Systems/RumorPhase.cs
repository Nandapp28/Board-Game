using UnityEngine;
using System.Collections;

[System.Serializable]
public class TransformCamera {
    public Vector3 Position;
    public Vector3 Rotation;
}

[System.Serializable]
public class RumorCardAnimator {    
    public Transform cameraTransform; // Transform kamera
    public Vector3 offsetFromCamera; // Offset dari kamera
    public Vector3 manualRotation; // Rotasi manual untuk kartu
    public Vector3 targetScale = Vector3.one; // Skala akhir kartu
    public float animationDuration = 1f; // Durasi animasi
}

public class RumorPhase : MonoBehaviour {

    [Header("Camera Position And Rotation Settings")]
    [SerializeField] private TransformCamera Infrastuktur = new TransformCamera();
    [SerializeField] private TransformCamera Mining = new TransformCamera();
    [SerializeField] private TransformCamera Consumen = new TransformCamera();
    [SerializeField] private TransformCamera Finance = new TransformCamera();

    private Camera MainCamera;
    private Vector3 initialPosition; // Menyimpan posisi awal
    private Quaternion initialRotation; // Menyimpan rotasi awal
    private TransformCamera[] sectors;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f; // Kecepatan pergerakan kamera
    [SerializeField] private float waitTime = 10f; // Waktu tunggu antar sektor
    private int CurrentSectorIndex;

    #region Unity Lifecycle

    /// Memulai fase rumor dan menginisialisasi kamera.
    public void StartRumorPhase() {
        MainCamera = Camera.main; // Mengambil kamera utama
        initialPosition = MainCamera.transform.position; // Simpan posisi awal
        initialRotation = MainCamera.transform.rotation; // Simpan rotasi awal

        sectors = new TransformCamera[] { Infrastuktur, Finance, Mining, Consumen };
        CurrentSectorIndex = 0; // Inisialisasi indeks sektor saat ini

        StartCoroutine(MoveCameraThroughSectors());
    }

    #endregion

    #region Camera Movement Coroutines
    /// Menggerakkan kamera melalui semua sektor.
    private IEnumerator MoveCameraThroughSectors() {
        while (CurrentSectorIndex < sectors.Length) {
            yield return StartCoroutine(MoveCameraToSector(sectors[CurrentSectorIndex]));
            yield return new WaitForSeconds(waitTime); // Tunggu sebelum pindah ke sektor berikutnya
            CurrentSectorIndex++;
        }

        // Setelah semua sektor, kembali ke posisi awal
        yield return StartCoroutine(MoveCameraToInitialPosition());
    }
    /// Memindahkan kamera ke sektor tertentu.
    /// <param name="sector">Sektor yang dituju.</param>
    private IEnumerator MoveCameraToSector(TransformCamera sector) {
        Vector3 startPosition = MainCamera.transform.position;
        Quaternion startRotation = MainCamera.transform.rotation;
        Vector3 targetPosition = sector.Position;
        Quaternion targetRotation = Quaternion.Euler(sector.Rotation);

        float elapsedTime = 0f;

        // Interpolasi posisi dan rotasi kamera
        while (elapsedTime < 1f) {
            elapsedTime += Time.deltaTime * moveSpeed;
            MainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            MainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);
            yield return null; // Tunggu hingga frame berikutnya
        }

        // Pastikan posisi dan rotasi akhir tepat
        MainCamera.transform.position = targetPosition;
        MainCamera.transform.rotation = targetRotation;
    }
    /// Mengembalikan kamera ke posisi awal.
    private IEnumerator MoveCameraToInitialPosition() {
        Vector3 startPosition = MainCamera.transform.position;
        Quaternion startRotation = MainCamera.transform.rotation;
        Vector3 targetPosition = initialPosition;
        Quaternion targetRotation = initialRotation;

        float elapsedTime = 0f;

        // Interpolasi posisi dan rotasi kamera kembali ke posisi awal
        while (elapsedTime < 1f) {
            elapsedTime += Time.deltaTime * moveSpeed;
            MainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            MainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);
            yield return null; // Tunggu hingga frame berikutnya
        }

        // Pastikan posisi dan rotasi akhir tepat
        MainCamera.transform.position = targetPosition;
        MainCamera.transform.rotation = targetRotation;
    }

    #endregion

    #region Rumor Card Animation
    
    

    #endregion
}