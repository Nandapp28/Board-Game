using System.Collections;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Referensi ke CanvasGroup
    public float fadeDuration = 1f; // Durasi fade

    private void Start()
    {
        // Memastikan canvasGroup tidak null
        if (canvasGroup != null)
        {
            StartCoroutine(FadeInImage());
            StartCoroutine(FadeOutImage());
        }
        else
        {
            Debug.LogError("CanvasGroup tidak diatur di Inspector!");
        }
    }

    private IEnumerator FadeInImage()
    {
        // Set alpha awal ke 0 (transparan)
        canvasGroup.alpha = 0;

        float time = 0;

        // Proses fade in
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, time / fadeDuration); // Ubah alpha dari 0 ke 1
            yield return null; // Tunggu frame berikutnya
        }

        canvasGroup.alpha = 1; // Pastikan alpha akhir adalah 1
    }

    public IEnumerator FadeOutImage()
    {
        yield return new WaitForSeconds(3f); // Tunggu 1 detik
        // Set alpha awal ke 1 (tidak transparan)
        canvasGroup.alpha = 1;

        float time = 0;

        // Proses fade out
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, time / fadeDuration); // Ubah alpha dari 1 ke 0
            yield return null; // Tunggu frame berikutnya
        }

        canvasGroup.alpha = 0; // Pastikan alpha akhir adalah 0
    }
}