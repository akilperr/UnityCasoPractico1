using System;

[Serializable]
public class GoogleBooksResponse
{
    public GoogleBookItem[] items;
}

[Serializable]
public class GoogleBookItem
{
    public GoogleVolumeInfo volumeInfo;
}

[Serializable]
public class GoogleVolumeInfo
{
    public string title;
    public string[] authors;
    public string publisher;
    public string publishedDate;
    public string[] categories;
}