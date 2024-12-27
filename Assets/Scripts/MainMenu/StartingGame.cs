using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class StartingGame : MonoBehaviour
{
    public Image loadingBar; // Loading bar dengan fill type "Filled"
    public TextMeshProUGUI progressText; // Teks untuk menampilkan persentase loading

    private float targetProgress = 0f; // Target progres yang akan dicapai
    private float fillSpeed = 0.2f; // Kecepatan pengisian loading bar yang lebih lambat

    void Start()
    {
        StartCoroutine(UpdateLoadingBar()); // Mulai proses update loading bar
    }

    private IEnumerator UpdateLoadingBar()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1); // Ganti "1" dengan index scene yang ingin dimuat
        operation.allowSceneActivation = false; // Jangan aktifkan scene segera

        while (!operation.isDone)
        {
            // Hitung progres berdasarkan operasi loading scene
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // Scene loading progress (0 hingga 0.9)

            // Tentukan target progres berdasarkan waktu pemuatan
            targetProgress = progress;

            // Update loading bar secara bertahap
            float currentProgress = loadingBar.fillAmount;
            if (currentProgress < targetProgress)
            {
                loadingBar.fillAmount = Mathf.MoveTowards(currentProgress, targetProgress, fillSpeed * Time.deltaTime);
            }

            // Update teks loading (opsional)
            if (progressText != null)
                progressText.text = Mathf.RoundToInt(loadingBar.fillAmount * 100) + "%";

            // Jika sudah hampir selesai (progress >= 0.9), lanjutkan dengan delay kecil
            if (operation.progress >= 0.9f)
            {
                // Setelah selesai, aktifkan scene
                yield return new WaitForSeconds(1f); // Delay tambahan sebelum scene diaktifkan
                operation.allowSceneActivation = true; // Aktifkan scene
            }

            yield return null; // Tunggu frame berikutnya
        }
    }
}
