using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using SangjiagouCore;

public class Player : MonoBehaviour
{
    public GameObject DebugObject;
    public PlayerInput Input;
    public UIHandler UIHandler;
    public Camera MainCamera;
    public MapRenderer MapRenderer;
    public GraphicRaycaster[] GraphicRaycaster;

    int id;
    public int ID => id;

    School playerSchool;

    Vector2 mousePosition;
    Vector2 mouseScroll;

    void OnEnable()
    {
        Input.Point += UpdateMousePosition;
        Input.Point += CameraScroll;
        Input.Zoom += UpdateMouseScroll;
        Input.Zoom += CameraZoom;
        Input.Select += SelectMap;
        Input.NextTurn += NextTurn;
        Input.ShowMenu += ShowMenu;
    }

    void OnDisable()
    {
        Input.Point -= UpdateMousePosition;
        Input.Point -= CameraScroll;
        Input.Zoom -= UpdateMouseScroll;
        Input.Zoom -= CameraZoom;
        Input.Select -= SelectMap;
        Input.NextTurn -= NextTurn;
        Input.ShowMenu -= ShowMenu;
    }

    void Awake()
    {
        id = 1;
    }

    void Start()
    {
        GameObject.FindGameObjectWithTag("Tilemap").GetComponent<MapRenderer>().RefreshMap();
    }

    void Update()
    {

    }

    void UpdateMousePosition(Vector2 v)
    {
        mousePosition = v;
    }

    void CameraScroll(Vector2 mousePosition)
    {
        MainCamera.GetComponent<CameraController>().Scroll(mousePosition);
    }

    void UpdateMouseScroll(Vector2 v)
    {
        mouseScroll = v;
    }

    void CameraZoom(Vector2 scroll)
    {
        MainCamera.GetComponent<CameraController>().Zoom(scroll.y);
    }

    void SelectMap()
    {
        var ped = new ExtendedPointerEventData(EventSystem.current);
        var results = new List<RaycastResult>();
        ped.position = mousePosition;   // 侥幸方法（反编译可知GraphicRaycaster只用这个来做射线检测）
        foreach (var gr in GraphicRaycaster) {
            gr.Raycast(ped, results);
        }
        if (results.Count > 0) return;
        
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

    }

    void ShowMenu()
    {
        UIHandler.OnMenuButtonClick();
    }
}
