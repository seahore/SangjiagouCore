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
    enum Mode
    {
        Normal,
        SelectTown,
    }

    public GameObject DebugObject;
    public PlayerInput Input;
    public UIHandler UIHandler;
    public Camera MainCamera;
    public MapRenderer MapRenderer;
    public GraphicRaycaster[] GraphicRaycasters;

    int id;
    public int ID => id;

    Mode mode;

    School playerSchool;

    #region 选择城镇模式使用的属性

    List<string> openingPanels;
    UnityAction<Town> selectTownCallback;

    #endregion

    #region 和用户输入有关的属性

    Vector2 mousePosition;
    Vector2 mouseScroll;

    #endregion

    void OnEnable()
    {
        Input.Point += UpdateMousePosition;
        Input.Point += CameraScroll;
        Input.Point += CheckTooltipAndShow;
        Input.Zoom += UpdateMouseScroll;
        Input.Zoom += CameraZoom;
        Input.LeftSelect += SelectMapTile;
        Input.LeftSelect += SelectTownInSelectTownMode;
        Input.RightSelect += SelectStateByTile;
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
        Input.LeftSelect -= SelectMapTile;
        Input.LeftSelect -= SelectTownInSelectTownMode;
        Input.RightSelect -= SelectStateByTile;
        Input.NextTurn -= NextTurn;
        Input.ShowMenu -= ShowMenu;
    }

    void Awake()
    {
        id = 1;
        mode = Mode.Normal;
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
        foreach (var gr in GraphicRaycasters) {
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

    Vector2Int SelectTile()
    {
        Vector3 t = MainCamera.ScreenToWorldPoint(mousePosition);
        return new Vector2Int((int)t.x, (int)t.y);
    }

    bool MouseOnUI() => GraphicRaycast(mousePosition).Count > 0;

    void SelectMapTile()
    {
        if (mode != Mode.Normal || MouseOnUI()) return;

        Vector2Int pos = SelectTile();
        if (pos.x < 0 || pos.y < 0 || pos.x >= Game.CurrentEntities.MapSize.x || pos.y >= Game.CurrentEntities.MapSize.y)
            return;

        UIHandler.OnSelectTile(pos);
    }

    void SelectStateByTile()
    {
        if (mode != Mode.Normal || MouseOnUI()) return;

        Vector2Int pos = SelectTile();
        if (pos.x < 0 || pos.y < 0 || pos.x >= Game.CurrentEntities.MapSize.x || pos.y >= Game.CurrentEntities.MapSize.y)
            return;

        UIHandler.OnSelectTile(pos, true);
    }

    void SelectTownInSelectTownMode()
    {
        if (mode != Mode.SelectTown || MouseOnUI()) return;

        Vector3 t = MainCamera.ScreenToWorldPoint(mousePosition);
        Vector2Int selection = new Vector2Int((int)t.x, (int)t.y);
        if (selection.x < 0 || selection.y < 0 || selection.x >= Game.CurrentEntities.MapSize.x || selection.y >= Game.CurrentEntities.MapSize.y) return;
        foreach (var i in Game.CurrentEntities.Towns) {
            if (i.Position == selection) {
                QuitSelectTownMode(i);
                return;
            }
        }
    }

    /// <summary>
    /// 检查鼠标目前停留位置的UI对象能否显示悬浮提示，如能则显示
    /// </summary>
    /// <param name="mousePos">鼠标位置坐标</param>
    void CheckTooltipAndShow(Vector2 mousePos)
    {
        ITooltipDisplayable tip = null;
        /*
        var casted = GraphicRaycast(mousePos);
        if (casted.Count == 0) return;
        if(!GraphicRaycast(mousePos)[0].gameObject.TryGetComponent(out tip)) {
            return;
        }
        */
        foreach (var i in GraphicRaycast(mousePos)) {
            if (i.gameObject.TryGetComponent(out tip))
                break;
        }
        if (tip is null) UIHandler.DisplayTooltip(false, "", mousePos);
        else UIHandler.DisplayTooltip(true, tip.TooltipContent, mousePos);
    }

    void NextTurn()
    {
        UIHandler.OnNextTurnButtonClick();
    }

    /// <summary>
    /// 进入选择城镇模式，此时通过右键单击某城镇选择之，然后通过调用回调函数来将选择的城镇传给申请进入选择模式的来源
    /// </summary>
    /// <param name="callback">回调函数</param>
    public void EnterSelectTownMode(UnityAction<Town> callback)
    {
        mode = Mode.SelectTown;
        openingPanels = UIHandler.RecordOpeningPanels();
        UIHandler.HideAllPanels();
        UIHandler.HideAllUpperPanels();
        selectTownCallback = callback;
        UIHandler.ShowTopBanner("选择城镇", "取消", () => { QuitSelectTownMode(null); });
    }

    public void QuitSelectTownMode(Town selected)
    {
        mode = Mode.Normal;
        selectTownCallback(selected);
        UIHandler.HideTopBanner();
        UIHandler.ShowUpperPanels();
        UIHandler.ShowPanels(openingPanels);
    }

    void ShowMenu()
    {
        UIHandler.OnMenuButtonClick();
    }
}
