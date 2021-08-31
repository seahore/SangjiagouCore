using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SangjiagouCore;
using static SangjiagouCore.Utilities.ChineseNumerals;

public class UIHandler : MonoBehaviour
{
    public Canvas UICanvas;
    public Canvas UpperUICanvas;
    public Canvas TopUICanvas;
    public MapRenderer Tilemap;
    public GameObject ScenariosPanelPrefab;
    public GameObject SavedGamesPanelPrefab;
    public GameObject SettingsPanelPrefab;
    public GameObject CopyrightsPanelPrefab;
    public GameObject SelectSchoolPanelPrefab;
    public GameObject CheckStatesPanelPrefab;
    public GameObject CheckScholarsPanelPrefab;
    public GameObject NextTurnProgressPrefab;
    public GameObject NextTurnInfoPanelPrefab;
    public GameObject WarningBoxPrefab;
    public GameObject TooltipPrefab;
    public GameObject LoadingMaskPrefab;

    GameObject tooltip;
    Player player;
    MapRenderer mapRenderer;

    void Start()
    {
        tooltip = Instantiate(TooltipPrefab, TopUICanvas.transform);
        GameObject o = GameObject.FindGameObjectWithTag("Player");
        if (o is null || !o.TryGetComponent(out player))
            player = null;

        o = GameObject.Find("Version");
        if (!(o is null) && o.TryGetComponent(out Text text)) {
            text.text = "版本：" + Application.version;
        }

        var mapRendererObject = GameObject.FindGameObjectWithTag("Tilemap");
        if (!(mapRendererObject is null))
            mapRenderer = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<MapRenderer>();
    }

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

    public void OnCopyrightsButtonClick()
    {
        Instantiate(CopyrightsPanelPrefab, UpperUICanvas.transform);
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

    public void OnScenariosButtonClick()
    {
        Instantiate(ScenariosPanelPrefab, UpperUICanvas.transform);
    }

    public void OnSavedGamesButtonClick()
    {
        Instantiate(SavedGamesPanelPrefab, UpperUICanvas.transform);
    }

    public void OnSettingsButtonClick()
    {
        Instantiate(SettingsPanelPrefab, UpperUICanvas.transform);
    }

    public void OnMainMenuButtonClick()
    {
        SetLoadingMask("MainMenuScene", () => { });
    }

    public void OnCreditButtonClick()
    {
        var anim = GameObject.Find("Credit Panel").GetComponent<Animator>();
        anim.SetBool("Open", !anim.GetBool("Open"));
    }

    public void OnExitGameButtonClick()
    {
        Application.Quit();
    }

    public void OnActionsButtonClick()
    {
        var panel = GameObject.Find("Action Assignment Panel");
        var anim = panel.GetComponent<Animator>();
        if (!anim.GetBool("Open")) {
            panel.GetComponent<ActionAssignmentPanel>().Reset();
            anim.SetBool("Open", true);
        } else {
            anim.SetBool("Open", false);
        }
    }

    public void OnStatisticsColumnButtonClick()
    {
        var buttonText = GameObject.Find("Statistics Column Button/Text").GetComponent<Text>();
        var anim = GameObject.Find("Statistics Column Panel").GetComponent<Animator>();
        if (!anim.GetBool("Open")) {
            buttonText.text = "◀";
            anim.SetBool("Open", true);
        } else {
            buttonText.text = "▶";
            anim.SetBool("Open", false);
        }
    }

    public void OnCheckStatesButtonClick()
    {
        Instantiate(CheckStatesPanelPrefab, UpperUICanvas.transform);
    }

    public void OnCheckScholarsButtonClick()
    {
        var panel = Instantiate(CheckScholarsPanelPrefab, UpperUICanvas.transform);
        panel.GetComponent<CheckScholarsPanel>().ShowAllScholars();
    }

    public void OnNextTurnButtonClick()
    {
        HideAllPanels();
        var ntp = Instantiate(NextTurnProgressPrefab, UpperUICanvas.transform).transform;
        Game.CurrentEntities.NextTurn();
        GameObject.Find("Player Panel").transform.Find("Next Turn Button/Text").GetComponent<Text>().text = Game.CurrentEntities.DateToString();
        Tilemap.RefreshMap();
        ShowPanels(new List<string> { "Player Panel" });
        var ntip = Instantiate(NextTurnInfoPanelPrefab, UpperUICanvas.transform).transform;
        ntip.Find("Title").GetComponent<Text>().text = $"{Game.CurrentEntities.GetPlayerSchool(player.ID).Name}内部参考";
        ntip.Find("Date").GetComponent<Text>().text = Game.CurrentEntities.DateToString() + "刊";
        ntip.Find("Scroll View/Viewport/Info").GetComponent<Text>().text = Game.CurrentEntities.GetMonthlyReport();
        ntp.GetComponent<Animator>().SetTrigger("Close");
    }

    public void OnSelectTile(Vector2Int pos, bool selectState = false)
    {
        var sip = GameObject.Find("State Info Panel").GetComponent<StateInfoPanel>();
        var tip = GameObject.Find("Town Info Panel").GetComponent<TownInfoPanel>();
        Town town = null;
        foreach (var i in Game.CurrentEntities.Towns) {
            if (i.Position == pos) {
                town = i;
                break;
            }
        }
        if (town is null) {
            mapRenderer.SelectTile(pos);
            tip.Drawback();
            sip.Drawback();
            return;
        }
        if (selectState) {
            List<Vector2Int> selections = new List<Vector2Int>();
            foreach (var t in town.Controller.Territory) {
                selections.Add(t.Position);
            }
            mapRenderer.SelectTiles(selections);
            tip.Drawback();
            if (town is null) {
                sip.Drawback();
            } else {
                sip.Set(town.Controller);
                sip.Show();
            }
        } else {
            mapRenderer.SelectTile(pos);
            sip.Drawback();
            if (town is null) {
                tip.Drawback();
            } else {
                tip.Set(town);
                tip.Show();
            }
        }
    }

    public void SetLoadingMask(string sceneName, UnityAction onFadedIn)
    {
        var lm = Instantiate(LoadingMaskPrefab);
        lm.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        lm.GetComponent<LoadingMask>().LoadingSceneName = sceneName;
        lm.GetComponent<LoadingMask>().OnFadedIn += onFadedIn;
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
