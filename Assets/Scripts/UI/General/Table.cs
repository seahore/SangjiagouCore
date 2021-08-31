using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Table")]
public class Table : MonoBehaviour
{
    public enum DirectionMode
    {
        Vertical,
        Horizontal
    }

    public DirectionMode Direction;
    public List<string> ColumnsText;
    public List<string> Types;
    public float rowHeight;
    public Font Font;
    public int FontSize;
    public bool BoldHeaders = true;
    public TextAnchor Alignment = TextAnchor.MiddleCenter;

    List<Type> _types;
    public List<Type> ColumnTypes {
        get => _types;
        set => _types = value;
    }
    List<List<object>> _table;
    public List<List<object>> TableValue => _table;

    void Start()
    {
        Reset();
    }

    public string[,] ReadableTable {
        get {
            if (_table.Count == 0 || _table[0].Count == 0)
                return new string[0, 0];
            string[,] table = new string[_table[0].Count, _table.Count];
            for (int i = 0; i < _table[0].Count; i++) {
                for (int j = 0; j < _table.Count; j++) {
                    table[i, j] = _table[j][i].ToString();
                }
            }
            return table;
        }
    }

    public void AddRow(params object[] row)
    {
        if (row.Length < _table.Count) throw new ArgumentException("参数个数不足");
        for (int i = 0; i < _types.Count; i++) {
            if (row[i].GetType() != _types[i]) {
                throw new ArgumentException($"参数{i}类型不正确");
            }
        }
        for (int i = 0; i < _table.Count; i++) {
            _table[i].Add(row[i]);
        }
        UpdateGUI();
    }

    public void InsertRow(int index, params object[] row)
    {
        if (row.Length < _table.Count) throw new ArgumentException("参数个数不足");
        for (int i = 0; i < _types.Count; i++) {
            if (row[i].GetType() != _types[i]) {
                throw new ArgumentException($"参数{i}类型不正确");
            }
        }
        for (int i = 0; i < _table.Count; i++) {
            _table[i].Insert(index, row[i]);
        }
        UpdateGUI();
    }

    public void ClearTable()
    {
        _table = new List<List<object>>();
        for (int i = 0; i < _types.Count; i++) {
            _table.Add(new List<object>());
        }
        UpdateGUI();
    }

    public void UpdateGUI()
    {
        var rt = GetComponent<RectTransform>();
        rt.anchorMin = new Vector2 { x = 0, y = 1 };
        rt.anchorMax = new Vector2 { x = 1, y = 1 };
        rt.offsetMax = Vector2.zero;
        rt.offsetMin = new Vector2 { x = 0, y = -_table[0].Count * rowHeight };

        for (int i = 0; i < rt.childCount; i++) {
            Destroy(rt.GetChild(i).gameObject);
        }

        float columnWidth = GetComponent<RectTransform>().rect.width / _table.Count;

        for (int j = 0; j < _table.Count; j++) {    // column
            var grid = new GameObject($"Header {j}", typeof(RectTransform), typeof(Text)).transform;
            grid.SetParent(transform);
            var gridRt = grid.GetComponent<RectTransform>();
            gridRt.anchorMax = gridRt.anchorMin = new Vector2 { x = 0, y = 1 };
            gridRt.pivot = new Vector2 { x = 0, y = 1 };
            gridRt.offsetMax = new Vector2((j + 1) * columnWidth, 0);
            gridRt.offsetMin = new Vector2(j * columnWidth, -rowHeight);
            var text = grid.GetComponent<Text>();
            text.font = Font;
            text.fontSize = FontSize;
            text.fontStyle = BoldHeaders ? FontStyle.Bold : FontStyle.Normal;
            text.color = Color.black;
            text.alignment = Alignment;
            text.text = ColumnsText[j];
        }

        for (int i = 0; i < _table[0].Count; i++) {     // row
            for (int j = 0; j < _table.Count; j++) {    // column
                var grid = new GameObject($"{i}, {j}", typeof(RectTransform), typeof(Text)).transform;
                grid.SetParent(transform);
                var gridRt = grid.GetComponent<RectTransform>();
                gridRt.anchorMax = gridRt.anchorMin = new Vector2 { x = 0, y = 1 };
                gridRt.pivot = new Vector2 { x = 0, y = 1 };
                gridRt.offsetMax = new Vector2((j + 1) * columnWidth, -(i + 1) * rowHeight);
                gridRt.offsetMin = new Vector2(j * columnWidth, -(i + 2) * rowHeight);
                var text = grid.GetComponent<Text>();
                text.font = Font;
                text.fontSize = FontSize;
                text.fontStyle = FontStyle.Normal;
                text.color = Color.black;
                text.alignment = Alignment;
                text.text = _table[j][i].ToString();
            }
        }
    }

    public void Reset()
    {
        _types = new List<Type>();
        if (!(Types is null) && Types.Count > 0) {
            for (int i = 0; i < Types.Count; i++) {
                _types.Add(Type.GetType(Types[i], true));
            }
        }
        _table = new List<List<object>>();
        for (int i = 0; i < _types.Count; i++) {
            _table.Add(new List<object>());
        }
        UpdateGUI();
    }
}

/*
[CustomEditor(typeof(Table))]
public class TableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
*/