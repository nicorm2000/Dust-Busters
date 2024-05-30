using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CleanableObjectState
{
    public string objectName;
    public GameObject cleanObject;
    public GameObject dirtyObject;
    public float cleanliness;

    public CleanableObjectState(string objectName,GameObject cleanObject, GameObject dirtyObject, float cleanliness)
    {
        this.objectName = objectName;
        this.cleanObject = cleanObject;
        this.dirtyObject = dirtyObject;
        this.cleanliness = cleanliness;
    }
}

public class CleaningManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Camera gameCamera = null;
    [SerializeField] private Animator playerAnimator = null;
    [SerializeField] private Animator mopAnimator = null;
    [SerializeField] private Animator spongeAnimator = null;
    [SerializeField] private Animator handsAnimator = null;
    [SerializeField] private InputManager inputManager = null;
    [SerializeField] private CleaningTool cleaningTool = null;
    [SerializeField] private LayerMask mopLayerMask;
    [SerializeField] private LayerMask spongeLayerMask;
    [SerializeField] private float interactionDistance;

    public float[] CleaningPercentages { get; private set; }
    public int DirtyMaxValue { get; private set; }
    public int DirtyIncrementAmount { get; private set; }

    public List<CleanableObjectState> cleanableObjectStates = new ();

    private void Start()
    {
        CleaningPercentages = new float[4];

        CleaningPercentages[0] = 1.0f;
        CleaningPercentages[1] = 0.66f;
        CleaningPercentages[2] = 0.33f;
        CleaningPercentages[3] = 0.0f;

        DirtyMaxValue = 100;
        DirtyIncrementAmount = cleaningTool.DirtyIncrement;
    }

    private void OnDestroy()
    {
        CleaningPercentages = null;
    }

    public Camera GetCamera() => gameCamera;
    public Animator GetPlayerAnimator() => playerAnimator;
    public Animator GetMopAnimator() => mopAnimator;
    public Animator GetSpongeAnimator() => spongeAnimator;
    public Animator GetHandsAnimator() => handsAnimator;
    public InputManager GetInputManager() => inputManager;
    public CleaningTool GetToolSelector() => cleaningTool;
    public int GetDirtyMaxValue() => DirtyMaxValue;
    public int GetDirtyIncrementAmount() => cleaningTool.DirtyIncrement;
    public LayerMask GetMopLayerMask() => mopLayerMask;
    public LayerMask GetSpongeLayerMask() => spongeLayerMask;
    public float GetInteractionDistance() => interactionDistance;
}