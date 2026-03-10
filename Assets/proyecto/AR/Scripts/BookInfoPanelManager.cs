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

        infoPanel.SetActive(true);

        titleText.gameObject.SetActive(true);
        titleText.text = currentBook.titulo ?? "Sin título";

        authorText.gameObject.SetActive(true);
        authorText.text = "<color=#B5B5B5>Autor:</color> cargando...";

        publisherText.gameObject.SetActive(true);
        publisherText.text = "<color=#B5B5B5>Editorial:</color> cargando...";

        dateText.gameObject.SetActive(true);
        dateText.text = "<color=#B5B5B5>Fecha:</color> cargando...";

        categoryText.gameObject.SetActive(true);
        categoryText.text = "<color=#B5B5B5>Categoría:</color> cargando...";

        if (currentBook.esAutoconclusivo)
        {
            sagaText.gameObject.SetActive(true);
            sagaText.text = "<color=#B5B5B5>Tipo:</color> Autoconclusivo";

            typeText.gameObject.SetActive(false);
            statusText.gameObject.SetActive(false);
            orderText.gameObject.SetActive(false);
            previousText.gameObject.SetActive(false);
            nextText.gameObject.SetActive(false);
            otherBooksText.gameObject.SetActive(false);
        }
        else
        {
            // Serie
            if (!string.IsNullOrEmpty(currentBook.serie))
            {
                sagaText.gameObject.SetActive(true);
                sagaText.text = "<color=#B5B5B5>Serie:</color> " + currentBook.serie;
            }
            else
            {
                sagaText.gameObject.SetActive(false);
            }

            // Tipo de serie
            if (!string.IsNullOrEmpty(currentBook.tipoSerie))
            {
                typeText.gameObject.SetActive(true);
                typeText.text = "<color=#B5B5B5>Tipo de Serie:</color> " + currentBook.tipoSerie;
            }
            else
            {
                typeText.gameObject.SetActive(false);
            }

            // Estado
            statusText.gameObject.SetActive(true);
            statusText.text = "<color=#B5B5B5>Estado de Publicación:</color> " + (currentBook.serieTerminada ? "Terminada" : "No terminada");

            // Orden de lectura
            orderText.gameObject.SetActive(true);
            orderText.text = "<color=#B5B5B5>Orden de lectura:</color> " + currentBook.numeroEnSerie + " de " + currentBook.totalLibrosSerie;

            // Libro anterior
            BookData previousBook = bookTopicSpawner.GetBookById(currentBook.libroAnteriorId);
            if (previousBook != null)
            {
                previousText.gameObject.SetActive(true);
                previousText.text = "<color=#B5B5B5>Libro anterior:</color> " + previousBook.titulo;
            }
            else
            {
                previousText.gameObject.SetActive(false);
            }

            // Libro siguiente
            BookData nextBook = bookTopicSpawner.GetBookById(currentBook.libroSiguienteId);
            if (nextBook != null)
            {
                nextText.gameObject.SetActive(true);
                nextText.text = "<color=#B5B5B5>Siguiente Libro:</color> " + nextBook.titulo;
            }
            else
            {
                nextText.gameObject.SetActive(false);
            }

            // Otros libros
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
                {
                    otherBooksText.gameObject.SetActive(true);
                    otherBooksText.text = "<color=#B5B5B5>Otros Libros:</color> " + string.Join(" ", otrosTitulos);
                }
                else
                {
                    otherBooksText.gameObject.SetActive(false);
                }
            }
            else
            {
                otherBooksText.gameObject.SetActive(false);
            }
        }

        if (googleBooksManager != null)
        {
            StartCoroutine(googleBooksManager.GetBookExtraData(currentBook, (data) =>
            {
                if (data != null)
                {
                    if (!string.IsNullOrEmpty(data.author))
                    {
                        authorText.gameObject.SetActive(true);
                        authorText.text = "<color=#B5B5B5>Autor:</color> " + data.author;                    }
                    else
                    {
                        authorText.gameObject.SetActive(false);
                    }

                    if (!string.IsNullOrEmpty(data.publisher))
                    {
                        publisherText.gameObject.SetActive(true);
                        publisherText.text = "<color=#B5B5B5>Editorial:</color> " + data.publisher;
                    }
                    else
                    {
                        publisherText.gameObject.SetActive(false);
                    }

                    if (!string.IsNullOrEmpty(data.publishedDate))
                    {
                        dateText.gameObject.SetActive(true);
                        dateText.text = "<color=#B5B5B5>Fecha de Publicación:</color> " + data.publishedDate;
                    }
                    else
                    {
                        dateText.gameObject.SetActive(false);
                    }

                    if (!string.IsNullOrEmpty(data.category))
                    {
                        categoryText.gameObject.SetActive(true);
                        categoryText.text = "<color=#B5B5B5>Categoría:</color> " + data.category;
                    }
                    else
                    {
                        categoryText.gameObject.SetActive(false);
                    }
                }
                else
                {
                    authorText.gameObject.SetActive(false);
                    publisherText.gameObject.SetActive(false);
                    dateText.gameObject.SetActive(false);
                    categoryText.gameObject.SetActive(false);
                }
            }));
        }
        else
        {
            authorText.gameObject.SetActive(false);
            publisherText.gameObject.SetActive(false);
            dateText.gameObject.SetActive(false);
            categoryText.gameObject.SetActive(false);
        }
    }

    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
    }
}