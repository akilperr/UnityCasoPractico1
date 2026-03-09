using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BookInfoPanelManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject infoPanel;

    [Header("Texts")]
    public TMP_Text titleText;
    public TMP_Text authorText;
    public TMP_Text sagaText;
    public TMP_Text typeText;
    public TMP_Text statusText;
    public TMP_Text orderText;
    public TMP_Text previousText;
    public TMP_Text nextText;
    public TMP_Text otherBooksText;
    public TMP_Text publisherText;
    public TMP_Text dateText;
    public TMP_Text categoryText;

    [Header("Referencias")]
    public BookTopicSpawner bookTopicSpawner;
    public GoogleBooksManager googleBooksManager;

    public void OpenInfoPanel()
    {
        BookData currentBook = bookTopicSpawner.GetCurrentCenteredBook();

        if (currentBook == null)
            return;

        titleText.text = currentBook.titulo ?? "Sin título";

        // Estos campos vendrán de la API
        authorText.text = "Autor: cargando...";
        publisherText.text = "Editorial: cargando...";
        dateText.text = "Fecha: cargando...";
        categoryText.text = "Categoría: cargando...";

        if (currentBook.esAutoconclusivo)
        {
            sagaText.text = "Tipo: Autoconclusivo";
            typeText.text = "";
            statusText.text = "";
            orderText.text = "";
            previousText.text = "";
            nextText.text = "";
            otherBooksText.text = "";
        }
        else
        {
            sagaText.text = "Saga: " + (string.IsNullOrEmpty(currentBook.saga) ? "-" : currentBook.saga);
            typeText.text = "Formato: " + (string.IsNullOrEmpty(currentBook.tipoSaga) ? "-" : currentBook.tipoSaga);
            statusText.text = "Estado: " + (currentBook.sagaTerminada ? "Terminada" : "No terminada");
            orderText.text = "Libro en saga: " + currentBook.numeroEnSaga + " de " + currentBook.totalLibrosSaga;

            BookData previousBook = bookTopicSpawner.GetBookById(currentBook.libroAnteriorId);
            BookData nextBook = bookTopicSpawner.GetBookById(currentBook.libroSiguienteId);

            previousText.text = "Anterior: " + (previousBook != null ? previousBook.titulo : "-");
            nextText.text = "Siguiente: " + (nextBook != null ? nextBook.titulo : "-");

            if (currentBook.otrosLibros != null && currentBook.otrosLibros.Count > 0)
            {
                List<string> otrosTitulos = new List<string>();

                foreach (string otherBookId in currentBook.otrosLibros)
                {
                    BookData otherBook = bookTopicSpawner.GetBookById(otherBookId);

                    if (otherBook != null)
                    {
                        otrosTitulos.Add(otherBook.titulo);
                    }
                }

                if (otrosTitulos.Count > 0)
                    otherBooksText.text = "Otros libros:\n" + string.Join("\n", otrosTitulos);
                else
                    otherBooksText.text = "Otros libros: -";
            }
            else
            {
                otherBooksText.text = "Otros libros: -";
            }
        }

        if (googleBooksManager != null)
        {
            StartCoroutine(googleBooksManager.GetBookExtraData(currentBook, (data) =>
            {
                if (data != null)
                {
                    authorText.text = "Autor: " + (!string.IsNullOrEmpty(data.author) ? data.author : "No disponible");
                    publisherText.text = "Editorial: " + (!string.IsNullOrEmpty(data.publisher) ? data.publisher : "No disponible");
                    dateText.text = "Fecha: " + (!string.IsNullOrEmpty(data.publishedDate) ? data.publishedDate : "No disponible");
                    categoryText.text = "Categoría: " + (!string.IsNullOrEmpty(data.category) ? data.category : "No disponible");
                }
                else
                {
                    authorText.text = "Autor: No disponible";
                    publisherText.text = "Editorial: No disponible";
                    dateText.text = "Fecha: No disponible";
                    categoryText.text = "Categoría: No disponible";
                }
            }));
        }
        else
        {
            authorText.text = "Autor: No disponible";
            publisherText.text = "Editorial: No disponible";
            dateText.text = "Fecha: No disponible";
            categoryText.text = "Categoría: No disponible";
        }

        infoPanel.SetActive(true);
    }

    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
    }
}