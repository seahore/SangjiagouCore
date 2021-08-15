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

    GameObject selectOverlay;

    const float COLOR_OVERLAY_Z_DIST = 2;
    const float SELECT_OVERLAY_Z_DIST = 4;

    void Awake()
    {
        selectOverlay = Instantiate(SelectOverlayPrefab);
        selectOverlay.transform.SetParent(Tilemap.transform);
        selectOverlay.SetActive(false);
        new GameObject("Refresh").transform.SetParent(Tilemap.transform);
    }

    void Update()
    {
        
    }

    void Paint(Vector2Int pos, Color color)
    {
        color.a = 0.6667f;
        GameObject o = Instantiate(ColorOverlayPrefab);
        var refresh = Tilemap.transform.Find("Refresh");
        o.transform.SetParent(refresh);
        o.transform.SetPositionAndRotation(new Vector3(pos.x, pos.y, Tilemap.transform.position.z - COLOR_OVERLAY_Z_DIST), Quaternion.identity);
        o.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        o.GetComponent<SpriteRenderer>().material.SetColor("_Color", color);
    }

    public void SelectTile(Vector2Int pos)
    {
        selectOverlay.SetActive(true);
        selectOverlay.transform.SetPositionAndRotation(new Vector3(pos.x, pos.y, Tilemap.transform.position.z - SELECT_OVERLAY_Z_DIST), Quaternion.identity);
    }

    public void DeselectTile()
    {
        selectOverlay.SetActive(false);
    }

    public void RefreshMap()
    {
        var refresh = Tilemap.transform.Find("Refresh").transform;
        for (int i = 0; i < refresh.childCount; ++i) {
            Destroy(refresh.GetChild(i).gameObject);
        }

        GameObject tb = Instantiate(BorderLinePrefab, refresh);
        GameObject bb = Instantiate(BorderLinePrefab, refresh);
        GameObject lb = Instantiate(BorderLinePrefab, refresh);
        GameObject rb = Instantiate(BorderLinePrefab, refresh);

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
    }
}
