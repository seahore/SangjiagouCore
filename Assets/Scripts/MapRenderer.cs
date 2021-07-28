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

    GameObject selectOverlay;

    const float COLOR_OVERLAY_Z_DIST = 2;
    const float SELECT_OVERLAY_Z_DIST = 4;

    // Start is called before the first frame update
    void Start()
    {
        selectOverlay = Instantiate(SelectOverlayPrefab);
        selectOverlay.transform.SetParent(transform);
        selectOverlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Paint(Vector2Int pos, Color color)
    {
        color.a = 0.5f;
        GameObject o = Instantiate(ColorOverlayPrefab);
        o.transform.SetParent(transform);
        o.transform.SetPositionAndRotation(new Vector3(pos.x, pos.y, transform.position.z - COLOR_OVERLAY_Z_DIST), Quaternion.identity);
        o.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        o.GetComponent<SpriteRenderer>().material.SetColor("_Color", color);
    }

    public void SelectTile(Vector2Int pos)
    {
        selectOverlay.SetActive(true);
        selectOverlay.transform.SetPositionAndRotation(new Vector3(pos.x, pos.y, transform.position.z - SELECT_OVERLAY_Z_DIST), Quaternion.identity);
    }

    public void DeselectTile()
    {
        selectOverlay.SetActive(false);
    }

    public void RefreshMap()
    {
        GameObject tb = new GameObject("Top Border", typeof(LineRenderer));
        GameObject bb = new GameObject("Bottom Border", typeof(LineRenderer));
        GameObject lb = new GameObject("Left Border", typeof(LineRenderer));
        GameObject rb = new GameObject("Right Border", typeof(LineRenderer));

        tb.transform.SetParent(Tilemap.transform);
        bb.transform.SetParent(Tilemap.transform);
        lb.transform.SetParent(Tilemap.transform);
        rb.transform.SetParent(Tilemap.transform);

        tb.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(0, Game.CurrentEntities.MapSize.y, 0), new Vector3(Game.CurrentEntities.MapSize.x, Game.CurrentEntities.MapSize.y, 0) });
        bb.GetComponent<LineRenderer>().SetPositions(new Vector3[] { Vector3.zero, new Vector3(Game.CurrentEntities.MapSize.x, 0, 0) });
        lb.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(0, Game.CurrentEntities.MapSize.y, 0), Vector3.zero });
        rb.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(Game.CurrentEntities.MapSize.x, Game.CurrentEntities.MapSize.y, 0), new Vector3(Game.CurrentEntities.MapSize.x, 0, 0) });
        

        foreach (var t in Game.CurrentEntities.Towns) {
            Tilemap.SetTile(new Vector3Int(t.Position.x, t.Position.y, 0), TownTile);
            Paint(t.Position, t.Controller.PrimaryColor);
        }
    }
}
