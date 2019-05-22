using System.Collections.Generic;
using UnityEngine;

public enum TypeDir { None = 0, Top = 1, Right = 2, Below = 4, Left = 8 };

/// <summary>
/// Класс представления отдельной клетки (комнаты) в лабиринте
/// </summary>
public class Cell
{
    /// <summary>
    /// список стен с весами, где ноль - это отсутствие стены, больше ноля - её наличие
    /// </summary>
    public Dictionary<TypeDir, int> walls;

    public bool Lock { get; set; }

    public Vector3 position { get; set; }

    public GameObject CloseTile { get; set; }

    /// <summary>
    /// Конструктор создания клетки по величинам всех стен
    /// </summary>
    public Cell(int _top, int _right, int _below, int _left)
    {
        walls = new Dictionary<TypeDir, int>() {
            { TypeDir.Top, _top },
            { TypeDir.Right, _right },
            { TypeDir.Below, _below },
            { TypeDir.Left, _left },
        };

        Lock = true;
    }

    /// <summary>
    /// Метод получения веса стены по её типу
    /// </summary>
    public int GetWall(TypeDir dir)
    {
        return walls[dir];
    }

    /// <summary>
    /// Метод присвоения веса стене по её типу
    /// </summary>
    public void SetWall(TypeDir dir, int value)
    {
        walls[dir] = value;
    }

    public void OpenTile()
    {
        CloseTile.gameObject.SetActive(false);
    }

    public static TypeDir ConvertFromVector(Vector2Int dir)
    {
        if(dir == Vector2Int.up) {
            return TypeDir.Top;
        }
        else if (dir == Vector2Int.down) {
            return TypeDir.Below;
        }
        else if (dir == Vector2Int.right) {
            return TypeDir.Right;
        }
        else if (dir == Vector2Int.left) {
            return TypeDir.Left;
        }
        else {
            return TypeDir.None;
        }
    }
}
