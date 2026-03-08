using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class BookTopicSpawner : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;
    private Camera mainCamera;

    [Header("Prefab del tópico")]
    public GameObject topicPrefab;

    [Header("Ajustes visuales")]
    public float topicRadius = 0.055f;
    public float topicHeight = 0.02f;
    public float centerSelectionMaxDistance = 250f; // en píxeles aprox.

    // raíz de tópicos por libro
    private Dictionary<string, GameObject> spawnedRoots = new Dictionary<string, GameObject>();

    // imágenes detectadas activas por nombre de portada
    private Dictionary<string, ARTrackedImage> trackedImages = new Dictionary<string, ARTrackedImage>();

    // portada actualmente asociada a cada libro
    private Dictionary<string, ARTrackedImage> activeTrackedImagePerBook = new Dictionary<string, ARTrackedImage>();

    // libro actualmente seleccionado
    private string currentCenteredBookId = null;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        trackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    private void OnDisable()
    {
        trackedImageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }

    private void Update()
    {
        UpdateCenteredBook();
    }

    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            string imageName = trackedImage.referenceImage.name;
            trackedImages[imageName] = trackedImage;
            CreateOrReattachTopics(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            string imageName = trackedImage.referenceImage.name;
            trackedImages[imageName] = trackedImage;

            // Si vuelve a aparecer o se actualiza, permitimos reenganchar
            CreateOrReattachTopics(trackedImage);
        }

        foreach (var trackedImagePair in eventArgs.removed)
        {
            ARTrackedImage removedImage = trackedImagePair.Value;
            string imageName = removedImage.referenceImage.name;

            if (trackedImages.ContainsKey(imageName))
                trackedImages.Remove(imageName);

            HandleRemovedTrackedImage(removedImage);
        }
    }

    private void CreateOrReattachTopics(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        BookData book = GetBookByImageName(imageName);

        if (book == null || book.topics == null || book.topics.Count == 0)
            return;

        // Guardamos qué portada está asociada ahora mismo a este libro
        activeTrackedImagePerBook[book.bookId] = trackedImage;

        // Si ya existe el root del libro, lo reenganchamos a esta nueva portada
        if (spawnedRoots.TryGetValue(book.bookId, out GameObject existingRoot))
        {
            existingRoot.transform.SetParent(trackedImage.transform);
            existingRoot.transform.localPosition = Vector3.zero;
            existingRoot.transform.localRotation = Quaternion.identity;
            return;
        }

        // Si no existe, lo creamos
        GameObject root = new GameObject("Topics_" + book.bookId);
        root.transform.SetParent(trackedImage.transform);
        root.transform.localPosition = Vector3.zero;
        root.transform.localRotation = Quaternion.identity;

        List<string> topics = book.topics;

        // Radio dinámico según cantidad de tópicos
        float dynamicRadius = topicRadius;

        if (topics.Count <= 4)
            dynamicRadius = 0.055f;
        else if (topics.Count == 5)
            dynamicRadius = 0.06f;
        else
            dynamicRadius = 0.065f;

        for (int i = 0; i < topics.Count; i++)
        {
            float angle = i * Mathf.PI * 2f / topics.Count;

            Vector3 localPos = new Vector3(
                Mathf.Cos(angle) * dynamicRadius,
                topicHeight,
                Mathf.Sin(angle) * dynamicRadius
            );

            GameObject topicObj = Instantiate(topicPrefab, root.transform);
            topicObj.transform.localPosition = localPos;
            topicObj.transform.localRotation = Quaternion.identity;

            Transform background = topicObj.transform.Find("Background");
            if (background != null)
            {
                Renderer renderer = background.GetComponent<Renderer>();
                if (renderer != null && ColorUtility.TryParseHtmlString(book.labelColor, out Color parsedColor))
                {
                    renderer.material.color = parsedColor;
                }
            }

            TMP_Text tmp = topicObj.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
            {
                string cleanTopic = topics[i].Replace("_", " ");
                tmp.text = cleanTopic;

                Transform background2 = topicObj.transform.Find("Background");
                if (background2 != null)
                {
                    // Mantener la forma original de tu prefab
                    float baseWidth = 0.045f;          // ancho base real de tu cápsula
                    float extraPerCharacter = 0.002f; // crecimiento suave

                    float newWidth = baseWidth + (cleanTopic.Length * extraPerCharacter);

                    Vector3 scale = background2.localScale;
                    scale.x = 0.03f;
                    scale.y = newWidth;   
                    scale.z = 0.001f;

                    background2.localScale = scale;
                }
            }
        }

        root.SetActive(false);
        spawnedRoots[book.bookId] = root;
    }

    private void UpdateCenteredBook()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        string bestBookId = null;
        float bestDistance = float.MaxValue;

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        foreach (var kvp in trackedImages)
        {
            string imageName = kvp.Key;
            ARTrackedImage trackedImage = kvp.Value;

            if (trackedImage == null)
                continue;

            if (trackedImage.trackingState != TrackingState.Tracking)
                continue;

            Vector3 screenPos = mainCamera.WorldToScreenPoint(trackedImage.transform.position);

            // ignorar si está detrás de la cámara
            if (screenPos.z < 0)
                continue;

            float distanceToCenter = Vector2.Distance(
                new Vector2(screenPos.x, screenPos.y),
                screenCenter
            );

            if (distanceToCenter < bestDistance)
            {
                BookData book = GetBookByImageName(imageName);
                if (book != null)
                {
                    bestDistance = distanceToCenter;
                    bestBookId = book.bookId;
                }
            }
        }

        if (bestBookId == null || bestDistance > centerSelectionMaxDistance)
        {
            currentCenteredBookId = null;
            SetOnlyOneRootVisible(null);
            return;
        }

        currentCenteredBookId = bestBookId;
        SetOnlyOneRootVisible(currentCenteredBookId);
    }

    private void SetOnlyOneRootVisible(string bookIdToShow)
    {
        foreach (var kvp in spawnedRoots)
        {
            string bookId = kvp.Key;
            GameObject root = kvp.Value;

            if (root != null)
            {
                root.SetActive(bookId == bookIdToShow);
            }
        }
    }

    private void HandleRemovedTrackedImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        BookData book = GetBookByImageName(imageName);

        if (book == null)
            return;

        // Solo ocultamos el root si la portada que se ha perdido es la que estaba activa para ese libro
        if (activeTrackedImagePerBook.TryGetValue(book.bookId, out ARTrackedImage activeImage))
        {
            if (activeImage == trackedImage)
            {
                activeTrackedImagePerBook.Remove(book.bookId);

                if (spawnedRoots.TryGetValue(book.bookId, out GameObject root))
                {
                    root.SetActive(false);
                }
            }
        }

        if (currentCenteredBookId == book.bookId)
        {
            currentCenteredBookId = null;
        }
    }

    private BookData GetBookByImageName(string imageName)
    {
        if (BookDataLoader.database == null || BookDataLoader.database.books == null)
            return null;

        return BookDataLoader.database.books.Find(
            b => b.imageNames != null && b.imageNames.Contains(imageName)
        );
    }

    public BookData GetCurrentCenteredBook()
    {
        if (string.IsNullOrEmpty(currentCenteredBookId))
            return null;

        return GetBookById(currentCenteredBookId);
    }

    public BookData GetBookById(string bookId)
    {
        if (string.IsNullOrEmpty(bookId))
            return null;

        if (BookDataLoader.database == null || BookDataLoader.database.books == null)
            return null;

        return BookDataLoader.database.books.Find(b => b.bookId == bookId);
    }
}