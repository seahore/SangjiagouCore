using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SangjiagouCore;

public class ActionAssignmentPanel : MonoBehaviour
{
    public GameObject ScholarSelection;

    List<Scholar> selecting;

    Town selectingTown;

    void Start()
    {
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

    public void SetArgumentsSetters(List<Type> types)
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        foreach (var type in types) {
            if(type == typeof(Town)) {
                player.EnterSelectTownMode((Town t)=>selectingTown = t);
            }
        }
    }

    public void OnActionsDropdownChanged(int index)
    {
        switch(index) {
            case 0:     // 周游
                // ToDo
                break;
            case 1:     // 游说
                // ToDo
                break;
        }
    }

    public void OnAssignButtonClick()
    {
        var dropdown = transform.Find("Actions Dropdown").GetComponent<Dropdown>();
        if(dropdown.options[dropdown.value].text == "周游") {
            
        }
    }
}
