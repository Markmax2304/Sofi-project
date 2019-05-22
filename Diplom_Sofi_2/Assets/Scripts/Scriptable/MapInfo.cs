using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapInfo")]
public class MapInfo : ScriptableObject
{
    [Space, Header("Customize settings of map")]
    public int height = 20;
    public int width = 15;
    [Range(0, 99)]
    public int endUpPercent = 50;
    [Space]
    public GameObject closeTile;
    [Space]
    public GameObject line;

    [Space, Header("Dont touch! Dir name must equals name sprite!")]
    public List<SpriteImage> sprites;
}

[System.Serializable]
public struct SpriteImage
{
    public TypeDir dir;
    public Sprite sprite;
}
