using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// класс предназначен для представления лабиринта как единой сущности для доступа с сервера и клиента
/// </summary>
public class MapController : NetworkBehaviour
{
    [SerializeField] MapInfo info;

    //server side
    private static bool firstPlayerConnected = false;
    private static bool secondPlayerConnected = false;
    private static bool firstInitialized = false;
    private static bool secondInitialized = false;

    //client side
    public Cell[,] Map { get; private set; }
    public Vector2Int beginPoint;
    public Vector2Int endPoint;

    private List<GameObject> cells = new List<GameObject>();
    private List<GameObject> closeTiles = new List<GameObject>();
    private List<GameObject> lineWay = new List<GameObject>();

    public int id;
    private static int nextId = 0;

    /// <summary>
    /// Метод срабатывает в начале работы программы
    /// Начальная инициализация
    /// </summary>
    private void Awake()
    {
        id = nextId++;
    }

    /// <summary>
    /// Последующая инициализация
    /// Генерируем лабиринт и создаём объект класса Player
    /// </summary>
    void Start()
    {
        if (isLocalPlayer) {

            CmdRegister(id);
        }
    }

    [Command]
    private void CmdRegister(int id)
    {
        if(id == 0) {
            firstPlayerConnected = true;
        }
        if(id == 1) {
            secondPlayerConnected = true;
        }
    }

    // Test update
    private void Update()
    {
        // initialize
        if(firstInitialized && secondInitialized) {
            firstPlayerConnected = secondPlayerConnected = firstInitialized = secondInitialized = false;
        }

        if (firstPlayerConnected && secondPlayerConnected) {
            if (isServer) {
                if (id == 0) {
                    int seed = Random.Range(0f, 1f).ToString().GetHashCode();
                    RpcInitialize(seed, -10, 1, 270, 80);
                    firstInitialized = true;
                }
                if (id == 1) {
                    int seed = Random.Range(0f, 1f).ToString().GetHashCode();
                    RpcInitialize(seed, 10, 1, 1024 - 270, 80);
                    secondInitialized = true;
                }
            }
        }
    }

    #region Initialize
    [ClientRpc]
    private void RpcInitialize(int seed, float posX, float posY, float contrX, float contrY)
    {
        transform.position = new Vector3(posX, posY, 0);

        Map = MazeGenerator.Generate(seed, info.height, info.width, info.endUpPercent);
        MazeGenerator.SetBeginEnd(Map, out beginPoint, out endPoint);

        VizualizeMaze();

        if (isLocalPlayer)
            CloseMaze();

        if (!isLocalPlayer)
            VizualizeTrueWay();

        Vector3 startPos = new Vector3(-Map.GetLength(1) / 2f + .5f, -Map.GetLength(0) / 2f + .5f, 0) + transform.position;
        GetComponent<Player>().CreatePlayer(startPos, new Vector3(contrX, contrY, 0), id);
    }

    /// <summary>
    /// Получения сгенерированого лаибиринта и визуализация его на сцене
    /// </summary>
    private void VizualizeMaze()
    {
        //clen up
        for (int i = 0; i < cells.Count; i++) {
            Destroy(cells[i]);
        }
        cells.Clear();

        //vizualize
        int h = Map.GetLength(0);
        int w = Map.GetLength(1);

        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                foreach (var wall in Map[y, x].walls) {

                    Map[y, x].position = new Vector3(x - w / 2f + .5f, y - h / 2f + .5f, 0) + transform.position;

                    if (wall.Value != 0) {

                        var sprRend = new GameObject().AddComponent<SpriteRenderer>();
                        sprRend.gameObject.name = $"tile-{y}-{x}";
                        sprRend.sprite = info.sprites.Find(val => val.dir == wall.Key).sprite;

                        sprRend.transform.position = Map[y, x].position;
                        sprRend.transform.parent = transform;
                        cells.Add(sprRend.gameObject);
                    }
                }
            }
        }
    }

    private void CloseMaze()
    {
        //clen up
        for (int i = 0; i < closeTiles.Count; i++) {
            Destroy(closeTiles[i]);
        }
        closeTiles.Clear();

        //vizualize
        int h = Map.GetLength(0);
        int w = Map.GetLength(1);

        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                GameObject go = Instantiate(info.closeTile, Map[y, x].position, Quaternion.identity, transform);
                go.name = $"closeTile-{y}-{x}";
                Map[y, x].CloseTile = go;
                closeTiles.Add(go);
            }
        }
    }

    private void VizualizeTrueWay()
    {
        //clen up
        for (int i = 0; i < lineWay.Count; i++) {
            Destroy(lineWay[i]);
        }
        lineWay.Clear();

        //vizualize
        List<Vector2Int> way = MazeGenerator.CalculateTrueWayInMaze(Map, beginPoint, endPoint);
        if (way == null)
            return;

        for(int i = 0; i < way.Count - 1; i++) {
            Cell begin = Map[way[i].y, way[i].x];
            Cell end = Map[way[i + 1].y, way[i + 1].x];
            Vector3 position = (begin.position + end.position)/ 2f;
            Quaternion rotate = CalculateRotation(end.position - begin.position);
            GameObject go = Instantiate(info.line, position, rotate, transform);
            go.name = $"line-{i}";
            lineWay.Add(go);
        }
    }

    private Quaternion CalculateRotation(Vector3 direction)
    {
        if(direction.x == 0 && direction.y != 0) {
            return Quaternion.identity;
        }
        else if(direction.y == 0 && direction.x != 0) {
            return Quaternion.Euler(0, 0, 90);
        }

        return Quaternion.identity;
    }
    #endregion
}
