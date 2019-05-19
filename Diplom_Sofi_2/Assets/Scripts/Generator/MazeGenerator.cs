using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Класс отвечающий за генерацию клеточного лабиринта
/// </summary>
public static class MazeGenerator
{
    private static int _height;
    private static int _width;

    private static bool OnDiggingTunnel = false;
    private static bool OnBackTrack = false;

    /// <summary>
    /// Установка точек входа и выхода в уже сгенерированом лабиринте
    /// </summary>
    public static void SetBeginEnd(Cell[,] map, out Vector2Int begin, out Vector2Int end)
    {
        begin = Vector2Int.zero;
        end = new Vector2Int(map.GetLength(1) - 1, map.GetLength(0) - 1);

        map[0, 0].walls[TypeDir.Below] = 0;
        map[end.y, end.x].walls[TypeDir.Top] = 0;
    }

    /// <summary>
    /// Генерация лабиринта по заданому seed
    /// </summary>
    public static Cell[,] Generate(int seed, int height, int width, int endUpPercent)
    {
        Random.InitState(seed);
        return Generate(height, width, endUpPercent);
    }

    /// <summary>
    /// Основной метод генерации лабиринта на основе его величины и процента тупиков в нём
    /// </summary>
    public static Cell[,] Generate(int height, int width, int endUpPercent)
    {
        _height = height;
        _width = width;

        Cell[,] map = GenerateWallWeightForCells(height, width);

        Vector2Int currentPoint = Vector2Int.zero;
        Stack<Vector2Int> track = new Stack<Vector2Int>();

        while (true) 
        {
            OnDiggingTunnel = DigMaze(ref map, ref currentPoint, ref track);

            if (OnDiggingTunnel) {
                OnBackTrack = false;
                continue;
            }
            else if(track.Count > 0) {
                if (!OnDiggingTunnel && !OnBackTrack) {
                    int randomPercent = Random.Range(0, 100);
                    if (randomPercent > endUpPercent) {
                        BreakOneWall(currentPoint, ref map);
                    }
                }
                // backtracking
                OnBackTrack = true;
                currentPoint = track.Pop();
            }
            else {
                break;
            }
        }

        return map;
    }

    #region Private Methods
    /// <summary>
    /// Первичная задание величины ребёр клеток (рёбра представляют будущие стены)
    /// </summary>
    private static Cell[,] GenerateWallWeightForCells(int height, int width)
    {
        Cell[,] map = new Cell[height, width];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {

                int top = y == height - 1 ? 100 : Random.Range(1, 100); 
                int left = x == 0 ? 100 : map[y, x - 1].GetWall(TypeDir.Right); 
                int below = y == 0 ? 100 : map[y - 1, x].GetWall(TypeDir.Top);
                int right = x == width - 1 ? 100 : Random.Range(1, 100);

                map[y, x] = new Cell(top, right, below, left);
            }
        }

        return map;
    }

    /// <summary>
    /// Алгоритм тунельного прокладывания лабиринта
    /// </summary>
    private static bool DigMaze(ref Cell[,] map, ref Vector2Int currentPoint, ref Stack<Vector2Int> track)
    {
        map[currentPoint.y, currentPoint.x].Lock = false;

        foreach (var wall in map[currentPoint.y, currentPoint.x].walls.OrderBy(x => x.Value)) {
            Vector2Int direction = Helper.ConvertDirToVector(wall.Key);
            Vector2Int nextPoint = currentPoint + direction;

            if (Helper.VerifyBorderByCell(_height, _width, nextPoint)) {
                if (map[nextPoint.y, nextPoint.x].Lock) {
                    // unlock
                    map[currentPoint.y, currentPoint.x].SetWall(wall.Key, 0);
                    track.Push(currentPoint);

                    TypeDir nextDir = Helper.ConvertVectorToDir(Helper.InvertVector2(direction));
                    map[nextPoint.y, nextPoint.x].SetWall(nextDir, 0);
                    currentPoint = nextPoint;

                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Метод уничтожения стены в заданой клетке
    /// </summary>
    private static void BreakOneWall(Vector2Int currentPoint, ref Cell[,] map)
    {
        foreach (var wall in map[currentPoint.y, currentPoint.x].walls.OrderBy(x => x.Value)) {
            Vector2Int direction = Helper.ConvertDirToVector(wall.Key);
            Vector2Int nextPoint = currentPoint + direction;

            if (Helper.VerifyBorderByCell(_height, _width, nextPoint)) {
                if (map[currentPoint.y, currentPoint.x].GetWall(wall.Key) != 0) {
                    // unlock
                    map[currentPoint.y, currentPoint.x].SetWall(wall.Key, 0);

                    TypeDir nextDir = Helper.ConvertVectorToDir(Helper.InvertVector2(direction));
                    map[nextPoint.y, nextPoint.x].SetWall(nextDir, 0);

                    break;
                }
            }
        }
    }
    #endregion
}
