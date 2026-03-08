using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class PhotoCaptureManager : MonoBehaviour
{
    [Header("UI opcional que quieres ocultar durante la foto")]
    public GameObject canvasUI;

    [Header("Texto opcional de feedback")]
    public TMP_Text feedbackText;

    public void CapturePhoto()
    {
        StartCoroutine(CaptureAndSave());
    }

    private IEnumerator CaptureAndSave()
    {
        if (canvasUI != null)
            canvasUI.SetActive(false);

        yield return new WaitForEndOfFrame();

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        string fileName = "ARBook_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";

        NativeGallery.SaveImageToGallery(screenImage, "ARBooks", fileName, (success, path) =>
        {
            Debug.Log("Guardado en galería: " + success + " | Ruta: " + path);

            if (feedbackText != null)
            {
                feedbackText.gameObject.SetActive(true);
                feedbackText.text = success ? "Foto guardada en galería" : "No se pudo guardar la foto";
                StartCoroutine(HideFeedbackAfterDelay());
            }
        });

        Destroy(screenImage);

        if (canvasUI != null)
            canvasUI.SetActive(true);
    }

    private IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }
}