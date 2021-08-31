using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SangjiagouCore;

/// <summary>
/// 根据地图数据渲染地图
/// </summary>
public class MapRenderer : MonoBehaviour
{
    public Tilemap Tilemap;
    public TileBase TownTile;
    public int PixelsPerUnit = 100;
    public GameObject ColorOverlayPrefab;
    public GameObject SelectOverlayPrefab;
    public GameObject BorderLinePrefab;
    public GameObject RoadLinePrefab;

    Transform _selectOverlays;
    Transform _refresh;

    const float COLOR_OVERLAY_Z_DIST = 2;
    const float SELECT_OVERLAY_Z_DIST = 4;

    void Awake()
    {
        _selectOverlays = new GameObject("Select Overlays").transform;
        _selectOverlays.SetParent(Tilemap.transform);
        _selectOverlays.SetPositionAndRotation(new Vector3(0, 0, - SELECT_OVERLAY_Z_DIST), Quaternion.identity);

        _refresh = new GameObject("Refresh").transform;
        _refresh.SetParent(Tilemap.transform);
    }

    void Update()
    {
        
    }

    void Paint(Vector2Int pos, Color color)
    {
        color.a = 0.6667f;
        GameObject o = Instantiate(ColorOverlayPrefab);
        o.transform.SetParent(_refresh);
        o.transform.SetPositionAndRotation(new Vector3(pos.x, pos.y, Tilemap.transform.position.z - COLOR_OVERLAY_Z_DIST), Quaternion.identity);
        o.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        o.GetComponent<SpriteRenderer>().material.SetColor("_Color", color);
    }

    void DrawRoad(Vector2Int a, Vector2Int b)
    {
        var road = Instantiate(RoadLinePrefab, _refresh);
        road.GetComponent<LineRenderer>().SetPositions(new Vector3[]{ new Vector2(a.x + 0.5f, a.y + 0.5f), new Vector2(b.x + 0.5f, b.y + 0.5f)});
    }

    public void SelectTile(Vector2Int pos)
    {
        DeselectTile();
        var overlay = Instantiate(SelectOverlayPrefab, _selectOverlays).transform;
        overlay.SetPositionAndRotation(new Vector3(pos.x, pos.y, 0), Quaternion.identity);
    }

    public void SelectTiles(List<Vector2Int> positions)
    {
        DeselectTile();
        foreach (var i in positions) {
            var overlay = Instantiate(SelectOverlayPrefab, _selectOverlays).transform;
            overlay.SetPositionAndRotation(new Vector3(i.x, i.y, 0), Quaternion.identity);
        }
    }

    public void DeselectTile()
    {
        for (int i = 0; i < _selectOverlays.childCount; i++) {
            Destroy(_selectOverlays.GetChild(i).gameObject);
        }
    }

    public void RefreshMap()
    {
        for (int i = 0; i < _refresh.childCount; ++i) {
            Destroy(_refresh.GetChild(i).gameObject);
        }

        GameObject tb = Instantiate(BorderLinePrefab, _refresh);
        GameObject bb = Instantiate(BorderLinePrefab, _refresh);
        GameObject lb = Instantiate(BorderLinePrefab, _refresh);
        GameObject rb = Instantiate(BorderLinePrefab, _refresh);

        tb.name = "Top Border";
        bb.name = "Bottom Border";
        lb.name = "Left Border";
        rb.name = "Right Border";

        float width = tb.GetComponent<LineRenderer>().startWidth;
        float z = Tilemap.transform.position.z;
        tb.GetComponent<LineRenderer>().SetPositions(new Vector3[] {
            new Vector3(-width, Game.CurrentEntities.MapSize.y + 0.5f * width, z),
            new Vector3(Game.CurrentEntities.MapSize.x + width, Game.CurrentEntities.MapSize.y + 0.5f * width, z)
        });
        bb.GetComponent<LineRenderer>().SetPositions(new Vector3[] { 
            new Vector3(-width, -0.5f * width, z),
            new Vector3(Game.CurrentEntities.MapSize.x + width, -0.5f * width, z)
        });
        lb.GetComponent<LineRenderer>().SetPositions(new Vector3[] {
            new Vector3(-0.5f * width, Game.CurrentEntities.MapSize.y + width, z),
            new Vector3(-0.5f * width, -width, z)
        });
        rb.GetComponent<LineRenderer>().SetPositions(new Vector3[] {
            new Vector3(Game.CurrentEntities.MapSize.x + 0.5f * width, Game.CurrentEntities.MapSize.y + width, z),
            new Vector3(Game.CurrentEntities.MapSize.x + 0.5f * width, -width, z)
        });

        foreach (var t in Game.CurrentEntities.Towns) {
            Tilemap.SetTile(new Vector3Int(t.Position.x, t.Position.y, 0), TownTile);
            Paint(t.Position, t.Controller.PrimaryColor);
        }

        foreach (var r in Game.CurrentEntities.Roads) {
            DrawRoad(r.Town1.Position, r.Town2.Position);
        }
    }
}
