using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс представляющий логику ввода
/// </summary>
public class InputsController : MonoBehaviour
{
    public Button top;
    public Button right;
    public Button bottom;
    public Button left;

    /// <summary>
    /// Показывает наличие ввода движения игрока вверх
    /// </summary>
    public bool Up { get; private set; }
    /// <summary>
    /// Показывает наличие ввода движения игрока направо
    /// </summary>
    public bool Right { get; private set; }
    /// <summary>
    /// Показывает наличие ввода движения игрока вниз
    /// </summary>
    public bool Down { get; private set; }
    /// <summary>
    /// Показывает наличие ввода движения игрока влево
    /// </summary>
    public bool Left { get; private set; }

    /// <summary>
    /// Метод для привязки к кнопке для нажатия
    /// </summary>
    public void PressSide(int dir)
    {
        switch ((TypeDir)dir) {
            case TypeDir.Below:
                Down = true;
                break;
            case TypeDir.Right:
                Right = true;
                break;
            case TypeDir.Left:
                Left = true;
                break;
            case TypeDir.Top:
                Up = true;
                break;
        }
    }

    /// <summary>
    /// Метод отката изменений после нажатия
    /// </summary>
    public void LateUpdate()
    {
        Up = false;
        Right = false;
        Down = false;
        Left = false;
    }
}
