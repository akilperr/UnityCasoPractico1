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
    public float topicRadius = 0.11f;
    public float topicHeight = 0.03f;
    public float centerSelectionMaxDistance = 250f; // en píxeles aprox.

    // raíz de tópicos por libro
    private Dictionary<string, GameObject> spawnedRoots = new Dictionary<string, GameObject>();

    // tracked image activa por nombre
    private Dictionary<string, ARTrackedImage> trackedImages = new Dictionary<string, ARTrackedImage>();

    // libro actualmente seleccionado
    private string currentCenteredImageName = null;

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
            trackedImages[trackedImage.referenceImage.name] = trackedImage;
            CreateTopics(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            trackedImages[trackedImage.referenceImage.name] = trackedImage;
        }

        foreach (var trackedImagePair in eventArgs.removed)
        {
            ARTrackedImage removedImage = trackedImagePair.Value;
            string imageName = removedImage.referenceImage.name;

            if (trackedImages.ContainsKey(imageName))
                trackedImages.Remove(imageName);

            RemoveTopics(removedImage);

            if (currentCenteredImageName == imageName)
                currentCenteredImageName = null;
        }
    }

    private void CreateTopics(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (spawnedRoots.ContainsKey(imageName))
            return;

        if (BookDataLoader.database == null || BookDataLoader.database.books == null)
            return;

        BookData book = BookDataLoader.database.books.Find(b => b.imageName == imageName);

        if (book == null || book.topics == null || book.topics.Count == 0)
            return;

        GameObject root = new GameObject("Topics_" + imageName);
        root.transform.SetParent(trackedImage.transform);
        root.transform.localPosition = Vector3.zero;
        root.transform.localRotation = Quaternion.identity;

        List<string> topics = book.topics;

        for (int i = 0; i < topics.Count; i++)
        {
            float angle = i * Mathf.PI * 2f / topics.Count;

            Vector3 localPos = new Vector3(
                Mathf.Cos(angle) * topicRadius,
                topicHeight,
                Mathf.Sin(angle) * topicRadius
            );

            GameObject topicObj = Instantiate(topicPrefab, root.transform);
            topicObj.transform.localPosition = localPos;
            topicObj.transform.localRotation = Quaternion.identity;

            Transform background = topicObj.transform.Find("Background");
            if (background != null)
            {
                Renderer renderer = background.GetComponent<Renderer>();
                if (renderer != null)
                {
                    if (ColorUtility.TryParseHtmlString(book.labelColor, out Color parsedColor))
                    {
                        renderer.material.color = parsedColor;
                    }
                }
            }

            TMP_Text tmp = topicObj.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
            {
                tmp.text = topics[i];
            }
        }

        root.SetActive(false);
        spawnedRoots[imageName] = root;
    }

    private void UpdateCenteredBook()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        string bestImageName = null;
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
                bestDistance = distanceToCenter;
                bestImageName = imageName;
            }
        }

        // si no hay ninguno suficientemente centrado, ocultamos todos
        if (bestImageName == null || bestDistance > centerSelectionMaxDistance)
        {
            currentCenteredImageName = null;
            SetOnlyOneRootVisible(null);
            return;
        }

        currentCenteredImageName = bestImageName;
        SetOnlyOneRootVisible(currentCenteredImageName);
    }

    private void SetOnlyOneRootVisible(string imageNameToShow)
    {
        foreach (var kvp in spawnedRoots)
        {
            string imageName = kvp.Key;
            GameObject root = kvp.Value;

            if (root != null)
            {
                root.SetActive(imageName == imageNameToShow);
            }
        }
    }

    private void RemoveTopics(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (spawnedRoots.TryGetValue(imageName, out GameObject root))
        {
            Destroy(root);
            spawnedRoots.Remove(imageName);
        }
    }
}