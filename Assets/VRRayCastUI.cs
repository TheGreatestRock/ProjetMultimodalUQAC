using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRRaycastUI : MonoBehaviour
{
    public XRRayInteractor leftRayInteractor; // Reference to the left controller's XRRayInteractor
    public XRRayInteractor rightRayInteractor; // Reference to the right controller's XRRayInteractor
    public float rayLength = 10f; // Length of the ray
    public LayerMask interactableLayer; // Layer for UI elements (or interactables)

    private void Update()
    {
        // Handle raycasting and interaction for both controllers
        HandleRaycastInteraction(leftRayInteractor);
        HandleRaycastInteraction(rightRayInteractor);
    }

    private void HandleRaycastInteraction(XRRayInteractor rayInteractor)
    {
        // Check if the ray interactor is interacting with something (UI element)
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit) && hit.collider)
        {
            // Visualize the ray (optional)
            Debug.DrawLine(rayInteractor.transform.position, hit.point, Color.red);

            // Check if the ray hit a UI element
            if (hit.collider.CompareTag("UIElement"))
            {
                // Trigger UI interaction (clicking a button)
                var button = hit.collider.GetComponent<Button>();
                if (button)
                {
                    button.onClick.Invoke();
                }
            }
        }
    }
}