using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static Vector2Int ConvertDirToVector(TypeDir dir)
    {
        Vector2Int vector = Vector2Int.zero;
        switch (dir) {
            case TypeDir.Top:
                vector = Vector2Int.up;
                break;
            case TypeDir.Right:
                vector = Vector2Int.right;
                break;
            case TypeDir.Below:
                vector = Vector2Int.down;
                break;
            case TypeDir.Left:
                vector = Vector2Int.left;
                break;
        }
        return vector;
    }

    public static TypeDir ConvertVectorToDir(Vector2Int vector)
    {
        if (vector == Vector2Int.up) {
            return TypeDir.Top;
        }
        else if (vector == Vector2Int.right) {
            return TypeDir.Right;
        }
        else if (vector == Vector2Int.down) {
            return TypeDir.Below;
        }
        else if (vector == Vector2Int.left) {
            return TypeDir.Left;
        }

        Debug.Log("Unreachable code is reached!!! o.o");
        return TypeDir.Below;
    }

    public static bool VerifyBorderByCell(int height, int width, Vector2Int point)
    {
        return point.y >= 0 && point.x >= 0 && point.y < height && point.x < width;
    }

    public static Vector2Int InvertVector2(Vector2Int vector)
    {
        int x = vector.x * (-1);
        int y = vector.y * (-1);
        return new Vector2Int(x, y);
    }
}
