using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayConverter
{
    public static int[] ToOneArray(int[,] array, out int h, out int w)
    {
        h = array.GetLength(0);
        w = array.GetLength(1);
        int[] result = new int[h * w];

        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                result[y * w + x] = array[y, x];
            }
        }

        return result;
    }

    public static int[,] ToTwoArray(int[] array, int h, int w)
    {
        int[,] result = new int[h, w];

        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                result[y, x] = array[y * w + x];
            }
        }

        return result;
    }

    public static int[,] ToIntMap(Cell[,] map)
    {
        int h = map.GetLength(0);
        int w = map.GetLength(1);
        int[,] res = new int[h, w];

        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                foreach (var wall in map[y, x].walls) {
                    if (wall.Value != 0) {
                        res[y, x] |= (int)wall.Key;
                    }
                }
            }
        }

        return res;
    }
}
