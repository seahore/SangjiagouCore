using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SangjiagouCore;

public class ActionAssignmentPanel : MonoBehaviour
{
    public GameObject ScholarSelection;
    public GameObject SelectTownButtonPrefab;

    readonly Color colorForAssigned = new Color((float)0xAF / 0xFF, (float)0xEE / 0xFF, (float)0xEE / 0xFF);

    Town townSelected;

    Player player;
    List<Scholar> selecting;

    Dropdown dropdown;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        dropdown = transform.Find("Actions Dropdown").GetComponent<Dropdown>();
        Refresh();
    }

    void Update()
    {
        
    }

    public void Refresh()
    {
        townSelected = null;

        School playerSchool = Game.CurrentEntities.GetPlayerSchool(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().ID);
        selecting = new List<Scholar>();

        var list = transform.Find("Scroll View/Viewport/Scholars List");
        for (int i = 0; i < list.childCount; ++i) {
            Destroy(list.GetChild(i).gameObject);
        }

        dropdown.value = 0;

        var unitHeight = ScholarSelection.GetComponent<RectTransform>().rect.height;
        var unitWidth = ScholarSelection.GetComponent<RectTransform>().rect.width;
        float th = -8;
        float tw = -8;
        for (int i = 0; i < playerSchool.Members.Count; ++i) {
            var s = playerSchool.Members[i];
            var o = Instantiate(ScholarSelection, list).transform;
            o.GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn) {
                if (isOn) selecting.Add(s);
                else selecting.Remove(s);
            });
            o.GetComponent<RectTransform>().offsetMin = new Vector2(tw + 225, th - unitHeight);
            o.GetComponent<RectTransform>().offsetMax = new Vector2(tw + unitWidth + 225, th);
            o.Find("Avatar").GetComponent<Image>().sprite = s.Avatar;
            o.Find("Name").GetComponent<Text>().text = s.FullCourtesyName;
            if(!(s.Action is null)) {
                o.Find("Button Face").GetComponent<Image>().color = colorForAssigned;
            }
            if (i % 3 == 2)
                th -= unitHeight + 5;
            else
                tw -= unitWidth + 5;
        }
    }

    public void OnSelectTownButtonClick(Text buttonText)
    {
        player.EnterSelectTownMode((Town t) => {
            if (t is null) return;
            townSelected = t;
            buttonText.text = townSelected.Name;
        });
    }

    public void SetArgumentsSetters(List<Type> types)
    {
        float x = 115;
        float y = 16;
        float h = 32;
        var argPanel = transform.Find("Action Arguments");
        for (int i = 0; i < argPanel.childCount; ++i) {
            Destroy(argPanel.GetChild(i).gameObject);
        }
        foreach (var type in types) {
            y -= h;
            if (type == typeof(Town)) {
                var o = Instantiate(SelectTownButtonPrefab, argPanel).transform;
                o.name = "Select Town Button";
                o.GetComponent<Button>().onClick.AddListener(() => { OnSelectTownButtonClick(o.Find("Text").GetComponent<Text>()); });
                o.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            }
        }
    }

    public void OnActionsDropdownChanged()
    {
        var types = new List<Type>();
        switch (dropdown.value) {
            case 0:     // 闲在
                break;
            case 1:     // 周游
                types.Add(typeof(Town));
                break;
            case 2:     // 游说
                // ToDo
                break;
        }
        SetArgumentsSetters(types);
    }

    public void OnAssignButtonClick()
    {
        switch (dropdown.value) {
            case 0:     // 闲在
                foreach (var s in selecting) {
                    s.Action = null;
                }
                break;
            case 1:     // 周游
                if (townSelected is null)
                    return;
                foreach (var s in selecting) {
                    if(s.Location.Name != townSelected.Name) 
                        s.Action = new TravelAction(s, s.Location, townSelected);
                }
                break;
            case 2:     // 游说
                // ToDo
                break;
        }
        Refresh();
    }
}
