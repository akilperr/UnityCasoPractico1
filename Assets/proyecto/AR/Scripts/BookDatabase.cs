using System.Collections.Generic;

[System.Serializable]
public class BookData
{
    public string imageName;
    public string titulo;
    public List<string> topics;
    public string labelColor;
}

[System.Serializable]
public class BookDatabase
{
    public List<BookData> books;
}