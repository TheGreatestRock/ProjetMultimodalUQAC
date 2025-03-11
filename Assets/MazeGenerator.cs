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

                _mazeGrid[x, z] = mazeCell;
            }
        }

        yield return GenerateMaze(null, _mazeGrid[0, 0]);

        SetStartAndEnd();
    }

    private void SetStartAndEnd()
    {
        MazeCell startCell = _mazeGrid[0, 0];
        startCell.SetCellType(MazeCell.CellType.Start);
        startCell.RemoveWall(MazeCell.Direction.Left);
        
        MazeCell endCell = _mazeGrid[_mazeWidth - 1, _mazeDepth - 1];
        endCell.SetCellType(MazeCell.CellType.End);
        endCell.RemoveWall(MazeCell.Direction.Right);
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
}
