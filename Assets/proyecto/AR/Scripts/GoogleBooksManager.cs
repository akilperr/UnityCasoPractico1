using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleBooksManager : MonoBehaviour
{
    [Header("Tu API Key de Google Books")]
    public string apiKey = ApiKeys.GOOGLE_BOOKS;
    
    [Serializable]
    public class GoogleBookExtraData
    {
        public string author;
        public string publisher;
        public string publishedDate;
        public string category;
    }

    public IEnumerator GetBookExtraData(BookData book, Action<GoogleBookExtraData> onResult)
    {
        if (book == null || book.googleBooksISBN == null || book.googleBooksISBN.Count == 0)
        {
            onResult?.Invoke(null);
            yield break;
        }

        for (int i = 0; i < book.googleBooksISBN.Count; i++)
        {
            string isbn = book.googleBooksISBN[i];

            if (string.IsNullOrEmpty(isbn))
                continue;

            string url = "https://www.googleapis.com/books/v1/volumes?q=isbn:" + isbn + "&key=" + apiKey;

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning("Error Google Books con ISBN " + isbn + ": " + request.error);
                    continue;
                }

                string json = request.downloadHandler.text;
                GoogleBooksResponse response = JsonUtility.FromJson<GoogleBooksResponse>(json);

                if (response != null && response.items != null && response.items.Length > 0)
                {
                    GoogleVolumeInfo info = response.items[0].volumeInfo;

                    if (info != null)
                    {
                        GoogleBookExtraData data = new GoogleBookExtraData();

                        data.author = (info.authors != null && info.authors.Length > 0) ? info.authors[0] : null;
                        data.publisher = info.publisher;
                        data.publishedDate = info.publishedDate;
                        data.category = (info.categories != null && info.categories.Length > 0) ? info.categories[0] : null;

                        onResult?.Invoke(data);
                        yield break;
                    }
                }
            }
        }

        onResult?.Invoke(null);
    }
}