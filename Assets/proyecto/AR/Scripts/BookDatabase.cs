using System.Collections.Generic;

[System.Serializable]
public class BookData
{
    public string bookId;
    public List<string> imageNames;
    public string titulo;
    public string autor;
    public List<string> topics;
    public string labelColor;

    public bool esAutoconclusivo;
    public string serie;
    public string tipoSerie;
    public bool serieTerminada;
    public int numeroEnSerie;
    public int totalLibrosSerie;
    public string libroAnteriorId;
    public string libroSiguienteId;
    public List<string> otrosLibros;

    public List<string> googleBooksISBN;
}

[System.Serializable]
public class BookDatabase
{
    public List<BookData> books;
}