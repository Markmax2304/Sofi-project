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

    public Color normalColor = new Color(1, 1, 1);
    public Color highlightedColor = new Color(.96f, .96f, .96f);
    public Color pressedColor = new Color(.75f, .75f, .75f);
    public Color selectedColor = new Color(.96f, .96f, .96f);

    public Color normalChooseColor = new Color(0, 1, 0);
    public Color highlightedChooseColor = new Color(0, .96f, 0);
    public Color pressedChooseColor = new Color(0, .75f, 0);
    public Color selectedChooseColor = new Color(0, .96f, 0);

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

    public void SetDirectionButton(TypeDir dir)
    {
        switch (dir) {
            case TypeDir.Below:
                SetColor(bottom);
                break;
            case TypeDir.Right:
                SetColor(right);
                break;
            case TypeDir.Left:
                SetColor(left);
                break;
            case TypeDir.Top:
                SetColor(top);
                break;
        }
    }

    private void Update()
    {
        InterpolateColor(bottom);
        InterpolateColor(right);
        InterpolateColor(left);
        InterpolateColor(top);
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

    #region Private Methods
    private void SetColor(Button button)
    {
        ColorBlock blockColor = button.colors;
        blockColor.normalColor = normalChooseColor;
        blockColor.pressedColor = pressedChooseColor;
        blockColor.highlightedColor = highlightedChooseColor;
        blockColor.selectedColor = selectedChooseColor;
        button.colors = blockColor;
    }

    private void InterpolateColor(Button button)
    {
        ColorBlock blockColor = button.colors;
        blockColor.normalColor = Color.Lerp(blockColor.normalColor, normalColor, Time.deltaTime);
        blockColor.pressedColor = Color.Lerp(blockColor.pressedColor, pressedColor, Time.deltaTime);
        blockColor.highlightedColor = Color.Lerp(blockColor.highlightedColor, highlightedColor, Time.deltaTime); 
        blockColor.selectedColor = Color.Lerp(blockColor.selectedColor, selectedColor, Time.deltaTime);
        button.colors = blockColor;
    }
    #endregion
}
