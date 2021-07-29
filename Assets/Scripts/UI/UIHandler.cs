using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SangjiagouCore;

public class UIHandler : MonoBehaviour
{
    public GameObject UpperUICanvas;
    public GameObject SavedGamesPanelPrefab;
    public GameObject SettingsPanelPrefab;
    public GameObject WarningBoxPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
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
}
