using UnityEngine;
using System.Collections;

[System.Serializable]
public class TransformCamera {
    public Vector3 Position;
    public Vector3 Rotation;
}

public class RumorPhase : MonoBehaviour {
    [Header("Camera Position And Rotation Settings")]
    [SerializeField] private TransformCamera Infrastuktur = new TransformCamera();
    [SerializeField] private TransformCamera Mining = new TransformCamera();
    [SerializeField] private TransformCamera Consumen = new TransformCamera();
    [SerializeField] private TransformCamera Finance = new TransformCamera();

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f; // Kecepatan pergerakan kamera
    [SerializeField] private float waitTime = 10f; // Waktu tunggu antar sektor

    private Camera MainCamera;
    private Vector3 initialPosition; // Menyimpan posisi awal
    private Quaternion initialRotation; // Menyimpan rotasi awal

    public void StartRumorPhase() {
        MainCamera = Camera.main; // Mengambil kamera utama
        initialPosition = MainCamera.transform.position; // Simpan posisi awal
        initialRotation = MainCamera.transform.rotation; // Simpan rotasi awal
        StartCoroutine(MoveCameraThroughSectors());
    }

    private IEnumerator MoveCameraThroughSectors() {
        // Daftar sektor yang akan dilalui
        TransformCamera[] sectors = new TransformCamera[] { Infrastuktur, Mining, Consumen, Finance };

        foreach (var sector in sectors) {
            // Mengatur posisi dan rotasi kamera dengan interpolasi
            yield return StartCoroutine(MoveCameraToSector(sector));
            // Tunggu selama waktu yang ditentukan
            yield return new WaitForSeconds(waitTime);
        }

        // Kembali ke posisi dan rotasi awal
        yield return StartCoroutine(MoveCameraToInitialPosition());
    }

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
}