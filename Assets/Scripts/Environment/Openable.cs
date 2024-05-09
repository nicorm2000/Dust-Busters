using UnityEngine;
using UnityEngine.InputSystem;

public class Openable : MonoBehaviour
{
    [Header("Openable Config")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float raycastDistance = 3f;
    [SerializeField] private LayerMask openableRaycastLayerMask = ~0;

    private Animator _openableAnimator;
    public bool _isOpen { get; private set; }

    private readonly string _openableOpen = "Open";

    private void OnEnable()
    {
        inputManager.InteractEvent += ToggleObjectState;
    }

    private void OnDisable()
    {
        inputManager.InteractEvent -= ToggleObjectState;
    }

    private void Start()
    {
        _openableAnimator = GetComponent<Animator>();
        _isOpen = false;
    }

    private void ToggleObjectState()
    {
        if (IsMouseLookingAtObject())
        {
            _isOpen = !_isOpen;
            _openableAnimator.SetBool(_openableOpen, _isOpen);
            Debug.Log((_isOpen ? "Open" : "Close") + " Object: " + name);
        }
    }

    private bool IsMouseLookingAtObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance, openableRaycastLayerMask))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 2f);
            return hit.transform.parent == transform;
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red, 2f);
        }
        return false;
    }
}