using UnityEngine;

/// <summary>
/// Représente une cellule du labyrinthe, avec des murs et des types de matériaux.
/// </summary>
public class MazeCell : MonoBehaviour
{
    [Header("Références des murs")]
    [SerializeField] private GameObject _leftWall; // Mur gauche
    [SerializeField] private GameObject _rightWall; // Mur droit
    [SerializeField] private GameObject _frontWall; // Mur avant
    [SerializeField] private GameObject _backWall; // Mur arrière

    [Header("Bloc non visité")]
    [SerializeField] private GameObject _unvisitedBlock; // Bloc indiquant que la cellule n'a pas été visitée

    [Header("Matériaux")]
    [SerializeField] private Material _DefaultMaterial; // Matériau par défaut
    [SerializeField] private Material _StartMaterial; // Matériau pour la cellule de départ
    [SerializeField] private Material _EndMaterial; // Matériau pour la cellule de fin

    private Renderer _leftRenderer;
    private Renderer _rightRenderer;
    private Renderer _frontRenderer;
    private Renderer _backRenderer;

    // Types de murs possibles
    public enum WallType
    {
        BlackAndWhite = 0,
        RGB = 1,
        Start = 2,
        End = 3
    }

    // Type de mur actuel
    public WallType _currentWallType = WallType.BlackAndWhite;

    public bool IsVisited { get; set; } // Indique si la cellule a été visitée

    public enum Direction { Left, Right, Front, Back } // Directions des murs
    public enum CellType { Start, End, Default } // Types de cellules

    private void Awake()
    {
        // Initialise les renderers des murs
        _leftRenderer = GetWallRenderer(_leftWall);
        _rightRenderer = GetWallRenderer(_rightWall);
        _frontRenderer = GetWallRenderer(_frontWall);
        _backRenderer = GetWallRenderer(_backWall);

        // Définit le type de cellule par défaut
        SetCellType(CellType.Default);
    }

    /// <summary>
    /// Récupère le renderer d'un mur.
    /// </summary>
    private Renderer GetWallRenderer(GameObject wall)
    {
        if (wall == null) return null;

        Transform graphics = wall.transform.Find("Graphics");
        if (graphics != null)
        {
            return graphics.GetComponent<Renderer>();
        }
        else
        {
            Debug.LogError($"Graphics child not found on {wall.name}");
            return null;
        }
    }

    /// <summary>
    /// Marque la cellule comme visitée.
    /// </summary>
    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.SetActive(false);
    }

    /// <summary>
    /// Supprime un mur spécifique de la cellule.
    /// </summary>
    public void RemoveWall(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                if (_leftWall) _leftWall.SetActive(false);
                break;
            case Direction.Right:
                if (_rightWall) _rightWall.SetActive(false);
                break;
            case Direction.Front:
                if (_frontWall) _frontWall.SetActive(false);
                break;
            case Direction.Back:
                if (_backWall) _backWall.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// Définit le type de la cellule (départ, fin ou par défaut).
    /// </summary>
    public void SetCellType(CellType cellType)
    {
        ApplyMaterial(_leftRenderer, cellType);
        ApplyMaterial(_rightRenderer, cellType);
        ApplyMaterial(_frontRenderer, cellType);
        ApplyMaterial(_backRenderer, cellType);

        if (cellType == CellType.Start)
        {
            _currentWallType = WallType.Start;
        }
        else if (cellType == CellType.End)
        {
            _currentWallType = WallType.End;
        }
        else
        {
            _currentWallType = UserSessionManager.Instance.WallType;
        }
    }

    /// <summary>
    /// Définit le matériau par défaut pour la cellule.
    /// </summary>
    public void SetDefaultMaterial(Material material)
    {
        if (material == null) return;
        _DefaultMaterial = material;

        ApplyMaterial(_leftRenderer, CellType.Default);
        ApplyMaterial(_rightRenderer, CellType.Default);
        ApplyMaterial(_frontRenderer, CellType.Default);
        ApplyMaterial(_backRenderer, CellType.Default);
    }

    /// <summary>
    /// Applique un matériau à un mur en fonction du type de cellule.
    /// </summary>
    private void ApplyMaterial(Renderer renderer, CellType cellType)
    {
        if (renderer == null) return;

        switch (cellType)
        {
            case CellType.Start:
                renderer.material = _StartMaterial;
                break;
            case CellType.End:
                renderer.material = _EndMaterial;
                break;
            case CellType.Default:
                renderer.material = _DefaultMaterial;
                break;
        }
    }

    /// <summary>
    /// Vérifie si un mur spécifique est actif.
    /// </summary>
    public bool HasWall(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return _leftWall.activeSelf;
            case Direction.Right:
                return _rightWall.activeSelf;
            case Direction.Front:
                return _frontWall.activeSelf;
            case Direction.Back:
                return _backWall.activeSelf;
            default:
                return false;
        }
    }
}