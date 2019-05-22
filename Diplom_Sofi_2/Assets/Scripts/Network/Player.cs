using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Класс представляющий логику игрока
/// </summary>
public class Player : NetworkBehaviour
{
    private static bool turn = false;

    [SerializeField] GameObject inputPrefab;
    [SerializeField] GameObject playerPref;
    [SerializeField] GameObject victoryPref;

    private Transform player;
    private SpriteRenderer playerRender;
    private InputsController input;
    private MapController mapController;
    public int id;

    private Vector2Int currentPos;
    private Vector2Int serverCurrentPos;
    private bool isUpdatePos = false;

    private void Awake()
    {
        id = -1;
    }

    /// <summary>
    /// Создание игрока на сцене
    /// </summary>
    public void CreatePlayer(Vector3 pos, Vector3 inputPos, int _id)
    {
        mapController = GetComponent<MapController>();
        player = Instantiate(playerPref, pos, Quaternion.identity, transform).transform;
        playerRender = player.GetComponent<SpriteRenderer>();
        Transform canvas = GameObject.Find("Canvas").transform;
        input = Instantiate(inputPrefab, inputPos, Quaternion.identity, canvas).GetComponent<InputsController>();
        id = _id;

        if (isLocalPlayer) {
            currentPos = mapController.beginPoint;
            mapController.Map[currentPos.y, currentPos.x].OpenTile();
            CmdStep(mapController.beginPoint.x, mapController.beginPoint.y);
        }
    }

    /// <summary>
    /// Метод выполняющийся каждый кадр
    /// Считывает ввод и выполняет логику хода
    /// </summary>
    private void Update()
    {
        // color changing
        if (playerRender != null) {
            if (CalculateTurn()) {
                playerRender.color = Color.green;
            }
            else {
                playerRender.color = Color.red;
            }
        }

        // steping
        if (isServer) {
            if (isUpdatePos) {
                isUpdatePos = false;
                RpcStepAll(serverCurrentPos.x, serverCurrentPos.y, turn);
            }
        }

        // input logic
        if (isLocalPlayer) {
            if (input != null && CalculateTurn()) {
                if (input.Up) {
                    Step(Vector2Int.up);
                }
                else if (input.Left) {
                    Step(Vector2Int.left);
                }
                else if (input.Down) {
                    Step(Vector2Int.down);
                }
                else if (input.Right) {
                    Step(Vector2Int.right);
                }
            }
        }

        // hand input
        if (isClient) {
            if (input != null && CalculateTurn()) {
                if (input.Up) {
                    CmdSetHandButton(TypeDir.Top);
                }
                else if (input.Left) {
                    CmdSetHandButton(TypeDir.Left);
                }
                else if (input.Down) {
                    CmdSetHandButton(TypeDir.Below);
                }
                else if (input.Right) {
                    CmdSetHandButton(TypeDir.Right);
                }
            }
        }
    }

    #region Step by Step
    /// <summary>
    /// Метод выполняющий ход игрока в заданом направлении
    /// </summary>
    private void Step(Vector2Int dir)
    {
        if (CanPass(dir)) {
            currentPos += dir;
        }
        else {
            Debug.Log("Cant Pass");
        }

        mapController.Map[currentPos.y, currentPos.x].OpenTile();

        if(currentPos == mapController.endPoint) {
            Transform canvas = GameObject.Find("Canvas").transform;
            Instantiate(victoryPref, new Vector3(512, 384, 0), Quaternion.identity, canvas);
        }

        CmdStep(currentPos.x, currentPos.y);
    }

    [Command]
    private void CmdStep(int x, int y)
    {
        isUpdatePos = true;
        serverCurrentPos = new Vector2Int(x, y);

        turn = !turn;
    }

    [ClientRpc]
    private void RpcStepAll(int x, int y, bool _turn)
    {
        player.position = mapController.Map[y, x].position;
        turn = _turn;
    }

    /// <summary>
    /// Проверка может ли игрок походить в выбраном направлении
    /// </summary>
    private bool CanPass(Vector2Int dir)
    {
        TypeDir type = Helper.ConvertVectorToDir(dir);
        Vector2Int nextPos = currentPos + dir;
        if (nextPos.y >= 0 && nextPos.y < mapController.Map.GetLength(0) && nextPos.x >= 0 && nextPos.x < mapController.Map.GetLength(1)) {
            return mapController.Map[currentPos.y, currentPos.x].walls[type] == 0;
        }

        return false;
    }

    /// <summary>
    /// Определение может ли игрок ходить в данный момент
    /// </summary>
    private bool CalculateTurn()
    {
        return (turn && id == 0) || (!turn && id == 1);
    }
    #endregion

    #region Trust or Not
    [Command]
    private void CmdSetHandButton(TypeDir dir)
    {
        RpcSetHandButton(dir);
    }

    [ClientRpc]
    private void RpcSetHandButton(TypeDir dir)
    {
        input.SetDirectionButton(dir);
    }
    #endregion
}
