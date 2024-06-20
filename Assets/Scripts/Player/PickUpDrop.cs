using UnityEngine;

public class PickUpDrop : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private CleaningManager cleaningManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickupLayerMask;
    [SerializeField] private float maxThrowingForce = 20f;
    [SerializeField] private float forceChargeRate = 5f;
    //[SerializeField] private Material outlineMaterial;

    public InputManager inputManager;

    [Header("Audio Config")]
    [SerializeField] private AudioManager audioManager = null;
    [SerializeField] private string grabDropEvent = null;
    [SerializeField] private string throwEvent = null;

    private ObjectGrabbable ObjectGrabbable;
    private float currentThrowingForce;
    private bool isChargingThrow;

    //private Material originalMaterial;
    //private Renderer objectRenderer;

    private void OnEnable()
    {
        inputManager.PickUpEvent += PickUpAndDropObject;
        inputManager.ThrowStartEvent += StartChargingThrow;
        inputManager.ThrowEndEvent += ThrowObject;
        cleaningManager.GetToolSelector().OnToolSwitched += HandleToolSwitched;
    }

    private void OnDisable()
    {
        inputManager.PickUpEvent -= PickUpAndDropObject;
        inputManager.ThrowStartEvent -= StartChargingThrow;
        inputManager.ThrowEndEvent -= ThrowObject;
        cleaningManager.GetToolSelector().OnToolSwitched -= HandleToolSwitched;
    }

    private void Update()
    {
        if (isChargingThrow)
        {
            currentThrowingForce += forceChargeRate * Time.deltaTime;
            currentThrowingForce = Mathf.Min(currentThrowingForce, maxThrowingForce);
        }

        ///Outline shader test
        ///if (Physics.Raycast(mainCamera.position, mainCamera.forward, out RaycastHit raycastHit, cleaningManager.GetInteractionDistance()))
        ///{
        ///    if (raycastHit.transform.TryGetComponent(out objectGrabbable))
        ///    {
        ///        if (raycastHit.transform.TryGetComponent(out objectRenderer))
        ///        {
        ///            if (originalMaterial == null)
        ///            {
        ///                originalMaterial = objectRenderer.material;
        ///            }
        ///            objectRenderer.material = outlineMaterial;
        ///        }
        ///    }
        ///    else
        ///    {
        ///        if (objectRenderer != null && originalMaterial != null)
        ///        {
        ///            objectRenderer.material = originalMaterial;
        ///            originalMaterial = null;
        ///        }
        ///    }
        ///}
        ///else
        ///{
        ///    if (objectRenderer != null && originalMaterial != null)
        ///    {
        ///        objectRenderer.material = originalMaterial;
        ///        originalMaterial = null;
        ///    }
        ///}
    }

    private void PickUpAndDropObject()
    {
        if (ObjectGrabbable == null && cleaningManager.GetToolSelector().CurrentToolIndex == 2)
        {
            if (Physics.Raycast(mainCamera.position, mainCamera.forward, out RaycastHit raycastHit, cleaningManager.GetInteractionDistance()))
            {
                if (raycastHit.transform.TryGetComponent(out ObjectGrabbable))
                {
                    audioManager.PlaySound(grabDropEvent);
                    ObjectGrabbable.Grab(objectGrabPointTransform);
                    playerController.SetObjectGrabbable(ObjectGrabbable);
                }
            }
        }
        else if (ObjectGrabbable != null)
        {
            audioManager.PlaySound(grabDropEvent);
            DropObject();
        }
    }

    private void DropObject()
    {
        if (ObjectGrabbable != null)
        {
            ObjectGrabbable.Drop();
            ObjectGrabbable = null;
        }

        playerController.ClearObjectGrabbable();
    }

    private void HandleToolSwitched(int newToolIndex)
    {
        if (newToolIndex != 2 && ObjectGrabbable != null)
        {
            DropObject();
        }
    }

    private void StartChargingThrow()
    {
        if (ObjectGrabbable != null)
        {
            isChargingThrow = true;
            currentThrowingForce = 0f;
        }
    }

    public float GetCurrentThrowingForce()
    {
        return currentThrowingForce;
    }

    public float GetMaxThrowingForce()
    {
        return maxThrowingForce;
    }

    public ObjectGrabbable GetObjectGrabbable()
    {
        return ObjectGrabbable;
    }

    private void ThrowObject()
    {
        if (ObjectGrabbable != null)
        {
            audioManager.PlaySound(throwEvent);
            Vector3 throwDirection = mainCamera.forward;
            ObjectGrabbable.Throw(currentThrowingForce, throwDirection);

            ObjectGrabbable = null;
            isChargingThrow = false;
        }

        playerController.ClearObjectGrabbable();
    }
}