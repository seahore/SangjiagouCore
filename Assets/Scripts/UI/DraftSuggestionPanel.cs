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

        transform.Find("OK Button").GetComponent<Button>().enabled = false;

        var mask = Instantiate(MaskPrefab, transform.parent);
        transform.SetParent(mask.transform);

        var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCam.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Focused")) {
            mainCam.GetComponent<Animator>().SetTrigger("Unfocus");
        }

        var selections = transform.Find("Action Selections");
        for (int i=0; i< selections.childCount; ++i) {
            var toggle = selections.GetChild(i).GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((bool isOn)=>{ OnTogglesChanged(toggle.name); });
        }
    }

    void OnTogglesChanged(string name)
    {
        transform.Find("OK Button").GetComponent<Button>().enabled = true;
        switch (name) {
            case "Declare War":
                SetArgumentsSetters(new List<Type> { typeof(State) });
                break;
            case "Declare Aggressive War":
            case "Develop":
                SetArgumentsSetters(new List<Type> { typeof(Town) });
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
        foreach (var type in types) {
            y -= h;
            Transform o = null;
            if (type == typeof(Town)) {
                o = Instantiate(SelectTownButtonPrefab, argPanel).transform;
                o.name = "Select Town Button";
                o.GetComponent<Button>().onClick.AddListener(() => { OnSelectTownButtonClick(o.Find("Text").GetComponent<Text>()); });
            }
            if (type == typeof(State)) {
                o = Instantiate(SelectStateButtonPrefab, argPanel).transform;
                o.name = "Select State Button";
                o.GetComponent<Button>().onClick.AddListener(() => { OnSelectStateButtonClick(o.GetComponent<Button>()); });
            }
            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
    }

    public void OnCloseButtonClick()
    {
        if (GameObject.Find("Upper UI Canvas").transform.childCount <= 1) {
            var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
            mainCam.GetComponent<Animator>().SetTrigger("Focus");
        }
        GetComponent<Animator>().SetTrigger("Close");
    }

    /// <summary>
    /// 这个函数会在关闭动画的最后调用
    /// </summary>
    public void OnCloseAnimationFinished()
    {
        Destroy(transform.parent.gameObject);
    }

    public void OnOKButtonClick()
    {
        string name = null;
        var selections = transform.Find("Action Selections");
        for (int i =0; i<selections.childCount; ++i) {
            if(selections.GetChild(i).TryGetComponent<Toggle>(out Toggle toggle) && toggle.isOn) {
                name = toggle.name;
                break;
            }
        }
        switch (name) {
            case "Declare War":
                callback(new DeclareWarAction(null, null, stateSelected));
                break;
            case "Declare Aggressive War":
                callback(new DeclareAggressiveWarAction(null, null, townSelected.Controller, townSelected));
                break;
            case "Develop":
                callback(new DevelopAction(null, null, townSelected));
                break;
        }
        OnCloseButtonClick();
    }
}
