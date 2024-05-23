using UnityEngine;
using UnityEngine.UI;

public class PlayersUIManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CleaningToolSelector toolSelector;
    [SerializeField] private Image toolImage;
    [SerializeField] private Sprite[] toolSprites;
    [SerializeField] private GameObject cleaningList;
    [SerializeField] private GameObject displayControls;

    private bool _cleaningListState = false;
    private bool _displayControlsState = false;

    private void OnEnable()
    {
        inputManager.CleaningListEvent += CleaningListState;
        inputManager.DisplayControlsEvent += DisplayControlsState;
    }

    private void OnDisable()
    {
        inputManager.CleaningListEvent -= CleaningListState;
        inputManager.DisplayControlsEvent -= DisplayControlsState;
    }

    private void Start()
    {
        toolSelector.OnToolSwitched += UpdateToolImage;
        UpdateToolImage(toolSelector.CurrentToolIndex);
    }

    private void OnDestroy()
    {
        toolSelector.OnToolSwitched -= UpdateToolImage;
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
        if (currentToolIndex >= 0 && currentToolIndex < toolSprites.Length)
        {
            toolImage.sprite = toolSprites[currentToolIndex];
        }
        else
        {
            Debug.LogWarning("No corresponding sprite found for the current tool.");
        }
    }
}