using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class PhotoCaptureManager : MonoBehaviour
{
    [Header("UI que NO quieres que salga en la foto")]
    public GameObject CaptureButton;
    public GameObject InfoButton;

    public void CapturePhoto()
    {
        StartCoroutine(CaptureAndSave());
    }

    private IEnumerator CaptureAndSave()
    {
        // Ocultar botones
        if (CaptureButton != null)
            CaptureButton.SetActive(false);
        if (InfoButton != null)
            InfoButton.SetActive(false);

        // Esperar a que Unity refresque la UI
        yield return null;
        yield return new WaitForEndOfFrame();

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        string fileName = "ARBook_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        NativeGallery.SaveImageToGallery(screenImage, "ARBooks", fileName, (success, path) =>
        {
            Debug.Log("Guardado en galería: " + success + " | Ruta: " + path);
        });

        Destroy(screenImage);

        // Volver a mostrar botones
        if (CaptureButton != null)
            CaptureButton.SetActive(true);
        if (InfoButton != null)
            InfoButton.SetActive(true);
    }
}