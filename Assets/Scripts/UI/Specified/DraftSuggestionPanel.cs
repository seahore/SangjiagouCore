using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SangjiagouCore;

public class DraftSuggestionPanel : MonoBehaviour
{
    public GameObject MaskPrefab;
    public GameObject SelectTownButtonPrefab;
    public GameObject SelectStateButtonPrefab;

    Player player;

    UnityAction<StateAction> callback;

    Town townSelected;
    State stateSelected;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        var selections = transform.Find("Action Selections");
        for (int i = 0; i < selections.childCount; ++i) {
            var toggle = selections.GetChild(i).GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((bool isOn) => { OnTogglesChanged(isOn, toggle.name); });
        }
    }

    public void Reset()
    {
        transform.Find("OK Button").GetComponent<Button>().enabled = false;

        var selections = transform.Find("Action Selections");
        for (int i = 0; i < selections.childCount; ++i) {
            selections.GetChild(i).GetComponent<Toggle>().isOn = false;
        }
        SetArgumentsSetters(null);
    }

    void OnTogglesChanged(bool isOn, string name)
    {
        if (!isOn) return;  // 如果没有这句，因为可能设置了事件参数只有一个时自动点击，所以可能在Reset()里取消选择或者选择其他选项时仍会调用这个函数。

        transform.Find("OK Button").GetComponent<Button>().enabled = true;
        switch (name) {
            case "Declare War":
            case "Improve Relationships":
                SetArgumentsSetters(new List<Type> { typeof(State) });
                break;
            case "Declare Aggressive War":
            case "Develop":
                SetArgumentsSetters(new List<Type> { typeof(Town) });
                break;
            case "Collect Folk Songs":
            case "Hunt":
            case "Sacrifice":
            case "Impress":
            case "Commandeer":
            case "Politics Research":
            case "Military Research":
            case "Relieve Poor":
            default:
                SetArgumentsSetters(null);
                break;
        }
    }

    public void SetCallback(UnityAction<StateAction> f)
    {
        callback = f;
    }

    public void OnSelectTownButtonClick(Text buttonText)
    {
        player.EnterSelectTownMode((Town t) => {
            if (t is null) return;
            townSelected = t;
            buttonText.text = townSelected.Name;
        });
    }

    public void OnSelectStateButtonClick(Button button)
    {
        player.EnterSelectTownMode((Town t) => {
            if (t is null) return;
            stateSelected = t.Controller;
            button.transform.Find("Text").GetComponent<Text>().text = stateSelected.Name;
            button.transform.Find("Flag").GetComponent<Image>().sprite = null;
        });
    }

    public void SetArgumentsSetters(List<Type> types)
    {
        float x = 115;
        float y = -16;
        float h = 36;
        var argPanel = transform.Find("Action Arguments");
        for (int i = 0; i < argPanel.childCount; ++i) {
            Destroy(argPanel.GetChild(i).gameObject);
        }
        if (types is null) return;
        foreach (var type in types) {
            y -= h;
            Transform o = null;
            if (type == typeof(Town)) {
                o = Instantiate(SelectTownButtonPrefab, argPanel).transform;
                o.name = "Select Town Button";
                o.GetComponent<Button>().onClick.AddListener(() => { OnSelectTownButtonClick(o.Find("Text").GetComponent<Text>()); });
            } else if (type == typeof(State)) {
                o = Instantiate(SelectStateButtonPrefab, argPanel).transform;
                o.name = "Select State Button";
                o.GetComponent<Button>().onClick.AddListener(() => { OnSelectStateButtonClick(o.GetComponent<Button>()); });
            }
            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            if (Settings.Values.AutoSelectMode && types.Count == 1)
                o.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void OnCloseButtonClick()
    {
        GetComponent<Animator>().SetBool("Open", false);
    }

    public void OnOKButtonClick()
    {
        string name = null;
        var selections = transform.Find("Action Selections");
        for (int i =0; i<selections.childCount; ++i) {
            if(selections.GetChild(i).TryGetComponent(out Toggle toggle) && toggle.isOn) {
                name = toggle.name;
                break;
            }
        }
        if (name is null) return;
        StateAction a = null;
        switch (name) {
            case "Declare War":
                a = new DeclareWarAction(null, null, stateSelected);
                break;
            case "Declare Aggressive War":
                a = new DeclareAggressiveWarAction(null, null, townSelected.Controller, townSelected);
                break;
            case "Develop":
                a = new DevelopAction(null, null, townSelected);
                break;
            case "Collect Folk Songs":
                a = new CollectFolkSongsAction(null, null);
                break;
            case "Hunt":
                a = new HuntAction(null, null);
                break;
            case "Sacrifice":
                a = new SacrificeAction(null, null);
                break;
            case "Impress":
                a = new ImpressAction(null, null);
                break;
            case "Commandeer":
                a = new CommandeerAction(null, null);
                break;
            case "Improve Relationships":
                a = new ImproveRelationshipsAction(null, null, stateSelected);
                break;
            case "Politics Research":
                a = new PoliticsResearchAction(null, null);
                break;
            case "Military Research":
                a = new MilitaryResearchAction(null, null);
                break;
            case "Relieve Poor":
                a = new RelievePoorAction(null, null);
                break;
        }
        callback(a);
        OnCloseButtonClick();
    }
}
