using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Gère les interactions entre les rayons des contrôleurs VR et les éléments d'interface utilisateur (UI).
/// </summary>
public class VRRaycastUI : MonoBehaviour
{
    public XRRayInteractor leftRayInteractor; // Référence au rayon du contrôleur gauche
    public XRRayInteractor rightRayInteractor; // Référence au rayon du contrôleur droit
    public float rayLength = 10f; // Longueur du rayon
    public LayerMask interactableLayer; // Couches des éléments interactifs (UI)

    private void Update()
    {
        // Gère les interactions pour les deux contrôleurs
        HandleRaycastInteraction(leftRayInteractor);
        HandleRaycastInteraction(rightRayInteractor);
    }

    /// <summary>
    /// Gère l'interaction d'un rayon avec les éléments interactifs.
    /// </summary>
    /// <param name="rayInteractor">Le rayon interagissant avec les éléments.</param>
    private void HandleRaycastInteraction(XRRayInteractor rayInteractor)
    {
        // Vérifie si le rayon interagit avec un élément
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit) && hit.collider)
        {
            // Visualise le rayon (optionnel, utile pour le débogage)
            Debug.DrawLine(rayInteractor.transform.position, hit.point, Color.red);

            // Vérifie si l'élément touché est un élément UI
            if (hit.collider.CompareTag("UIElement"))
            {
                // Si c'est un bouton, déclenche son événement "onClick"
                var button = hit.collider.GetComponent<Button>();
                if (button)
                {
                    button.onClick.Invoke();
                }
            }
        }
    }
}