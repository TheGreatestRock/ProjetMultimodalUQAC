using Unity.Properties;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField] private GameObject _leftWall;
    [SerializeField] private GameObject _rightWall;
    [SerializeField] private GameObject _frontWall;
    [SerializeField] private GameObject _backWall;

    [SerializeField] private GameObject _unvisitedBlock;

    [SerializeField] private Material _DefaultMaterial;
    [SerializeField] private Material _StartMaterial;
    [SerializeField] private Material _EndMaterial;

    private Renderer _leftRenderer;
    private Renderer _rightRenderer;
    private Renderer _frontRenderer;
    private Renderer _backRenderer;
    
    //enum of the different wall types
    public enum WallType
    {
        BlackAndWhite = 0,
        RGB = 1,
        Start = 2,
        End = 3
    }
    
    //current wall type
    public WallType _currentWallType = WallType.BlackAndWhite;

    public bool IsVisited { get; set; }

    public enum Direction { Left, Right, Front, Back }
    public enum CellType { Start, End, Default }

    private void Awake()
    {
        _leftRenderer = GetWallRenderer(_leftWall);
        _rightRenderer = GetWallRenderer(_rightWall);
        _frontRenderer = GetWallRenderer(_frontWall);
        _backRenderer = GetWallRenderer(_backWall);
        SetCellType(CellType.Default);
    }

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

    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.SetActive(false);
    }

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

    public void SetDefaultMaterial(Material material)
    {
        if (material == null) return;
        _DefaultMaterial = material;
        ApplyMaterial(_leftRenderer, CellType.Default);
        ApplyMaterial(_rightRenderer, CellType.Default);
        ApplyMaterial(_frontRenderer, CellType.Default);
        ApplyMaterial(_backRenderer, CellType.Default);
    }

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
