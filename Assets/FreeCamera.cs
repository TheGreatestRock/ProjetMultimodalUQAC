using UnityEngine;

/// <summary>
/// Permet à l'utilisateur de naviguer librement dans la scène en contrôlant la caméra.
/// </summary>
public class FreeCamera : MonoBehaviour
{
    public float moveSpeed = 10f; // Vitesse de déplacement de la caméra
    public float lookSpeed = 2f; // Sensibilité de la rotation de la caméra

    private float rotationX = 0f; // Rotation horizontale de la caméra
    private float rotationY = 0f; // Rotation verticale de la caméra
    private bool isInteractingWithUI = false; // Indique si l'utilisateur interagit avec l'interface utilisateur

    void Start()
    {
        // Verrouille le curseur au centre de l'écran et le rend invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Permet de basculer entre l'interaction avec l'UI et le contrôle de la caméra
        if (Input.GetKeyDown(KeyCode.E))
        {
            isInteractingWithUI = !isInteractingWithUI;
            Cursor.lockState = isInteractingWithUI ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isInteractingWithUI;
        }

        // Si l'utilisateur n'interagit pas avec l'UI, contrôle la caméra
        if (!isInteractingWithUI)
        {
            HandleMovement(); // Gère le déplacement de la caméra
            HandleRotation(); // Gère la rotation de la caméra
        }

        // Permet de libérer le curseur avec la touche Échap
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Gère le déplacement de la caméra en fonction des entrées utilisateur.
    /// </summary>
    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; // Déplacement latéral
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime; // Déplacement avant/arrière
        float moveY = 0f;

        // Déplacement vertical (espace pour monter, contrôle gauche pour descendre)
        if (Input.GetKey(KeyCode.Space)) moveY = moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftControl)) moveY = -moveSpeed * Time.deltaTime;

        // Applique le déplacement à la caméra
        transform.Translate(new Vector3(moveX, moveY, moveZ));
    }

    /// <summary>
    /// Gère la rotation de la caméra en fonction des mouvements de la souris.
    /// </summary>
    private void HandleRotation()
    {
        rotationX += Input.GetAxis("Mouse X") * lookSpeed; // Rotation horizontale
        rotationY -= Input.GetAxis("Mouse Y") * lookSpeed; // Rotation verticale
        rotationY = Mathf.Clamp(rotationY, -90f, 90f); // Limite la rotation verticale pour éviter un retournement complet

        // Applique la rotation à la caméra
        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0);
    }
}