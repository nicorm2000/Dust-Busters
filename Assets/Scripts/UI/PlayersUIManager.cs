using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayersUIManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CleaningManager cleaningManager;
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private GameObject cleaningList;
    [SerializeField] private GameObject displayControls;
    [SerializeField] private GameObject textElementPrefab;
    [SerializeField] private Transform cleanableListParent;
    [SerializeField] private Transform disposableListParent;
    [SerializeField] private GameObject objectBackground;

    [Header("Tools UI")]
    [SerializeField] private GameObject reticle;
    [SerializeField] private GameObject toolHolder;
    [SerializeField] private float toolHolderLifetime;
    [SerializeField] private Image mopImage;
    [SerializeField] private Image spongeImage;
    [SerializeField] private Image handImage;
    [SerializeField] private Sprite mopSpriteOn;
    [SerializeField] private Sprite mopSpriteOff;
    [SerializeField] private Sprite spongeSpriteOn;
    [SerializeField] private Sprite spongeSpriteOff;
    [SerializeField] private Sprite handSpriteOn;
    [SerializeField] private Sprite handSpriteOff;

    public GameObject jobFinished;
    public GameObject jobUnfinished;

    private bool _cleaningListState = false;
    private bool _displayControlsState = false;
    private List<GameObject> cleaningTextElements = new();
    private List<GameObject> disposalTextElements = new();

    private Coroutine toolDissapearCoroutine;

    private void OnEnable()
    {
        inputManager.CleaningListEvent += CleaningListState;
        inputManager.DisplayControlsEvent += DisplayControlsState;
        gameStateManager.GameLost += TriggerLostUI;
        gameStateManager.GameWon += TriggerWinUI;

        foreach (Clean cleanableObject  in gameStateManager.CleanableObjects)
        {
            cleanableObject.GetComponent<Clean>().CleanedGO += UpdateCleaningList;
        }

        foreach (DisposableObject disposableObject in gameStateManager.DisposableObjects)
        {
            disposableObject.GetComponent<DisposableObject>().DisposedGO += UpdateDisposableList;
            disposableObject.GetComponent<DisposableObject>().Broken += UpdateDisposableListMultiple;
        }
    }

    private void OnDisable()
    {
        inputManager.CleaningListEvent -= CleaningListState;
        inputManager.DisplayControlsEvent -= DisplayControlsState;
        gameStateManager.GameLost -= TriggerLostUI;
        gameStateManager.GameWon -= TriggerWinUI;

        foreach (Clean cleanableObject in gameStateManager.CleanableObjects)
        {
            if (cleanableObject != null)
                cleanableObject.GetComponent<Clean>().CleanedGO -= UpdateCleaningList;
        }

        foreach (DisposableObject disposableObject in gameStateManager.DisposableObjects)
        {
            if (disposableObject != null)
            {
                disposableObject.GetComponent<DisposableObject>().DisposedGO -= UpdateDisposableList;
                disposableObject.GetComponent<DisposableObject>().Broken -= UpdateDisposableListMultiple;
            }
        }
    }

    private void Start()
    {
        cleaningManager.GetToolSelector().OnToolSwitched += UpdateToolImage;
        UpdateToolImage(cleaningManager.GetToolSelector().CurrentToolIndex);

        reticle.SetActive(true);
        toolHolder.SetActive(false);
        jobFinished.SetActive(false);
        jobUnfinished.SetActive(false);

        CreateCleaningList();
    }

    private void OnDestroy()
    {
        cleaningManager.GetToolSelector().OnToolSwitched -= UpdateToolImage;
    }

    private void CleaningListState()
    {
        _cleaningListState = !_cleaningListState;
        cleaningList.SetActive(_cleaningListState);
    }

    private void DisplayControlsState()
    {
        _displayControlsState = !_displayControlsState;
        displayControls.SetActive(_displayControlsState);
    }

    private void UpdateToolImage(int currentToolIndex)
    {
        reticle.SetActive(false);
        toolHolder.SetActive(true);
        switch (cleaningManager.GetToolSelector().CurrentToolIndex)
        {
            case 0:
                mopImage.sprite = mopSpriteOn;
                spongeImage.sprite = spongeSpriteOff;
                handImage.sprite = handSpriteOff;
                break;
            case 1:
                mopImage.sprite = mopSpriteOff;
                spongeImage.sprite = spongeSpriteOn;
                handImage.sprite = handSpriteOff;
                break;
            case 2:
                mopImage.sprite = mopSpriteOff;
                spongeImage.sprite = spongeSpriteOff;
                handImage.sprite = handSpriteOn;
                break;
            default:
                mopImage.sprite = mopSpriteOff;
                spongeImage.sprite = spongeSpriteOff;
                handImage.sprite = handSpriteOff;
                break;
        }

        if (toolDissapearCoroutine != null)
        {
            StopCoroutine(toolDissapearCoroutine);
        }
        toolDissapearCoroutine = StartCoroutine(WaitToolDisappear());
    }

    private IEnumerator WaitToolDisappear()
    {
        yield return new WaitForSeconds(toolHolderLifetime);
        toolHolder.SetActive(false);
        reticle.SetActive(true);
    }

    private void TriggerLostUI()
    {
        jobUnfinished.SetActive(true);
    }
    private void TriggerWinUI()
    {
        jobFinished.SetActive(true);
    }

    public void CreateCleaningList()
    {
        foreach (var element in cleaningTextElements)
        {
            Destroy(element);
        }

        cleaningTextElements.Clear();

        foreach (var cleanableObject in gameStateManager.CleanableObjects)
        {
            GameObject textElement = Instantiate(textElementPrefab, cleanableListParent);
            if (textElement.TryGetComponent<TextMeshProUGUI>(out var tmp))
            {
                tmp.text = cleanableObject.name;

                textElement.name = cleanableObject.name;
            }
            cleaningTextElements.Add(textElement);
        }

        foreach (var element in disposalTextElements)
        {
            Destroy(element);
        }

        disposalTextElements.Clear();

        foreach (var disposableObject in gameStateManager.DisposableObjects)
        {
            GameObject textElement = Instantiate(textElementPrefab, disposableListParent);
            if (textElement.TryGetComponent<TextMeshProUGUI>(out var tmp))
            {
                tmp.text = disposableObject.name;

                textElement.name = disposableObject.name;
            }
            disposalTextElements.Add(textElement);
        }
    }

    private void UpdateCleaningList(GameObject objectInList)
    {
        for (int i = 0; i < cleaningTextElements.Count; i++)
        {
            if (cleaningTextElements[i].name == objectInList.name)
            {
                Destroy(cleaningTextElements[i]);
                cleaningTextElements.RemoveAt(i);
                break;
            }
        }
    }

    private void UpdateDisposableList(GameObject objectInList)
    {
        for (int i = 0; i < disposalTextElements.Count; i++)
        {
            if (disposalTextElements[i].name == objectInList.name)
            {
                Destroy(disposalTextElements[i]);
                disposalTextElements.RemoveAt(i);
                break;
            }
        }
    }

    private void UpdateDisposableListMultiple(GameObject objectInList, List<GameObject> objectsToAdd)
    {
        for (int i = 0; i < disposalTextElements.Count; i++)
        {
            if (disposalTextElements[i].name == objectInList.name)
            {
                Destroy(disposalTextElements[i]);
                disposalTextElements.RemoveAt(i);
                break;
            }
        }

        foreach (var disposableObject in objectsToAdd)
        {
            GameObject textElement = Instantiate(textElementPrefab, disposableListParent);
            if (textElement.TryGetComponent<TextMeshProUGUI>(out var tmp))
            {
                tmp.text = disposableObject.name;

                textElement.name = disposableObject.name;
            }

            disposalTextElements.Add(textElement);
            disposableObject.GetComponent<DisposableObject>().DisposedGO += UpdateDisposableList;
        }
    }
}