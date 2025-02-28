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

    private MazeCell[,] _mazeGrid;

    IEnumerator Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                MazeCell mazeCell = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);
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

        //yield return new WaitForSeconds(0.05f);

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

        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

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

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.RemoveWall(MazeCell.Direction.Right);
            currentCell.RemoveWall(MazeCell.Direction.Left);
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.RemoveWall(MazeCell.Direction.Left);
            currentCell.RemoveWall(MazeCell.Direction.Right);
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.RemoveWall(MazeCell.Direction.Front);
            currentCell.RemoveWall(MazeCell.Direction.Back);
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.RemoveWall(MazeCell.Direction.Back);
            currentCell.RemoveWall(MazeCell.Direction.Front);
            return;
        }
    }
}
