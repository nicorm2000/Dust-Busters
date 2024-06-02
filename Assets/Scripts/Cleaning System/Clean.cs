using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Clean : MonoBehaviour, ICleanable
{
    [Header("Config")]
    [SerializeField] private CleaningManager cleaningManager;
    [SerializeField] private GameObject cleanObject;
    [SerializeField] private bool hasReplacement;

    private Renderer _renderer;
    private Material _cleanMaterial;
    public string CleanMessage => "Hold left click to clean";

    private bool _isCleaning = false;

    private float _cleaningInterval = 1.0f;
    private float _alphaPercentage = 1.0f;

    private Coroutine _coroutine = null;

    public event Action Cleaned;
    public event Action<GameObject> CleanedGO;
    public bool IsCleaned => isCleaned;
    private bool isCleaned = false;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _cleanMaterial = _renderer.material;
    }

    private void OnEnable()
    {
        cleaningManager.GetInputManager().CleanEvent += HandleCleanEvent;
    }

    private void OnDisable()
    {
        cleaningManager.GetInputManager().CleanEvent -= HandleCleanEvent;
        StopCleaning();
    }

    private void OnDestroy()
    {
        cleaningManager.GetInputManager().CleanEvent -= HandleCleanEvent;
        StopCleaning();
    }

    private void Start()
    {
        if (hasReplacement)
        {
            if (cleanObject != null)
            {
                cleanObject.GetComponent<Collider>().enabled = false;
            }
        }
        UpdateAlpha(_alphaPercentage);
    }

    private void Update()
    {
        if (_isCleaning)
        {
            CleanSurface();
        }
    }

    private void HandleCleanEvent(bool startCleaning)
    {
        if (startCleaning)
        {
            StartCleaning();
            cleaningManager.GetPlayerAnimator().SetBool("Cleaning", true);
        }
        else
        {
            StopCleaning();
            cleaningManager.GetPlayerAnimator().SetBool("Cleaning", false);
        }
    }

    private void StartCleaning()
    {
        _isCleaning = true;
    }

    private void StopCleaning()
    {
        _isCleaning = false;
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator CleaningCoroutine(int toolIndex)
    {
        Debug.Log("Cleaning.");

        while (_isCleaning)
        {
            if (!CanContinueCleaning())
            {
                StopCleaning();
                yield break;
            }

            UpdateAlphaAndDirtyPercentage(toolIndex);

            switch (_alphaPercentage)
            {
                case 1.00f:
                    _alphaPercentage = cleaningManager.CleaningPercentages[1];
                    break;
                case 0.66f:
                    _alphaPercentage = cleaningManager.CleaningPercentages[2];
                    break;
                case 0.33f:
                    _alphaPercentage = cleaningManager.CleaningPercentages[3];
                    break;
                case 0:
                    FinishCleaning();
                    yield break;
            }

            yield return new WaitForSeconds(_cleaningInterval);

            if (!CanContinueCleaning())
            {
                StopCleaning();
                yield break;
            }
        }
    }

    private bool CanContinueCleaning()
    {
        return cleaningManager.GetToolSelector().GetDirtyPercentage(cleaningManager.GetToolSelector().CurrentToolIndex) < cleaningManager.DirtyMaxValue;
    }

    private void UpdateAlphaAndDirtyPercentage(int toolIndex)
    {
        Debug.Log("Updating Alpha");
        UpdateAlpha(_alphaPercentage);
        cleaningManager.GetToolSelector().IncrementDirtyPercentage(toolIndex, cleaningManager.DirtyIncrementAmount);
    }

    private void FinishCleaning()
    {
        Debug.Log("Cleaned");
        isCleaned = true;
        Cleaned?.Invoke();
        CleanedGO?.Invoke(gameObject);
        StopCleaning();
        if (hasReplacement)
        {
            cleanObject.GetComponent<Collider>().enabled = true;
        }
        Destroy(gameObject);
    }

    private void CleanSurface()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = cleaningManager.GetCamera().ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, cleaningManager.GetInteractionDistance()))
        {
            if (hit.transform != gameObject.transform)
            {
                return;
            }

            int currentToolIndex = cleaningManager.GetToolSelector().CurrentToolIndex;

            if (cleaningManager.GetToolSelector().GetDirtyPercentage(currentToolIndex) < cleaningManager.DirtyMaxValue)
            {
                LayerMask mopLayer = cleaningManager.GetMopLayerMask();
                LayerMask spongeLayer = cleaningManager.GetSpongeLayerMask();

                if (currentToolIndex == 0 && (mopLayer.value & (1 << hit.transform.gameObject.layer)) != 0) // Mop
                {
                    _coroutine ??= StartCoroutine(CleaningCoroutine(currentToolIndex));
                }
                else if (currentToolIndex == 1 && (spongeLayer.value & (1 << hit.transform.gameObject.layer)) != 0) // Sponge
                {
                    _coroutine ??= StartCoroutine(CleaningCoroutine(currentToolIndex));
                }
                else
                {
                    Debug.Log("Tool is too dirty to clean or wrong tool for this surface!");
                }
            }
        }
    }

    private void UpdateAlpha(float alphaPercentage)
    {
        Color color = _cleanMaterial.color;
        color.a = alphaPercentage;
        _cleanMaterial.color = color;
    }

    public float GetAlphaPercentage()
    {
        return _alphaPercentage;
    }
}