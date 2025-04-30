using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Gère les panneaux d'information affichés dans le labyrinthe.
/// </summary>
public class TextBoxController : MonoBehaviour
{
    [Header("Champs de texte")]
    public TMP_Text topText; // Texte principal affiché en haut
    public TMP_Text button1Text; // Texte du bouton 1
    public TMP_Text button2Text; // Texte du bouton 2
    public TMP_Text button3Text; // Texte du bouton 3

    [Header("Références des boutons")]
    public Button button1; // Bouton 1
    public Button button2; // Bouton 2
    public Button button3; // Bouton 3

    [Header("Contenu des textes")]
    public string topTextContent = "temp"; // Contenu par défaut du texte principal
    public string button1TextContent = "temp"; // Contenu par défaut du bouton 1
    public string button2TextContent = "temp"; // Contenu par défaut du bouton 2
    public string button3TextContent = "temp"; // Contenu par défaut du bouton 3

    [SerializeField]
    private GameObject _canvas; // Référence au canvas contenant les éléments UI

    public enum ButtonActionType
    {
        None, // Aucune action
        ChangeMaterial, // Changer le matériau des murs
        ChangeScene, // Changer de scène
        Quit, // Quitter le jeu
        Pause // Mettre le jeu en pause
    }

    [System.Serializable]
    public class ButtonAction
    {
        public ButtonActionType actionType; // Type d'action du bouton
        public string parameter; // Paramètre associé à l'action (nom de scène, matériau, etc.)
    }

    [Header("Actions des boutons")]
    public ButtonAction action1; // Action associée au bouton 1
    public ButtonAction action2; // Action associée au bouton 2
    public ButtonAction action3; // Action associée au bouton 3

    void Start()
    {
        // Associe les actions aux boutons
        AssignButton(button1, action1);
        AssignButton(button2, action2);
        AssignButton(button3, action3);

        // Définit les textes par défaut si nécessaire
        if (topText.text == "temp") topText.text = topTextContent;
        if (button1Text.text == "temp") button1Text.text = button1TextContent;
        if (button2Text.text == "temp") button2Text.text = button2TextContent;
        if (button3Text.text == "temp") button3Text.text = button3TextContent;
    }

    void Update()
    {
        // Oriente le canvas vers la caméra principale
        if (_canvas)
        {
            _canvas.transform.LookAt(Camera.main.transform);
            _canvas.transform.Rotate(0, 180, 0);
        }
    }

    /// <summary>
    /// Associe une action à un bouton.
    /// </summary>
    void AssignButton(Button button, ButtonAction action)
    {
        button.onClick.RemoveAllListeners();
        switch (action.actionType)
        {
            case ButtonActionType.ChangeMaterial:
                button.onClick.AddListener(() => ChangeMaterial(action.parameter));
                break;
            case ButtonActionType.ChangeScene:
                button.onClick.AddListener(() =>
                {
                    if (Time.timeScale == 0) Time.timeScale = 1; // Reprend le temps si en pause
                    SceneManager.LoadScene(action.parameter);
                    UserSessionManager.Instance.StartSession();
                });
                break;
            case ButtonActionType.Quit:
                button.onClick.AddListener(() =>
                {
                    if (Time.timeScale == 0) Time.timeScale = 1; // Reprend le temps si en pause
                    Application.Quit();
                });
                break;
            case ButtonActionType.Pause:
                button.onClick.AddListener(PauseGame);
                break;
        }
    }

    /// <summary>
    /// Change le matériau des murs du labyrinthe.
    /// </summary>
    void ChangeMaterial(string newMatName)
    {
        if (Time.timeScale == 0) Time.timeScale = 1; // Reprend le temps si en pause

        if (newMatName == "BlackAndWhite") newMatName = "MazeMaterial";
        else if (newMatName == "RGB") newMatName = "MazeMaterial3";

        if (string.IsNullOrEmpty(newMatName)) return;

        Material newMat = Resources.Load<Material>(newMatName);
        if (newMat == null)
        {
            Debug.LogError("Échec du chargement du matériau : " + newMatName);
            return;
        }

        foreach (var obj in Object.FindObjectsByType(System.Type.GetType("MazeCell"), FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            var cell = (MazeCell)obj;
            if (cell != null)
            {
                if (cell._currentWallType == MazeCell.WallType.Start || cell._currentWallType == MazeCell.WallType.End)
                    continue; // Ignore les cellules de départ et de fin

                cell.SetCellType(MazeCell.CellType.Default); // Réinitialise avant d'appliquer le nouveau matériau
                cell.SetDefaultMaterial(newMat);
            }
        }
    }

    /// <summary>
    /// Met le jeu en pause ou le reprend.
    /// </summary>
    void PauseGame()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        Debug.Log("Pause activée/désactivée");
    }
}