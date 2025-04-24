using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;
    
    [SerializeField]
    private float _scaleFactorX = 2f; // Scaling factor for X axis
    [SerializeField]
    private float _scaleFactorY = 1f; // Scaling factor for Y axis
    [SerializeField]
    private float _scaleFactorZ = 2f; // Scaling factor for Z axis

    [SerializeField]
    private GameObject questionnairePrefab;
    
    [SerializeField]
    private GameObject TextBoxPrefab;
    
    private MazeCell[,] _mazeGrid;

    IEnumerator Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                // Position the cells properly based on the individual scale factors
                Vector3 position = new Vector3(x * _scaleFactorX, 0, z * _scaleFactorZ);

                // Instantiate maze cell and scale it for visual representation
                MazeCell mazeCell = Instantiate(_mazeCellPrefab, position, Quaternion.identity);
                mazeCell.transform.localScale = new Vector3(_scaleFactorX, _scaleFactorY, _scaleFactorZ);
                mazeCell._currentWallType = UserSessionManager.Instance.WallType;
                _mazeGrid[x, z] = mazeCell;
            }
        }

        yield return GenerateMaze(null, _mazeGrid[0, 0]);

        SetStartAndEnd();
    }

private void SetStartAndEnd()
{
    MazeCell startCell = _mazeGrid[0, 0];
    MazeCell endCell = _mazeGrid[_mazeWidth - 1, _mazeDepth - 1];

    startCell.SetCellType(MazeCell.CellType.Start);
    startCell.RemoveWall(MazeCell.Direction.Left);
    SpawnGameObjectAboveCell(startCell, questionnairePrefab);
    SpawnGameObjectAboveCell(startCell, TextBoxPrefab);

    endCell.SetCellType(MazeCell.CellType.End);
    endCell.RemoveWall(MazeCell.Direction.Right);
    SpawnGameObjectAboveCell(endCell, questionnairePrefab).GetComponent<QuestionnaireManager>().IsLastQuestionnaire = true;
    SpawnGameObjectAboveCell(endCell, TextBoxPrefab);

    // NEW: Find actual path and middle cell
    List<MazeCell> path = FindPath(startCell, endCell);
    if (path != null && path.Count > 2)
    {
        MazeCell middleCell = path[path.Count / 2];
        SpawnGameObjectAboveCell(middleCell, questionnairePrefab);
    }
}


    private GameObject SpawnGameObjectAboveCell(MazeCell cell, GameObject prefab)
    {
        GameObject obj = null;
        if (prefab == null || cell == null)
            return null;
        if (prefab == questionnairePrefab)
        {
            Vector3 spawnPosition = cell.transform.position + new Vector3(0, 1.5f, 0); // 1.5f units above the cell
            obj = Instantiate(questionnairePrefab, spawnPosition, Quaternion.identity);
        }
        else if (prefab == TextBoxPrefab)
        {
            TextBoxController textBoxController;
            if (cell._currentWallType == MazeCell.WallType.Start)
            {
                //log 
                Debug.Log("Start cell");
                Vector3 spawnPosition = cell.transform.position + new Vector3(-1.5f, 1.5f, 0); // 1.5f units above the cell
                obj = Instantiate(TextBoxPrefab, spawnPosition, Quaternion.identity);
                textBoxController = obj.GetComponent<TextBoxController>();
                textBoxController.topText.text = "Welcome to the Maze!, you will need to answer a questionnaire in the middle of the maze and at the end of the maze. \n\n you can choose the wall type you want before starting";
                textBoxController.button1Text.text = "Black and White";
                textBoxController.button3Text.text = "RGB";
                textBoxController.button2.gameObject.SetActive(false);
                textBoxController.action1.actionType = TextBoxController.ButtonActionType.ChangeMaterial;
                textBoxController.action1.parameter = MazeCell.WallType.BlackAndWhite.ToString();
                textBoxController.action3.actionType = TextBoxController.ButtonActionType.ChangeMaterial;
                textBoxController.action3.parameter = MazeCell.WallType.RGB.ToString();
            }
            else if (cell._currentWallType == MazeCell.WallType.End)
            {
                Debug.Log("End cell");
                Vector3 spawnPosition = cell.transform.position + new Vector3(1.5f, 1.5f, 0); // 1.5f units above the cell
                obj = Instantiate(TextBoxPrefab, spawnPosition, Quaternion.identity);
                textBoxController = obj.GetComponent<TextBoxController>();
                textBoxController.topText.text = "Congratulations! You have reached the end of the maze. \n\n you can choose to take a break, continue with the next maze or quit the game";
                textBoxController.button1Text.text = "Take a break";
                textBoxController.button2Text.text = "Continue";
                textBoxController.button3Text.text = "Quit";
                textBoxController.action1.actionType = TextBoxController.ButtonActionType.Pause;
                textBoxController.action1.parameter = "Pause";
                textBoxController.action2.actionType = TextBoxController.ButtonActionType.ChangeScene;
                textBoxController.action2.parameter =  UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                textBoxController.action3.actionType = TextBoxController.ButtonActionType.Quit;
                textBoxController.action3.parameter = "Quit";
            }
        }
        return obj;
    }


    private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        while (true)
        {
            MazeCell nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell == null)
                yield break;

            yield return GenerateMaze(currentCell, nextCell);
        }
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        List<MazeCell> unvisitedCells = GetUnvisitedCells(currentCell);

        if (unvisitedCells.Count == 0)
            return null;

        return unvisitedCells.OrderBy(_ => Random.Range(0, 100)).First();
    }

    private List<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        List<MazeCell> unvisitedCells = new List<MazeCell>();

        // Use grid indices (x, z) for logic, accounting for scaling
        int x = Mathf.FloorToInt(currentCell.transform.position.x / _scaleFactorX);
        int z = Mathf.FloorToInt(currentCell.transform.position.z / _scaleFactorZ);

        // Check the four possible neighboring cells
        if (x + 1 < _mazeWidth && !_mazeGrid[x + 1, z].IsVisited)
            unvisitedCells.Add(_mazeGrid[x + 1, z]);

        if (x - 1 >= 0 && !_mazeGrid[x - 1, z].IsVisited)
            unvisitedCells.Add(_mazeGrid[x - 1, z]);

        if (z + 1 < _mazeDepth && !_mazeGrid[x, z + 1].IsVisited)
            unvisitedCells.Add(_mazeGrid[x, z + 1]);

        if (z - 1 >= 0 && !_mazeGrid[x, z - 1].IsVisited)
            unvisitedCells.Add(_mazeGrid[x, z - 1]);

        return unvisitedCells;
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
            return;

        // Use grid indices (x, z) for logic to remove walls
        int prevX = Mathf.FloorToInt(previousCell.transform.position.x / _scaleFactorX);
        int prevZ = Mathf.FloorToInt(previousCell.transform.position.z / _scaleFactorZ);
        int currX = Mathf.FloorToInt(currentCell.transform.position.x / _scaleFactorX);
        int currZ = Mathf.FloorToInt(currentCell.transform.position.z / _scaleFactorZ);

        if (prevX < currX)
        {
            previousCell.RemoveWall(MazeCell.Direction.Right);
            currentCell.RemoveWall(MazeCell.Direction.Left);
            return;
        }

        if (prevX > currX)
        {
            previousCell.RemoveWall(MazeCell.Direction.Left);
            currentCell.RemoveWall(MazeCell.Direction.Right);
            return;
        }

        if (prevZ < currZ)
        {
            previousCell.RemoveWall(MazeCell.Direction.Front);
            currentCell.RemoveWall(MazeCell.Direction.Back);
            return;
        }

        if (prevZ > currZ)
        {
            previousCell.RemoveWall(MazeCell.Direction.Back);
            currentCell.RemoveWall(MazeCell.Direction.Front);
            return;
        }
    }
    
    private List<MazeCell> FindPath(MazeCell start, MazeCell end)
    {
        Dictionary<MazeCell, MazeCell> cameFrom = new Dictionary<MazeCell, MazeCell>();
        Queue<MazeCell> queue = new Queue<MazeCell>();
        HashSet<MazeCell> visited = new HashSet<MazeCell>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            MazeCell current = queue.Dequeue();
            if (current == end)
                break;

            foreach (MazeCell neighbor in GetConnectedNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Reconstruct the path
        List<MazeCell> path = new List<MazeCell>();
        MazeCell step = end;

        while (step != start)
        {
            path.Add(step);
            if (!cameFrom.ContainsKey(step)) return new List<MazeCell>(); // No path found
            step = cameFrom[step];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }
    
    private List<MazeCell> GetConnectedNeighbors(MazeCell cell)
    {
        List<MazeCell> neighbors = new List<MazeCell>();
        int x = Mathf.FloorToInt(cell.transform.position.x / _scaleFactorX);
        int z = Mathf.FloorToInt(cell.transform.position.z / _scaleFactorZ);

        if (!cell.HasWall(MazeCell.Direction.Right) && x + 1 < _mazeWidth)
            neighbors.Add(_mazeGrid[x + 1, z]);
        if (!cell.HasWall(MazeCell.Direction.Left) && x - 1 >= 0)
            neighbors.Add(_mazeGrid[x - 1, z]);
        if (!cell.HasWall(MazeCell.Direction.Front) && z + 1 < _mazeDepth)
            neighbors.Add(_mazeGrid[x, z + 1]);
        if (!cell.HasWall(MazeCell.Direction.Back) && z - 1 >= 0)
            neighbors.Add(_mazeGrid[x, z - 1]);

        return neighbors;
    }


}
