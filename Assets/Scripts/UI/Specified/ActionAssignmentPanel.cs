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
    public GameObject DraftSuggestionButtonPrefab;
    public GameObject CheckScholarsPanelPrefab;

    static readonly Color colorForAssigned = new Color((float)0xAF / 0xFF, (float)0xEE / 0xFF, (float)0xEE / 0xFF);

    Town townSelected;
    StateAction suggestionDrafted;

    Player player;
    List<Scholar> selecting;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        var selections = transform.Find("Action Selections");
        for (int i = 0; i < selections.childCount; ++i) {
            var toggle = selections.GetChild(i).GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((bool isOn) => { OnTogglesChanged(isOn, toggle.name); });
        }
        Reset();
    }

    public void OnCheckScholarButtonClick()
    {
        var canvas = GameObject.FindGameObjectWithTag("UIHandler").GetComponent<UIHandler>().UpperUICanvas.transform;
        var panel = Instantiate(CheckScholarsPanelPrefab, canvas);
        School playerSchool = Game.CurrentEntities.GetPlayerSchool(player.ID);
        panel.GetComponent<CheckScholarsPanel>().ShowScholarsOf(playerSchool);
    }

    public void Reset()
    {
        townSelected = null;
        suggestionDrafted = null;

        School playerSchool = Game.CurrentEntities.GetPlayerSchool(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().ID);
        if (playerSchool is null) return;

        selecting = new List<Scholar>();

        var list = transform.Find("Scroll View/Viewport/Scholars List");
        for (int i = 0; i < list.childCount; ++i) {
            Destroy(list.GetChild(i).gameObject);
        }

        var selections = transform.Find("Action Selections");
        for (int i = 0; i < selections.childCount; ++i) {
            selections.GetChild(i).GetComponent<Toggle>().isOn = false;
        }

        var argumentsst = transform.Find("Action Arguments");
        for (int i = 0; i < list.childCount; ++i) {
            Destroy(list.GetChild(i).gameObject);
        }

        var unitHeight = ScholarSelection.GetComponent<RectTransform>().rect.height;
        var unitWidth = ScholarSelection.GetComponent<RectTransform>().rect.width;
        float th = -8;
        float tw = -8;
        for (int i = 0; i < playerSchool.Members.Count; ++i) {
            var s = playerSchool.Members[i];
            var o = Instantiate(ScholarSelection, list).transform;
            o.GetComponent<Toggle>().onValueChanged.AddListener((bool isOn) => {
                if (isOn) selecting.Add(s);
                else selecting.Remove(s);
            });
            o.GetComponent<RectTransform>().offsetMin = new Vector2(tw + 225, th - unitHeight);
            o.GetComponent<RectTransform>().offsetMax = new Vector2(tw + unitWidth + 225, th);
            o.Find("Avatar").GetComponent<Image>().sprite = s.Avatar;
            o.Find("Name").GetComponent<Text>().text = s.FullCourtesyName;
            if (!(s.Action is null)) {
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

    public void OnDraftSuggestionButtonClick(Text buttonText)
    {
        var dsp = transform.Find("Draft Suggestion Panel");
        if (dsp.GetComponent<Animator>().GetBool("Open")) return;
        dsp.GetComponent<DraftSuggestionPanel>().Reset();
        dsp.GetComponent<Animator>().SetBool("Open", true);
        dsp.GetComponent<DraftSuggestionPanel>().SetCallback((StateAction action) => {
            if (action is null) return;
            suggestionDrafted = action;
            string text = "建议进行" + "<b>" + action.Name + "</b>";
            buttonText.text = text;
            if(Settings.Values.AutoAssign) transform.Find("Assign Button").GetComponent<Button>().onClick.Invoke();
        });
    }

    public void SetArgumentsSetters(List<Type> types)
    {
        float x = 115;
        float y = 16;
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
            } else if (type == typeof(StateAction)) {
                o = Instantiate(DraftSuggestionButtonPrefab, argPanel).transform;
                o.name = "Draft Suggestion Button";
                o.GetComponent<Button>().onClick.AddListener(() => { OnDraftSuggestionButtonClick(o.Find("Text").GetComponent<Text>()); });
            }
            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            if (Settings.Values.AutoSelectMode && types.Count == 1)
                o.GetComponent<Button>().onClick.Invoke();
        }
    }

    void OnTogglesChanged(bool isOn, string name)
    {
        if (!isOn) return;

        var types = new List<Type>();
        var dsp = transform.Find("Draft Suggestion Panel");
        if (name != "Propose") {
            dsp.GetComponent<Animator>().SetBool("Open", false);
        }
        switch (name) {
            case "Idle":
                break;
            case "Travel":
                types.Add(typeof(Town));
                break;
            case "Lobbying":
                break;
            case "Propose":
                types.Add(typeof(StateAction));
                break;
        }
        SetArgumentsSetters(types);
    }

    public void OnAssignButtonClick()
    {
        if (selecting.Count == 0) return;

        string name = null;
        var selections = transform.Find("Action Selections");
        for (int i = 0; i < selections.childCount; ++i) {
            if (selections.GetChild(i).TryGetComponent(out Toggle toggle) && toggle.isOn) {
                name = toggle.name;
                break;
            }
        }
        if (name is null) return;

        switch (name) {
            case "Idle":
                foreach (var s in selecting) {
                    s.Action = null;
                }
                break;
            case "Travel":
                if (townSelected is null)
                    return;
                foreach (var s in selecting) {
                    if (s.Location != townSelected)
                        s.Action = new TravelAction(s, s.Location, townSelected);
                }
                break;
            case "Lobbying":
                foreach (var s in selecting) {
                    if (s.Location.IsCapital)
                        s.Action = new LobbyingAction(s, s.Location);
                }
                break;
            case "Propose":
                if (suggestionDrafted is null)
                    return;
                foreach (var s in selecting) {
                    if (!s.Location.IsCapital)
                        continue;
                    Type stateActionType = suggestionDrafted.GetType();
                    State actor = s.Location.Controller;
                    School prop = s.BelongTo;
                    bool invalid = false;
                    switch (stateActionType.Name) {
                        case "DeclareWarAction": {
                            var a = suggestionDrafted as DeclareWarAction;
                            if (a.Declaree == actor || !a.Declaree.Monarch.IsInvader || !actor.Neighbours.Contains(a.Declaree)) {
                                invalid = true;
                                break;
                            }
                            suggestionDrafted = new DeclareWarAction(actor, prop, a.Declaree);
                            break;
                        }
                        case "DeclareAggressiveWarAction": {
                            var a = suggestionDrafted as DeclareAggressiveWarAction;
                            if (a.Declaree == actor || !actor.NeighbourTowns.Contains(a.Target)) {
                                invalid = true;
                                break;
                            }
                            suggestionDrafted = new DeclareAggressiveWarAction(actor, prop, a.Declaree, a.Target);
                            break;
                        }
                        case "DevelopAction": {
                            var a = suggestionDrafted as DevelopAction;
                            if (!actor.Territory.Contains(a.Target)) {
                                invalid = true;
                                break;
                            }
                            suggestionDrafted = new DevelopAction(actor, prop, a.Target);
                            break;
                        }
                        case "CollectFolkSongsAction": {
                            var a = suggestionDrafted as CollectFolkSongsAction;
                            suggestionDrafted = new CollectFolkSongsAction(actor, prop);
                            break;
                        }
                        case "HuntAction": {
                            suggestionDrafted = new HuntAction(actor, prop);
                            break;
                        }
                        case "SacrificeAction": {
                            suggestionDrafted = new SacrificeAction(actor, prop);
                            break;
                        }
                        case "ImpressAction": {
                            suggestionDrafted = new ImpressAction(actor, prop);
                            break;
                        }
                        case "CommandeerAction": {
                            suggestionDrafted = new CommandeerAction(actor, prop);
                            break;
                        }
                        case "ImproveRelationshipsAction": {
                            var a = suggestionDrafted as ImproveRelationshipsAction;
                            if (a.Counterpart == actor) {
                                invalid = true;
                                break;
                            }
                            suggestionDrafted = new ImproveRelationshipsAction(actor, prop, a.Counterpart);
                            break;
                        }
                        case "PoliticsResearchAction": {
                            suggestionDrafted = new PoliticsResearchAction(actor, prop);
                            break;
                        }
                        case "MilitaryResearchAction": {
                            suggestionDrafted = new MilitaryResearchAction(actor, prop);
                            break;
                        }
                        case "RelievePoorAction": {
                            suggestionDrafted = new RelievePoorAction(actor, prop);
                            break;
                        }
                    }
                    if (!invalid)
                        s.Action = new ProposeAction(s, s.Location, suggestionDrafted);
                }
                break;
        }
        Reset();
    }
}
