using UnityEngine;
using TMPro;

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

    [Header("Referencia al spawner")]
    public BookTopicSpawner bookTopicSpawner;

    public void OpenInfoPanel()
    {
        BookData currentBook = bookTopicSpawner.GetCurrentCenteredBook();

        if (currentBook == null)
            return;

        titleText.text = string.IsNullOrEmpty(currentBook.titulo) ? "Sin título" : currentBook.titulo;
        authorText.text = "Autor: " + (string.IsNullOrEmpty(currentBook.autor) ? "Desconocido" : currentBook.autor);

        if (currentBook.esAutoconclusivo)
        {
            sagaText.text = "Tipo: Autoconclusivo";
            typeText.text = "";
            statusText.text = "";
            orderText.text = "";
            previousText.text = "";
            nextText.text = "";
        }
        else
        {
            sagaText.text = "Saga: " + (string.IsNullOrEmpty(currentBook.saga) ? "-" : currentBook.saga);
            typeText.text = "Formato: " + (string.IsNullOrEmpty(currentBook.tipoSaga) ? "-" : currentBook.tipoSaga);

            string estado = currentBook.sagaTerminada ? "Terminada" : "No terminada";
            statusText.text = "Estado: " + estado;

            string numero = currentBook.numeroEnSaga > 0 ? currentBook.numeroEnSaga.ToString() : "-";
            string total = currentBook.totalLibrosSaga > 0 ? currentBook.totalLibrosSaga.ToString() : "?";
            orderText.text = "Libro en saga: " + numero + " de " + total;

            BookData previousBook = bookTopicSpawner.GetBookById(currentBook.libroAnteriorId);
            BookData nextBook = bookTopicSpawner.GetBookById(currentBook.libroSiguienteId);

            previousText.text = "Anterior: " + (previousBook != null ? previousBook.titulo : "-");
            nextText.text = "Siguiente: " + (nextBook != null ? nextBook.titulo : "-");
        }

        infoPanel.SetActive(true);
    }

    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
    }
}