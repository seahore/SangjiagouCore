using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SangjiagouCore;

public class UIHandler : MonoBehaviour
{
    public Canvas UICanvas;
    public Canvas UpperUICanvas;
    public Canvas TopUICanvas;
    public GameObject SavedGamesPanelPrefab;
    public GameObject SettingsPanelPrefab;
    public GameObject SelectSchoolPanelPrefab;
    public GameObject WarningBoxPrefab;
    public GameObject TooltipPrefab;

    GameObject tooltip;

    // Start is called before the first frame update
    void Start()
    {
        tooltip = Instantiate(TooltipPrefab, TopUICanvas.transform);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowWarningBox(string message)
    {
        var o = Instantiate(WarningBoxPrefab, UpperUICanvas.transform);
        o.transform.Find("Text").GetComponent<Text>().text = message;
    }

    public void OnMenuButtonClick()
    {
        var anim = GameObject.Find("Menu Panel").GetComponent<Animator>();
        anim.SetBool("Open", !anim.GetBool("Open"));
    }

    public void OnSwitchSchoolButtonClick()
    {
        Instantiate(SelectSchoolPanelPrefab, UpperUICanvas.transform);
    }

    public void OnQuickSaveButtonClick()
    {
        try {
            Game.SaveGame($"QuickSave{System.DateTime.Now:yyyy-MM-dd-HH-mm}.sjg", false);
        } catch (System.IO.IOException) {
            ShowWarningBox($"存档文件正在被占用");
        } catch (System.Exception e) {
            ShowWarningBox(e.Message);
        }
    }
    public void OnSavedGamesButtonClick()
    {
        Instantiate(SavedGamesPanelPrefab, UpperUICanvas.transform);
    }

    public void OnSettingsButtonClick()
    {
        Instantiate(SettingsPanelPrefab, UpperUICanvas.transform);
    }

    public void OnExitGameButtonClick()
    {
        Application.Quit();
    }

    public void OnTriggerWarButtonClick()
    {
        War war = new War(Game.CurrentEntities.States[0], Game.CurrentEntities.States[1]);
        war.Settle();
        War.Report report = war.GetReport();
        Debug.Log($"AttackerWon: {report.AttackerWon}\nAttackers: {war.Attackers} Defenders: { war.Defenders}\nAttackerLoss: {report.AttackerLoss} DefenderLoss: {report.DefenderLoss}");
    }

    public void OnActionsButtonClick()
    {
        var panel = GameObject.Find("Action Assignment Panel");
        panel.GetComponent<ActionAssignmentPanel>().Refresh();
        var anim = panel.GetComponent<Animator>();
        anim.SetBool("Open", !anim.GetBool("Open"));
    }

    public void OnNextTurnButtonClick()
    {
        Game.CurrentEntities.NextTurn();
        var states = Game.CurrentEntities.States;
        string str = $"{Game.CurrentEntities.Year}年{Game.CurrentEntities.Month}月\n";
        foreach (var s in states) {
            str += $"{s.Name} - 民生：{s.Satisfaction} 礼乐：{s.Ceremony}人口：{s.Population} 兵士：{s.Army} 粮食：{s.Food }\n";
        }
        Debug.Log(str);
    }

    public void OnSelectTile(Town town)
    {
        var t = GameObject.Find("Town Info Panel").GetComponent<TownInfoPanel>();
        if (town is null) {
            t.Drawback();
        } else {
            t.Set(town);
            t.Show();
        }
    }

    public void DisplayTooltip(bool display, string content, Vector2 pos)
    {
        tooltip.transform.SetPositionAndRotation(new Vector3(pos.x + 1, pos.y + 1), Quaternion.identity);
        var anim = tooltip.GetComponent<Animator>();
        if (display && content.Length > 0) {
            var cont = tooltip.transform.Find("Content");
            var rectTrans = tooltip.transform.GetComponent<RectTransform>();
            cont.GetComponent<Text>().text = content;
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cont.GetComponent<RectTransform>().rect.width + 10);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cont.GetComponent<RectTransform>().rect.height + 5);
            anim.SetBool("Display", true);
        } else {
            anim.SetBool("Display", false);
        }
    }

    public List<string> RecordOpeningPanels()
    {
        List<string> l = new List<string>();
        for (int i = 0; i < UICanvas.transform.childCount; ++i) {
            var o = UICanvas.transform.GetChild(i);
            if (o.TryGetComponent(out Animator anim) && anim.GetBool("Open")) {
                l.Add(o.name);
            }
        }
        return l;
    }

    public void HideAllPanels()
    {
        for (int i = 0; i < UICanvas.transform.childCount; ++i) {
            if (UICanvas.transform.GetChild(i).TryGetComponent(out Animator anim)) {
                anim.SetBool("Open", false);
            }
        }
    }

    public void HideAllUpperPanels()
    {
        if (UpperUICanvas.transform.childCount > 0) {
            var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
            mainCam.GetComponent<Animator>().SetTrigger("Focus");
            UpperUICanvas.gameObject.SetActive(false);
        }
    }

    public void ShowPanels(List<string> l)
    {
        for (int i = 0; i < UICanvas.transform.childCount; ++i) {
            var o = UICanvas.transform.GetChild(i);
            if (l.Contains(o.name) && o.TryGetComponent(out Animator anim)) {
                anim.SetBool("Open", true);
            }
        }
    }

    public void ShowUpperPanels()
    {
        if (UpperUICanvas.transform.childCount > 0) {
            var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
            mainCam.GetComponent<Animator>().SetTrigger("Unfocus");
            UpperUICanvas.gameObject.SetActive(true);
        }
    }

    public void ShowTopBanner(string text, string buttonText, UnityAction onButtonClick)
    {
        var banner = UICanvas.transform.Find("Top Banner");
        banner.Find("Text").GetComponent<Text>().text = text;
        var button = banner.Find("Button").GetComponent<Button>();
        button.transform.Find("Text").GetComponent<Text>().text = buttonText;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onButtonClick);
        banner.GetComponent<Animator>().SetBool("Open", true);
    }

    public void HideTopBanner()
    {
        UICanvas.transform.Find("Top Banner").GetComponent<Animator>().SetBool("Open", false);
    }
}
