using UnityEngine;

public class BookDataLoader : MonoBehaviour
{
    public static BookDatabase database;

    void Awake()
    {
        LoadJSON();
    }

    void LoadJSON()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("books");

        database = JsonUtility.FromJson<BookDatabase>(jsonFile.text);

        Debug.Log("Libros cargados: " + database.books.Count);
    }
}