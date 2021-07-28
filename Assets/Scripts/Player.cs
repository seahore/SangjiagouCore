using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SangjiagouCore;

public class Player : MonoBehaviour
{
    public GameObject DebugObject;
    public PlayerInput Input;
    public UIHandler UIHandler;
    public Camera MainCamera;
    public MapRenderer MapRenderer;

    Vector2 mousePosition;

    void OnEnable()
    {
        Input.Point += UpdateMousePosition;
        Input.Select += SelectMap;
        Input.NextTurn += NextTurn;
        Input.ShowMenu += ShowMenu;
    }

    void OnDisable()
    {
        Input.Point -= UpdateMousePosition;
        Input.Select -= SelectMap;
        Input.NextTurn -= NextTurn;
        Input.ShowMenu -= ShowMenu;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void UpdateMousePosition(Vector2 v)
    {
        Debug.Log(v);
        mousePosition = v;
    }

    void SelectMap()
    {
        Vector3 t = MainCamera.ScreenToWorldPoint(mousePosition);
        Vector2Int selection = new Vector2Int((int)t.x, (int)t.y);
        if (selection.x < 0 || selection.y < 0 || selection.x >= Game.CurrentEntities.MapSize.x || selection.y >= Game.CurrentEntities.MapSize.y) return;
        MapRenderer.SelectTile(selection);

        Town town = null;
        foreach (var i in Game.CurrentEntities.Towns) {
            if (i.Position == selection) {
                town = i;
                break;
            }
        }
        UIHandler.OnSelectTile(town);
    }

    void NextTurn()
    {
        Debug.Log("NextTurn!");
    }

    void ShowMenu()
    {
        UIHandler.OnMenuButtonClick();
    }
}
