using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
        Input.Point += CheckTooltipAndShow;
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
        Input.Point -= CheckTooltipAndShow;
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

    /// <summary>
    /// 使用侥幸方法做UI射线检测（反编译可知GraphicRaycaster只用这个来做射线检测）
    /// </summary>
    /// <param name="mousePos">检测位置的坐标</param>
    List<RaycastResult> GraphicRaycast(Vector2 mousePos)
    {
        var ped = new ExtendedPointerEventData(EventSystem.current);
        var results = new List<RaycastResult>();
        ped.position = mousePos;
        foreach (var gr in GraphicRaycaster) {
            gr.Raycast(ped, results);
        }
        return results;
    }

    void UpdateMousePosition(Vector2 v)
    {
        mousePosition = v;
    }

    void CameraScroll(Vector2 mousePos)
    {
        MainCamera.GetComponent<CameraController>().Scroll(mousePos);
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
        if (GraphicRaycast(mousePosition).Count > 0) return;
        
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

    /// <summary>
    /// 检查鼠标目前停留位置的UI对象能否显示悬浮提示，如能则显示
    /// </summary>
    /// <param name="mousePos">鼠标位置坐标</param>
    void CheckTooltipAndShow(Vector2 mousePos)
    {
        ITooltipDisplayable tip = null;
        foreach (var i in GraphicRaycast(mousePos)) {
            if (i.gameObject.TryGetComponent(out tip))
                break;
        }
        if (tip is null) UIHandler.DisplayTooltip(false, "", mousePos);
        else UIHandler.DisplayTooltip(true, tip.TooltipContent, mousePos);
    }

    void NextTurn()
    {

    }

    /// <summary>
    /// 进入选择城镇模式，此时通过右键单击某城镇选择之，然后通过调用回调函数来将选择的城镇传给申请进入选择模式的来源
    /// </summary>
    /// <param name="callback">回调函数</param>
    public void EnterSelectTownMode(UnityAction<Town> callback)
    {
        Town selected = null;
        UIHandler.HideAllUILayerPanels();

        // 处理选择的代码
        
        callback(selected);
    }

    void ShowMenu()
    {
        UIHandler.OnMenuButtonClick();
    }
}
