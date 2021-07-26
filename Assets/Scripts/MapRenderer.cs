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
    public GameObject ColorMaskPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Paint(Vector2Int pos, Color color)
    {
        color.a = 0.5f;
        GameObject o = Instantiate(ColorMaskPrefab);
        o.transform.SetParent(transform);
        o.transform.SetPositionAndRotation(new Vector3(pos.x, pos.y, transform.position.z - 10), Quaternion.identity);
        o.GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        o.GetComponent<SpriteRenderer>().material.SetColor("_Color", color);
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
