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

    Player player;
    List<Scholar> selecting;

    Town selectingTown;

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
        School playerSchool = Game.CurrentEntities.GetPlayerSchool(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().ID);
        selecting = new List<Scholar>();
        var list = transform.Find("Scroll View/Viewport/Scholars List");
        for (int i = 0; i < list.childCount; ++i) {
            Destroy(list.GetChild(i).gameObject);
        }
        var unitHeight = ScholarSelection.GetComponent<RectTransform>().rect.height;
        var unitWidth = ScholarSelection.GetComponent<RectTransform>().rect.width;
        float th = -8;
        float tw = -8;
        for (int i = 0; i < playerSchool.Members.Count; ++i) {
            var s = playerSchool.Members[i];
            var o = Instantiate(ScholarSelection, list);
            o.GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn) {
                if (isOn) selecting.Add(s);
                else selecting.Remove(s);
            });
            o.GetComponent<RectTransform>().offsetMin = new Vector2(tw + 225, th - unitHeight);
            o.GetComponent<RectTransform>().offsetMax = new Vector2(tw + unitWidth + 225, th);
            o.transform.Find("Avatar").GetComponent<Image>().sprite = s.Avatar;
            o.transform.Find("Name").GetComponent<Text>().text = s.FullCourtesyName;
            if (i % 3 == 2)
                th -= unitHeight + 5;
            else
                tw -= unitWidth + 5;
        }
    }

    public void OnSelectTownButtonClick(Text buttonText)
    {
        player.EnterSelectTownMode((Town t) => { selectingTown = t; buttonText.text = selectingTown.Name; });
    }

    public void SetArgumentsSetters(List<Type> types)
    {
        var dt = dropdown.GetComponent<RectTransform>();
        float x = dt.anchoredPosition.x;
        float y = dt.anchoredPosition.y;
        float h = dt.rect.height;
        foreach (var type in types) {
            y -= h;
            if (type == typeof(Town)) {
                var o = Instantiate(SelectTownButtonPrefab, transform).transform;
                o.GetComponent<Button>().onClick.AddListener(()=> { OnSelectTownButtonClick(o.Find("Text").GetComponent<Text>()); });
                o.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            }
        }
    }

    public void OnActionsDropdownChanged()
    {
        switch(dropdown.value) {
            case 0:     // 周游
                SetArgumentsSetters(new List<Type> { typeof(Town) });
                break;
            case 1:     // 游说
                // ToDo
                break;
        }
    }

    public void OnAssignButtonClick()
    {
        switch (dropdown.value) {
            case 0:     // 周游
                // ToDo
                break;
            case 1:     // 游说
                // ToDo
                break;
        }
    }
}
