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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshMap()
    {
        foreach (var t in Game.CurrentEntities.Towns) {
            Tilemap.SetTile(new Vector3Int(t.Position.x, t.Position.y, 0), TownTile);
        }
    }
}
